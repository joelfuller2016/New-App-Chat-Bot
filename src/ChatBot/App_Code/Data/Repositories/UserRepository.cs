using System;
using System.Collections.Generic;
using System.Data.SQLite;
using ChatBot.Data.Models;
using System.Linq;
using ChatBot.Services.Security;

namespace ChatBot.Data.Repositories
{
    /// <summary>
    /// Repository for user accounts
    /// </summary>
    public class UserRepository : IRepository<UserModel>
    {
        private readonly ISecurityService _securityService;

        public UserRepository(ISecurityService securityService)
        {
            _securityService = securityService ?? throw new ArgumentNullException(nameof(securityService));
        }

        public UserModel GetById(int id)
        {
            using (var connection = DbManager.GetConnection())
            {
                using (var command = new SQLiteCommand("SELECT * FROM Users WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return ReadUserFromReader(reader);
                        }
                    }
                }
            }
            return null;
        }

        public UserModel GetByUsername(string username)
        {
            using (var connection = DbManager.GetConnection())
            {
                using (var command = new SQLiteCommand("SELECT * FROM Users WHERE Username = @Username", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return ReadUserFromReader(reader);
                        }
                    }
                }
            }
            return null;
        }

        public IEnumerable<UserModel> GetAll()
        {
            var users = new List<UserModel>();
            using (var connection = DbManager.GetConnection())
            {
                using (var command = new SQLiteCommand("SELECT * FROM Users", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(ReadUserFromReader(reader));
                        }
                    }
                }
            }
            return users;
        }

        public IEnumerable<UserModel> Find(Func<UserModel, bool> predicate)
        {
            return GetAll().Where(predicate);
        }

        public void Add(UserModel entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            // Hash password if provided
            string salt = string.Empty;
            if (!string.IsNullOrEmpty(entity.PasswordHash))
            {
                entity.PasswordHash = _securityService.HashPassword(entity.PasswordHash, out salt);
                entity.PasswordSalt = salt;
            }

            using (var connection = DbManager.GetConnection())
            {
                using (var command = new SQLiteCommand(@"
                    INSERT INTO Users (Username, PasswordHash, PasswordSalt, Email, IsAdmin, DateCreated, DateModified)
                    VALUES (@Username, @PasswordHash, @PasswordSalt, @Email, @IsAdmin, @DateCreated, @DateModified);
                    SELECT last_insert_rowid();", connection))
                {
                    command.Parameters.AddWithValue("@Username", entity.Username);
                    command.Parameters.AddWithValue("@PasswordHash", entity.PasswordHash ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@PasswordSalt", entity.PasswordSalt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Email", entity.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsAdmin", entity.IsAdmin ? 1 : 0);
                    command.Parameters.AddWithValue("@DateCreated", DateTime.UtcNow.ToString("o"));
                    command.Parameters.AddWithValue("@DateModified", DateTime.UtcNow.ToString("o"));

                    entity.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public void Update(UserModel entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            using (var connection = DbManager.GetConnection())
            {
                using (var command = new SQLiteCommand(@"
                    UPDATE Users 
                    SET Username = @Username,
                        PasswordHash = @PasswordHash,
                        PasswordSalt = @PasswordSalt,
                        Email = @Email,
                        IsAdmin = @IsAdmin,
                        DateModified = @DateModified
                    WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", entity.Id);
                    command.Parameters.AddWithValue("@Username", entity.Username);
                    command.Parameters.AddWithValue("@PasswordHash", entity.PasswordHash ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@PasswordSalt", entity.PasswordSalt ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Email", entity.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsAdmin", entity.IsAdmin ? 1 : 0);
                    command.Parameters.AddWithValue("@DateModified", DateTime.UtcNow.ToString("o"));

                    command.ExecuteNonQuery();
                }
            }
        }

        public bool VerifyPassword(string username, string password)
        {
            var user = GetByUsername(username);
            if (user == null || string.IsNullOrEmpty(user.PasswordHash) || string.IsNullOrEmpty(user.PasswordSalt))
            {
                return false;
            }

            return _securityService.VerifyPassword(password, user.PasswordHash, user.PasswordSalt);
        }

        public void Delete(UserModel entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            using (var connection = DbManager.GetConnection())
            {
                using (var command = new SQLiteCommand("DELETE FROM Users WHERE Id = @Id", connection))
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

        private UserModel ReadUserFromReader(SQLiteDataReader reader)
        {
            var user = new UserModel
            {
                Id = Convert.ToInt32(reader["Id"]),
                Username = reader["Username"].ToString(),
                PasswordHash = reader["PasswordHash"] == DBNull.Value ? null : reader["PasswordHash"].ToString(),
                PasswordSalt = reader["PasswordSalt"] == DBNull.Value ? null : reader["PasswordSalt"].ToString(),
                Email = reader["Email"] == DBNull.Value ? null : reader["Email"].ToString(),
                IsAdmin = Convert.ToBoolean(reader["IsAdmin"]),
                DateCreated = DateTime.Parse(reader["DateCreated"].ToString()),
                DateModified = DateTime.Parse(reader["DateModified"].ToString())
            };

            return user;
        }
    }
}