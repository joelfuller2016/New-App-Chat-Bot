// Legacy AuthManager - Will be replaced with new implementation
using System;
using System.Web;
using ChatBot.Data;
using ChatBot.Data.Models;
using ChatBot.Data.Repositories;

namespace App_Code
{
    public static class AuthManager
    {
        // Simplified Google OAuth redirect using configured client ID
        public static void SignIn(HttpResponse response)
        {    
            string clientId = string.Empty;
            var configRepo = new ConfigRepository(new ChatBot.Services.Security.SecurityService());
            var clientIdConfig = configRepo.GetByKey("GoogleClientId");
            if (clientIdConfig != null)
            {
                clientId = clientIdConfig.Value;
            }

            // TODO: implement full OAuth flow and token handling
            var redirectUri = HttpUtility.UrlEncode("/Login.aspx");
            var url = $"https://accounts.google.com/o/oauth2/auth?client_id={clientId}&redirect_uri={redirectUri}&response_type=code&scope=openid%20email";
            response.Redirect(url);
        }
    }
}
