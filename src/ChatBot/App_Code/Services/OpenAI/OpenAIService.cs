using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ChatBot.Data.Repositories;
using ChatBot.Data.Models;
using System.Linq;

namespace ChatBot.Services.OpenAI
{
    /// <summary>
    /// Service for interacting with the OpenAI API
    /// </summary>
    public class OpenAIService : IOpenAIService
    {
        private readonly IRepository<ConfigModel> _configRepository;
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://api.openai.com/v1/chat/completions";
        private const string DefaultModel = "gpt-3.5-turbo";
        private const int MaxRetries = 3;

        public OpenAIService(IRepository<ConfigModel> configRepository)
        {
            _configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<string> GetChatCompletionAsync(string prompt, string model = null, 
            double temperature = 0.7, int maxTokens = 2000)
        {
            var messages = new List<ChatMessage>
            {
                new ChatMessage { Role = "user", Content = prompt }
            };

            return await GetChatCompletionWithHistoryAsync(
                messages, model, temperature, maxTokens);
        }

        public async Task<string> GetChatCompletionWithHistoryAsync(List<ChatMessage> messages, 
            string model = null, double temperature = 0.7, int maxTokens = 2000)
        {
            if (messages == null || messages.Count == 0)
                throw new ArgumentException("Messages cannot be null or empty", nameof(messages));

            // Get API key from configuration
            var apiKey = GetConfigValue("OpenAIApiKey");
            if (string.IsNullOrEmpty(apiKey))
                throw new InvalidOperationException("OpenAI API key is not configured");

            // Set model from configuration if not specified
            if (string.IsNullOrEmpty(model))
            {
                model = GetConfigValue("OpenAIModel");
                if (string.IsNullOrEmpty(model))
                    model = DefaultModel;
            }

            // Prepare request data
            var requestData = new
            {
                model = model,
                messages = messages.Select(m => new { role = m.Role, content = m.Content }).ToList(),
                temperature = temperature,
                max_tokens = maxTokens
            };

            var requestJson = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            // Set API key in headers
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            // Execute request with retry logic
            HttpResponseMessage response = null;
            string responseContent = null;
            int retryCount = 0;
            bool success = false;

            while (!success && retryCount < MaxRetries)
            {
                try
                {
                    response = await _httpClient.PostAsync(ApiUrl, content);
                    responseContent = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();
                    success = true;
                }
                catch (HttpRequestException ex)
                {
                    retryCount++;
                    if (retryCount >= MaxRetries)
                        throw new Exception($"Failed to communicate with OpenAI API after {MaxRetries} attempts", ex);

                    // Exponential backoff
                    await Task.Delay(1000 * (int)Math.Pow(2, retryCount));
                }
            }

            // Parse response
            dynamic jsonResponse = JsonConvert.DeserializeObject(responseContent);
            
            if (jsonResponse.choices == null || jsonResponse.choices.Count == 0)
                throw new Exception("Invalid response from OpenAI API");

            return jsonResponse.choices[0].message.content.ToString();
        }

        private string GetConfigValue(string key)
        {
            var config = _configRepository
                .Find(c => c.Key == key)
                .FirstOrDefault();

            return config?.Value;
        }
    }
}