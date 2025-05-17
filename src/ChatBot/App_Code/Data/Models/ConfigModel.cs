using System;

namespace ChatBot.Data.Models
{
    /// <summary>
    /// Model representing configuration settings
    /// </summary>
    public class ConfigModel
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public bool Encrypted { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}