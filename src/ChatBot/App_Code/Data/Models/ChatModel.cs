using System;

namespace ChatBot.Data.Models
{
    /// <summary>
    /// Model representing a chat message and response
    /// </summary>
    public class ChatModel
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Message { get; set; }
        public string Response { get; set; }
        public string RawRequest { get; set; }
        public string RawResponse { get; set; }
        public DateTime DateCreated { get; set; }
    }
}