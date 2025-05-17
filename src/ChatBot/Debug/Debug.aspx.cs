using System;
using System.Data.SQLite;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using App_Code;

namespace ChatBot.Debug
{
    public partial class DebugPage : Page
    {
        protected HtmlGenericControl Logs;

        protected void Page_Load(object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            using var conn = DbManager.GetConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Message, ResponseJson, CreatedAt FROM Chats ORDER BY CreatedAt DESC";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                sb.AppendLine(reader.GetString(0));
                sb.AppendLine(reader.GetString(1));
                sb.AppendLine(reader.GetDateTime(2).ToString());
                sb.AppendLine("---");
            }

            cmd.CommandText = "SELECT Message, StackTrace, CreatedAt FROM Errors ORDER BY CreatedAt DESC";
            using var er = cmd.ExecuteReader();
            sb.AppendLine("Errors:");
            while (er.Read())
            {
                sb.AppendLine(er.GetString(0));
                sb.AppendLine(er.GetString(1));
                sb.AppendLine(er.GetDateTime(2).ToString());
                sb.AppendLine("---");
            }

            Logs.InnerText = sb.ToString();
        }
    }
}
