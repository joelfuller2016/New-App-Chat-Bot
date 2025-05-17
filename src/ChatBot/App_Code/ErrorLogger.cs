using System;
using System.Data.SQLite;

namespace App_Code
{
    public static class ErrorLogger
    {
        public static void Log(Exception ex)
        {
            using var conn = DbManager.GetConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Errors(Message, StackTrace, CreatedAt) VALUES(@m, @s, @c)";
            cmd.Parameters.AddWithValue("@m", ex.Message);
            cmd.Parameters.AddWithValue("@s", ex.StackTrace);
            cmd.Parameters.AddWithValue("@c", DateTime.UtcNow);
            cmd.ExecuteNonQuery();
        }
    }
}
