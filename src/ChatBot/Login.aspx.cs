using System;
using System.Web.UI;
using ChatBot.Services.Auth;
using ChatBot.Services.Security;

namespace ChatBot
{
    public partial class LoginPage : Page
    {
        private readonly IAuthManager _authManager;
        private readonly ISecurityService _securityService;

        public LoginPage()
        {
            _authManager = DependencyResolver.Resolve<IAuthManager>();
            _securityService = DependencyResolver.Resolve<ISecurityService>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check for logout request
            if (Request.QueryString["logout"] != null)
            {
                _authManager.SignOut();
                Response.Redirect("~/Login.aspx");
                return;
            }

            // Process OAuth callback
            if (Request.QueryString["code"] != null)
            {
                string code = Request.QueryString["code"];
                bool success = _authManager.ProcessGoogleOAuthCallback(code);

                if (success)
                {
                    Response.Redirect("~/Chat/Chat.aspx");
                    return;
                }
                else
                {
                    lblError.Text = "OAuth authentication failed.";
                    return;
                }
            }

            // Check if user is already logged in
            if (_authManager.IsAuthenticated())
            {
                Response.Redirect("~/Chat/Chat.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // Generate anti-forgery token
                AntiForgeryToken.Value = _securityService.GenerateAntiForgeryToken();
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // Validate anti-forgery token
            if (!_securityService.ValidateAntiForgeryToken(AntiForgeryToken.Value))
            {
                lblError.Text = "Invalid request. Please try again.";
                return;
            }

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            try
            {
                bool isAuthenticated = _authManager.AuthenticateUser(username, password);

                if (isAuthenticated)
                {
                    Response.Redirect("~/Chat/Chat.aspx");
                }
                else
                {
                    lblError.Text = "Invalid username or password.";
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "An error occurred during login. Please try again.";
                // Log the error
                var errorLogger = DependencyResolver.Resolve<ChatBot.Services.Logging.IErrorLogger>();
                errorLogger.Log(ex);
            }
        }

        protected void btnGoogleLogin_Click(object sender, EventArgs e)
        {
            try
            {
                // Initiate Google OAuth flow
                string redirectUrl = _authManager.GetGoogleOAuthUrl();
                Response.Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                lblError.Text = "An error occurred with Google login. Please try again.";
                // Log the error
                var errorLogger = DependencyResolver.Resolve<ChatBot.Services.Logging.IErrorLogger>();
                errorLogger.Log(ex);
            }
        }
    }
}