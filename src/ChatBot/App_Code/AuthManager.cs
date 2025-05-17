using System;
using System.Web;

namespace App_Code
{
    public static class AuthManager
    {
        // Simplified Google OAuth redirect using configured client ID
        public static void SignIn(HttpResponse response)
        {
            string clientId = string.Empty;
            using (var conn = DbManager.GetConnection())
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT GoogleClientId FROM Config WHERE Id = 1";
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    clientId = reader.GetString(0);
                }
            }

            // TODO: implement full OAuth flow and token handling
            var redirectUri = HttpUtility.UrlEncode("/Login.aspx");
            var url = $"https://accounts.google.com/o/oauth2/auth?client_id={clientId}&redirect_uri={redirectUri}&response_type=code&scope=openid%20email";
            response.Redirect(url);
        }
    }
}
