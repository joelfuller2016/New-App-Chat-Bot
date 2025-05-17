using System;
using System.Collections.Generic;
using System.Data.SQLite;
using ChatBot.Data.Models;
using System.Linq;
using ChatBot.Services.Security;

namespace ChatBot.Data.Repositories
{
    /// <summary>
    /// Repository for configuration settings
    /// </summary>
    public class ConfigRepository : IRepository<ConfigModel>
    {
        private readonly ISecurityService _securityService;

        public ConfigRepository(ISecurityService securityService)
        {
            _securityService = securityService ?? throw new ArgumentNullException(nameof(securityService));
        }

        public ConfigModel GetById(int id)
        {
            using (var connection = DbManager.GetConnection())
            {
                using (var command = new SQLiteCommand("SELECT * FROM Config WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return ReadConfigFromReader(reader);
                        }
                    }
                }
            }
            return null;
        }

        public ConfigModel GetByKey(string key)
        {
            using (var connection = DbManager.GetConnection())
            {
                using (var command = new SQLiteCommand("SELECT * FROM Config WHERE Key = @Key", connection))
                {
                    command.Parameters.AddWithValue("@Key", key);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return ReadConfigFromReader(reader);
                        }
                    }
                }
            }
            return null;
        }

        public IEnumerable<ConfigModel> GetAll()
        {
            var configs = new List<ConfigModel>();
            using (var connection = DbManager.GetConnection())
            {
                using (var command = new SQLiteCommand("SELECT * FROM Config", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            configs.Add(ReadConfigFromReader(reader));
                        }
                    }
                }
            }
            return configs;
        }

        public IEnumerable<ConfigModel> Find(Func<ConfigModel, bool> predicate)
        {
            return GetAll().Where(predicate);
        }

        public void Add(ConfigModel entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            // Encrypt value if necessary
            string valueToStore = entity.Value;
            if (entity.Encrypted && !string.IsNullOrEmpty(valueToStore))
            {
                valueToStore = _securityService.Encrypt(valueToStore);
            }

            using (var connection = DbManager.GetConnection())
            {
                using (var command = new SQLiteCommand(@"
                    INSERT INTO Config (Key, Value, Encrypted, DateCreated, DateModified)
                    VALUES (@Key, @Value, @Encrypted, @DateCreated, @DateModified);
                    SELECT last_insert_rowid();", connection))
                {
                    command.Parameters.AddWithValue("@Key", entity.Key);
                    command.Parameters.AddWithValue("@Value", valueToStore);
                    command.Parameters.AddWithValue("@Encrypted", entity.Encrypted ? 1 : 0);
                    command.Parameters.AddWithValue("@DateCreated", DateTime.UtcNow.ToString("o"));
                    command.Parameters.AddWithValue("@DateModified", DateTime.UtcNow.ToString("o"));

                    entity.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public void Update(ConfigModel entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            // Encrypt value if necessary
            string valueToStore = entity.Value;
            if (entity.Encrypted && !string.IsNullOrEmpty(valueToStore))
            {
                valueToStore = _securityService.Encrypt(valueToStore);
            }

            using (var connection = DbManager.GetConnection())
            {
                using (var command = new SQLiteCommand(@"
                    UPDATE Config 
                    SET Value = @Value, 
                        Encrypted = @Encrypted, 
                        DateModified = @DateModified
                    WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", entity.Id);
                    command.Parameters.AddWithValue("@Value", valueToStore);
                    command.Parameters.AddWithValue("@Encrypted", entity.Encrypted ? 1 : 0);
                    command.Parameters.AddWithValue("@DateModified", DateTime.UtcNow.ToString("o"));

                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(ConfigModel entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            using (var connection = DbManager.GetConnection())
            {
                using (var command = new SQLiteCommand("DELETE FROM Config WHERE Id = @Id", connection))
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

        private ConfigModel ReadConfigFromReader(SQLiteDataReader reader)
        {
            var config = new ConfigModel
            {
                Id = Convert.ToInt32(reader["Id"]),
                Key = reader["Key"].ToString(),
                Value = reader["Value"].ToString(),
                Encrypted = Convert.ToBoolean(reader["Encrypted"]),
                DateCreated = DateTime.Parse(reader["DateCreated"].ToString()),
                DateModified = DateTime.Parse(reader["DateModified"].ToString())
            };

            // Decrypt value if encrypted
            if (config.Encrypted && !string.IsNullOrEmpty(config.Value))
            {
                config.Value = _securityService.Decrypt(config.Value);
            }

            return config;
        }
    }
}