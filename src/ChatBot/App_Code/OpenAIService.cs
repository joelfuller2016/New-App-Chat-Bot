using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace App_Code
{
    public class OpenAIService
    {
        private readonly string _apiKey;
        private readonly string _model;

        public OpenAIService(string apiKey, string model)
        {
            _apiKey = apiKey;
            _model = model;
        }

        public async Task<string> SendMessageAsync(string message)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var payload = new JObject
            {
                ["model"] = _model,
                ["messages"] = new JArray
                {
                    new JObject { ["role"] = "user", ["content"] = message }
                }
            };

            var content = new StringContent(payload.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
