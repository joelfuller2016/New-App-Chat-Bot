// Legacy ErrorLogger - Redirects to new implementation in ChatBot.Services.Logging namespace
using System;
using ChatBot;
using ChatBot.Services.Logging;

namespace App_Code
{
    public static class ErrorLogger
    {
        public static void Log(Exception ex)
        {
            // Forward to new implementation
            var errorLogger = DependencyResolver.Resolve<IErrorLogger>();
            errorLogger.Log(ex);
        }
    }
}
