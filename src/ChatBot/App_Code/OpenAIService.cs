// Legacy OpenAIService - Redirects to new implementation in ChatBot.Services.OpenAI namespace
using System;
using System.Threading.Tasks;
using ChatBot;
using ChatBot.Services.OpenAI;

namespace App_Code
{
    public class OpenAIService
    {     
        private readonly string _apiKey;
        private readonly string _model;
        private readonly IOpenAIService _service;

        public OpenAIService(string apiKey, string model)
        {
            _apiKey = apiKey;
            _model = model;
            _service = DependencyResolver.Resolve<IOpenAIService>();
        }

        public async Task<string> SendMessageAsync(string message)
        {
            // Forward to new implementation
            return await _service.GetChatCompletionAsync(message, _model);
        }
    }
}
