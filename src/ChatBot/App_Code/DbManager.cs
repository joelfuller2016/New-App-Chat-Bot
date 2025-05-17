// Legacy DbManager - Redirects to new implementation in ChatBot.Data namespace
using System;
using System.Data.SQLite;
using ChatBot.Data;

namespace App_Code
{
    public static class DbManager
    {
        public static void Initialize()
        {
            // Forward to new implementation
            ChatBot.Data.DbManager.EnsureDatabaseExists();
        }

        public static SQLiteConnection GetConnection()
        {
            // Forward to new implementation
            return ChatBot.Data.DbManager.GetConnection();
        }
    }
}
