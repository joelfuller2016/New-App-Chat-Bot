using System;
using System.Data.SQLite;
using System.Web;
using ChatBot.Data;
using ChatBot.Data.Models;
using ChatBot.Data.Repositories;

namespace ChatBot.Services.Logging
{
    /// <summary>
    /// Interface for error logging operations
    /// </summary>
    public interface IErrorLogger
    {
        void Log(Exception ex);
        void Log(string message, string source = null, string stackTrace = null);
    }

    /// <summary>
    /// Service for logging errors to the database
    /// </summary>
    public class ErrorLogger : IErrorLogger
    {
        private readonly IRepository<ErrorModel> _errorRepository;

        public ErrorLogger(IRepository<ErrorModel> errorRepository)
        {
            _errorRepository = errorRepository ?? throw new ArgumentNullException(nameof(errorRepository));
        }

        public void Log(Exception ex)
        {
            if (ex == null) return;

            var error = new ErrorModel
            {
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                Source = ex.Source,
                Url = HttpContext.Current?.Request?.Url?.ToString(),
                UserId = GetCurrentUserId(),
                DateCreated = DateTime.UtcNow
            };

            _errorRepository.Add(error);
        }

        public void Log(string message, string source = null, string stackTrace = null)
        {
            var error = new ErrorModel
            {
                Message = message,
                StackTrace = stackTrace,
                Source = source,
                Url = HttpContext.Current?.Request?.Url?.ToString(),
                UserId = GetCurrentUserId(),
                DateCreated = DateTime.UtcNow
            };

            _errorRepository.Add(error);
        }

        private int? GetCurrentUserId()
        {
            // Get user ID from session or authentication ticket
            if (HttpContext.Current?.Session != null && 
                HttpContext.Current.Session["UserId"] != null)
            {
                return Convert.ToInt32(HttpContext.Current.Session["UserId"]);
            }

            return null;
        }
    }
}