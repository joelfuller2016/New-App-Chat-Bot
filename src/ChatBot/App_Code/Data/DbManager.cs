using System;
using System.Data.SQLite;
using System.IO;
using System.Web;

namespace ChatBot.Data
{
    /// <summary>
    /// Manages database connections and creation
    /// </summary>
    public class DbManager
    {
        private const string DbFileName = "ChatBot.db";
        private static readonly string DbPath = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), DbFileName);
        private static readonly string ConnectionString = $"Data Source={DbPath};Version=3;";

        private static bool _initialized = false;
        private static readonly object _lock = new object();

        /// <summary>
        /// Gets a connection to the SQLite database, creating it if it doesn't exist
        /// </summary>
        public static SQLiteConnection GetConnection()
        {
            EnsureDatabaseExists();
            var connection = new SQLiteConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Ensures the database exists and creates it if it doesn't
        /// </summary>
        public static void EnsureDatabaseExists()
        {
            if (_initialized) return;

            lock (_lock)
            {
                if (_initialized) return;

                var directory = Path.GetDirectoryName(DbPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                bool createTables = !File.Exists(DbPath);
                
                if (createTables)
                {
                    SQLiteConnection.CreateFile(DbPath);
                    using (var connection = new SQLiteConnection(ConnectionString))
                    {
                        connection.Open();
                        CreateTables(connection);
                        InsertDefaultData(connection);
                    }
                }

                _initialized = true;
            }
        }

        private static void CreateTables(SQLiteConnection connection)
        {
            // Create Config table for application settings
            ExecuteNonQuery(connection, @"
                CREATE TABLE Config (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Key TEXT NOT NULL UNIQUE,
                    Value TEXT,
                    Encrypted INTEGER DEFAULT 0,
                    DateCreated TEXT DEFAULT CURRENT_TIMESTAMP,
                    DateModified TEXT DEFAULT CURRENT_TIMESTAMP
                );
            ");

            // Create Users table for authentication
            ExecuteNonQuery(connection, @"
                CREATE TABLE Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT,
                    PasswordSalt TEXT,
                    Email TEXT,
                    IsAdmin INTEGER DEFAULT 0,
                    DateCreated TEXT DEFAULT CURRENT_TIMESTAMP,
                    DateModified TEXT DEFAULT CURRENT_TIMESTAMP
                );
            ");

            // Create Chats table for conversation storage
            ExecuteNonQuery(connection, @"
                CREATE TABLE Chats (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER,
                    Message TEXT,
                    Response TEXT,
                    RawRequest TEXT,
                    RawResponse TEXT,
                    DateCreated TEXT DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY(UserId) REFERENCES Users(Id)
                );
            ");

            // Create Errors table for error logging
            ExecuteNonQuery(connection, @"
                CREATE TABLE Errors (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Message TEXT,
                    StackTrace TEXT,
                    Source TEXT,
                    Url TEXT,
                    UserId INTEGER,
                    DateCreated TEXT DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY(UserId) REFERENCES Users(Id)
                );
            ");
        }

        private static void InsertDefaultData(SQLiteConnection connection)
        {
            // Create default admin user with password 'admin'
            // In production, use a more secure password and proper hashing
            string defaultUsername = "admin";
            string defaultPasswordHash = "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918"; // SHA-256 of 'admin'
            string defaultSalt = Guid.NewGuid().ToString();

            ExecuteNonQuery(connection, @"
                INSERT INTO Users (Username, PasswordHash, PasswordSalt, IsAdmin)
                VALUES (@Username, @PasswordHash, @PasswordSalt, 1);
            ", new SQLiteParameter("@Username", defaultUsername),
               new SQLiteParameter("@PasswordHash", defaultPasswordHash),
               new SQLiteParameter("@PasswordSalt", defaultSalt));

            // Insert default configuration
            ExecuteNonQuery(connection, @"
                INSERT INTO Config (Key, Value, Encrypted) VALUES ('OpenAIApiKey', '', 1);
                INSERT INTO Config (Key, Value, Encrypted) VALUES ('OpenAIModel', 'gpt-3.5-turbo', 0);
                INSERT INTO Config (Key, Value, Encrypted) VALUES ('GoogleClientId', '', 1);
                INSERT INTO Config (Key, Value, Encrypted) VALUES ('GoogleClientSecret', '', 1);
            ");
        }

        private static void ExecuteNonQuery(SQLiteConnection connection, string commandText, params SQLiteParameter[] parameters)
        {
            using (var command = new SQLiteCommand(commandText, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                command.ExecuteNonQuery();
            }
        }
    }
}