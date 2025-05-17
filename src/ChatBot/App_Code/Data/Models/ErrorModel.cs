using System;

namespace ChatBot.Data.Models
{
    /// <summary>
    /// Model representing an error log
    /// </summary>
    public class ErrorModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string Source { get; set; }
        public string Url { get; set; }
        public int? UserId { get; set; }
        public DateTime DateCreated { get; set; }
    }
}