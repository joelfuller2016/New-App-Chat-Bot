using System;
using System.Collections.Generic;
using System.Data.SQLite;
using ChatBot.Data.Models;
using System.Linq;

namespace ChatBot.Data.Repositories
{
    /// <summary>
    /// Repository for chat messages and responses
    /// </summary>
    public class ChatRepository : IRepository<ChatModel>
    {
        public ChatRepository()
        {
        }

        public ChatModel GetById(int id)
        {
            using (var connection = DbManager.GetConnection())
            {
                using (var command = new SQLiteCommand("SELECT * FROM Chats WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return ReadChatFromReader(reader);
                        }
                    }
                }
            }
            return null;
        }

        public IEnumerable<ChatModel> GetAll()
        {
            var chats = new List<ChatModel>();
            using (var connection = DbManager.GetConnection())
            {
                using (var command = new SQLiteCommand("SELECT * FROM Chats ORDER BY DateCreated DESC", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            chats.Add(ReadChatFromReader(reader));
                        }
                    }
                }
            }
            return chats;
        }

        public IEnumerable<ChatModel> Find(Func<ChatModel, bool> predicate)
        {
            return GetAll().Where(predicate);
        }

        public void Add(ChatModel entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            using (var connection = DbManager.GetConnection())
            {
                using (var command = new SQLiteCommand(@"
                    INSERT INTO Chats (UserId, Message, Response, RawRequest, RawResponse, DateCreated)
                    VALUES (@UserId, @Message, @Response, @RawRequest, @RawResponse, @DateCreated);
                    SELECT last_insert_rowid();", connection))
                {
                    command.Parameters.AddWithValue("@UserId", entity.UserId.HasValue ? (object)entity.UserId.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@Message", entity.Message ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Response", entity.Response ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@RawRequest", entity.RawRequest ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@RawResponse", entity.RawResponse ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@DateCreated", DateTime.UtcNow.ToString("o"));

                    entity.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public void Update(ChatModel entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            using (var connection = DbManager.GetConnection())
            {
                using (var command = new SQLiteCommand(@"
                    UPDATE Chats 
                    SET UserId = @UserId,
                        Message = @Message,
                        Response = @Response,
                        RawRequest = @RawRequest,
                        RawResponse = @RawResponse
                    WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", entity.Id);
                    command.Parameters.AddWithValue("@UserId", entity.UserId.HasValue ? (object)entity.UserId.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@Message", entity.Message ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Response", entity.Response ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@RawRequest", entity.RawRequest ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@RawResponse", entity.RawResponse ?? (object)DBNull.Value);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(ChatModel entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            using (var connection = DbManager.GetConnection())
            {
                using (var command = new SQLiteCommand("DELETE FROM Chats WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", entity.Id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void SaveChanges()
        {
            // No implementation needed for SQLite
        }

        private ChatModel ReadChatFromReader(SQLiteDataReader reader)
        {
            var chat = new ChatModel
            {
                Id = Convert.ToInt32(reader["Id"]),
                UserId = reader["UserId"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["UserId"]),
                Message = reader["Message"] == DBNull.Value ? null : reader["Message"].ToString(),
                Response = reader["Response"] == DBNull.Value ? null : reader["Response"].ToString(),
                RawRequest = reader["RawRequest"] == DBNull.Value ? null : reader["RawRequest"].ToString(),
                RawResponse = reader["RawResponse"] == DBNull.Value ? null : reader["RawResponse"].ToString(),
                DateCreated = DateTime.Parse(reader["DateCreated"].ToString())
            };

            return chat;
        }
    }
}