using System;
using System.Web;
using System.Web.Security;
using ChatBot.Data.Models;
using ChatBot.Data.Repositories;
using ChatBot.Services.Security;

namespace ChatBot.Services.Auth
{
    /// <summary>
    /// Interface for authentication operations
    /// </summary>
    public interface IAuthManager
    {
        bool AuthenticateUser(string username, string password);
        void SignOut();
        bool IsAuthenticated();
        string GetGoogleOAuthUrl();
        bool ProcessGoogleOAuthCallback(string code);
    }

    /// <summary>
    /// Service for authentication and authorization
    /// </summary>
    public class AuthManager : IAuthManager
    {
        private readonly IRepository<UserModel> _userRepository;
        private readonly IRepository<ConfigModel> _configRepository;
        private readonly ISecurityService _securityService;

        public AuthManager(
            IRepository<UserModel> userRepository,
            IRepository<ConfigModel> configRepository,
            ISecurityService securityService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
            _securityService = securityService ?? throw new ArgumentNullException(nameof(securityService));
        }

        public bool AuthenticateUser(string username, string password)
        {
            // Verify username and password
            var isValid = _userRepository.VerifyPassword(username, password);
            
            if (isValid)
            {
                // Get user details
                var user = _userRepository.GetByUsername(username);
                
                // Create authentication ticket
                FormsAuthentication.SetAuthCookie(username, true);
                
                // Store user info in session
                HttpContext.Current.Session["UserId"] = user.Id;
                HttpContext.Current.Session["Username"] = user.Username;
                HttpContext.Current.Session["IsAdmin"] = user.IsAdmin;
            }
            
            return isValid;
        }

        public void SignOut()
        {
            // Clear authentication ticket
            FormsAuthentication.SignOut();
            
            // Clear session
            HttpContext.Current.Session.Clear();
        }

        public bool IsAuthenticated()
        {
            return HttpContext.Current.User.Identity.IsAuthenticated;
        }

        public string GetGoogleOAuthUrl()
        {
            var clientId = GetConfigValue("GoogleClientId");
            if (string.IsNullOrEmpty(clientId))
            {
                throw new InvalidOperationException("Google OAuth is not configured");
            }

            // Generate state token to prevent CSRF
            var state = _securityService.GenerateAntiForgeryToken();
            
            // Get the redirect URI
            var redirectUri = HttpUtility.UrlEncode(GetOAuthRedirectUri());
            
            // Create the OAuth URL
            return $"https://accounts.google.com/o/oauth2/auth" +
                $"?client_id={clientId}" +
                $"&redirect_uri={redirectUri}" +
                $"&response_type=code" +
                $"&scope=openid%20email%20profile" +
                $"&state={state}";
        }

        public bool ProcessGoogleOAuthCallback(string code)
        {
            // This is a simplified implementation
            // In a real application, you would exchange the code for tokens
            // and use the tokens to get the user info from Google
            
            // For now, just create a temporary user
            var user = new UserModel
            {
                Username = "google_user_" + Guid.NewGuid().ToString().Substring(0, 8),
                Email = "googleuser@example.com",
                IsAdmin = false,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow
            };
            
            _userRepository.Add(user);
            
            // Create authentication ticket
            FormsAuthentication.SetAuthCookie(user.Username, true);
            
            // Store user info in session
            HttpContext.Current.Session["UserId"] = user.Id;
            HttpContext.Current.Session["Username"] = user.Username;
            HttpContext.Current.Session["IsAdmin"] = user.IsAdmin;
            
            return true;
        }

        private string GetConfigValue(string key)
        {
            var config = _configRepository
                .Find(c => c.Key == key)
                .FirstOrDefault();

            return config?.Value;
        }

        private string GetOAuthRedirectUri()
        {
            // Construct the full callback URL
            var request = HttpContext.Current.Request;
            string authority = request.Url.GetLeftPart(UriPartial.Authority);
            return $"{authority}/Login.aspx";
        }
    }
}