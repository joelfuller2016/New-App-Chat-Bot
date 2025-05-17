using System;
using System.Collections.Generic;
using System.Data.SQLite;
using ChatBot.Data.Models;
using System.Linq;

namespace ChatBot.Data.Repositories
{
    /// <summary>
    /// Repository for error logs
    /// </summary>
    public class ErrorRepository : IRepository<ErrorModel>
    {
        public ErrorRepository()
        {
        }

        public ErrorModel GetById(int id)
        {
            using (var connection = DbManager.GetConnection())
            {
                using (var command = new SQLiteCommand("SELECT * FROM Errors WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return ReadErrorFromReader(reader);
                        }
                    }
                }
            }
            return null;
        }

        public IEnumerable<ErrorModel> GetAll()
        {
            var errors = new List<ErrorModel>();
            using (var connection = DbManager.GetConnection())
            {
                using (var command = new SQLiteCommand("SELECT * FROM Errors ORDER BY DateCreated DESC", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            errors.Add(ReadErrorFromReader(reader));
                        }
                    }
                }
            }
            return errors;
        }

        public IEnumerable<ErrorModel> Find(Func<ErrorModel, bool> predicate)
        {
            return GetAll().Where(predicate);
        }

        public void Add(ErrorModel entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            using (var connection = DbManager.GetConnection())
            {
                using (var command = new SQLiteCommand(@"
                    INSERT INTO Errors (Message, StackTrace, Source, Url, UserId, DateCreated)
                    VALUES (@Message, @StackTrace, @Source, @Url, @UserId, @DateCreated);
                    SELECT last_insert_rowid();", connection))
                {
                    command.Parameters.AddWithValue("@Message", entity.Message);
                    command.Parameters.AddWithValue("@StackTrace", entity.StackTrace ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Source", entity.Source ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Url", entity.Url ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UserId", entity.UserId.HasValue ? (object)entity.UserId.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@DateCreated", DateTime.UtcNow.ToString("o"));

                    entity.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public void Update(ErrorModel entity)
        {
            // Error logs are generally immutable
            throw new NotImplementedException("Error logs cannot be updated");
        }

        public void Delete(ErrorModel entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            using (var connection = DbManager.GetConnection())
            {
                using (var command = new SQLiteCommand("DELETE FROM Errors WHERE Id = @Id", connection))
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

        private ErrorModel ReadErrorFromReader(SQLiteDataReader reader)
        {
            var error = new ErrorModel
            {
                Id = Convert.ToInt32(reader["Id"]),
                Message = reader["Message"] == DBNull.Value ? null : reader["Message"].ToString(),
                StackTrace = reader["StackTrace"] == DBNull.Value ? null : reader["StackTrace"].ToString(),
                Source = reader["Source"] == DBNull.Value ? null : reader["Source"].ToString(),
                Url = reader["Url"] == DBNull.Value ? null : reader["Url"].ToString(),
                UserId = reader["UserId"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["UserId"]),
                DateCreated = DateTime.Parse(reader["DateCreated"].ToString())
            };

            return error;
        }
    }
}