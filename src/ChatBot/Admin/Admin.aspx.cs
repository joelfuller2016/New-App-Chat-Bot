using System;
using System.Data.SQLite;
using System.Text;

namespace ChatBot.Admin
{
    public partial class Admin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            App_Code.DbManager.Initialize();
            if (!Authenticate())
            {
                Response.StatusCode = 401;
                Response.AddHeader("WWW-Authenticate", "Basic realm=\"Admin\"");
                Response.End();
                return;
            }

            if (!IsPostBack)
            {
                using var conn = App_Code.DbManager.GetConnection();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT ApiKey, Model, GoogleClientId, GoogleClientSecret, AdminUser, AdminPassword FROM Config WHERE Id = 1";
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    ApiKey.Text = reader.GetString(0);
                    Model.Text = reader.GetString(1);
                    GoogleClientId.Text = reader.GetString(2);
                    GoogleClientSecret.Text = reader.GetString(3);
                    AdminUser.Text = reader.GetString(4);
                    AdminPassword.Text = reader.GetString(5);
                }
            }
        }

        private bool Authenticate()
        {
            string auth = Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(auth) && auth.StartsWith("Basic "))
            {
                string encoded = auth.Substring("Basic ".Length).Trim();
                string decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
                var parts = decoded.Split(':');
                if (parts.Length == 2)
                {
                    string user = parts[0];
                    string pass = parts[1];
                    using var conn = App_Code.DbManager.GetConnection();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT AdminUser, AdminPassword FROM Config WHERE Id = 1";
                    using var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        return user == reader.GetString(0) && pass == reader.GetString(1);
                    }
                }
            }
            return false;
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            using var conn = App_Code.DbManager.GetConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "REPLACE INTO Config(Id, ApiKey, Model, GoogleClientId, GoogleClientSecret, AdminUser, AdminPassword) VALUES(1, @k, @m, @cid, @sec, @user, @pass)";
            cmd.Parameters.AddWithValue("@k", ApiKey.Text);
            cmd.Parameters.AddWithValue("@m", Model.Text);
            cmd.Parameters.AddWithValue("@cid", GoogleClientId.Text);
            cmd.Parameters.AddWithValue("@sec", GoogleClientSecret.Text);
            cmd.Parameters.AddWithValue("@user", AdminUser.Text);
            cmd.Parameters.AddWithValue("@pass", AdminPassword.Text);
            cmd.ExecuteNonQuery();
        }
    }
}
