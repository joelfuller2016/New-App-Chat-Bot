using System;
using System.Data.SQLite;
using System.Web.UI;
using App_Code;

namespace ChatBot.Chat
{
    public partial class ChatPage : Page
    {
        protected async void SendButton_Click(object sender, EventArgs e)
        {
            string apiKey = string.Empty;
            string model = string.Empty;
            using (var conn = DbManager.GetConnection())
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT ApiKey, Model FROM Config WHERE Id = 1";
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    apiKey = reader.GetString(0);
                    model = reader.GetString(1);
                }
            }

            var service = new OpenAIService(apiKey, model);
            try
            {
                var json = await service.SendMessageAsync(UserMessage.Text);
                JsonResponse.InnerText = json;

                using var conn = DbManager.GetConnection();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO Chats(UserId, Message, ResponseJson, CreatedAt) VALUES(@u, @m, @r, @c)";
                cmd.Parameters.AddWithValue("@u", User.Identity.Name ?? "anon");
                cmd.Parameters.AddWithValue("@m", UserMessage.Text);
                cmd.Parameters.AddWithValue("@r", json);
                cmd.Parameters.AddWithValue("@c", DateTime.UtcNow);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                JsonResponse.InnerText = "Error: " + ex.Message;
            }
        }
    }
}
