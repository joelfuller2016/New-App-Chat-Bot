using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ChatBot.Services.OpenAI
{
    /// <summary>
    /// Interface for OpenAI API operations
    /// </summary>
    public interface IOpenAIService
    {
        Task<string> GetChatCompletionAsync(string prompt, string model = null, 
            double temperature = 0.7, int maxTokens = 2000);
        Task<string> GetChatCompletionWithHistoryAsync(List<ChatMessage> messages, 
            string model = null, double temperature = 0.7, int maxTokens = 2000);
    }

    /// <summary>
    /// Message for chat conversations
    /// </summary>
    public class ChatMessage
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }
}