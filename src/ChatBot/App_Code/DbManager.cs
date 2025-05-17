using System;
using System.Data.SQLite;
using System.IO;

namespace App_Code
{
    public static class DbManager
    {
        private static readonly string DbPath = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "chatbot.db");
        private const string ConnectionString = "Data Source=" + "|DataDirectory|chatbot.db";

        public static void Initialize()
        {
            Directory.CreateDirectory(AppDomain.CurrentDomain.GetData("DataDirectory").ToString());
            if (!File.Exists(DbPath))
            {
                SQLiteConnection.CreateFile(DbPath);
                using var conn = new SQLiteConnection(ConnectionString);
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"CREATE TABLE Config(
                                        Id INTEGER PRIMARY KEY,
                                        ApiKey TEXT,
                                        Model TEXT,
                                        GoogleClientId TEXT,
                                        GoogleClientSecret TEXT,
                                        AdminUser TEXT,
                                        AdminPassword TEXT);
                                    CREATE TABLE Chats(Id INTEGER PRIMARY KEY AUTOINCREMENT, UserId TEXT, Message TEXT, ResponseJson TEXT, CreatedAt DATETIME);
                                    CREATE TABLE Errors(Id INTEGER PRIMARY KEY AUTOINCREMENT, Message TEXT, StackTrace TEXT, CreatedAt DATETIME);";
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"INSERT INTO Config(Id, ApiKey, Model, GoogleClientId, GoogleClientSecret, AdminUser, AdminPassword)
                                    VALUES(1, '', 'gpt-3.5-turbo', '', '', 'admin', 'password');";
                cmd.ExecuteNonQuery();
            }
        }

        public static SQLiteConnection GetConnection()
        {
            var conn = new SQLiteConnection(ConnectionString);
            conn.Open();
            return conn;
        }
    }
}
