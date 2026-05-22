using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using RealtyCRM.Api.Interfaces;
using RealtyCRM.Api.Models;

namespace RealtyCRM.Api.Repositories
{
    public class SqlUserRepository : IUserRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;

        public SqlUserRepository(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var list = new List<User>();
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            var query = "SELECT id, full_name, phone, email, password_hash, role, preferences, commission_rate FROM users";
            using var cmd = new NpgsqlCommand(query, connection);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(MapUser(reader));
            }

            return list;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            var query = "SELECT id, full_name, phone, email, password_hash, role, preferences, commission_rate FROM users WHERE id = @Id";
            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapUser(reader);
            }

            return null;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            var query = "SELECT id, full_name, phone, email, password_hash, role, preferences, commission_rate FROM users WHERE LOWER(email) = LOWER(@Email)";
            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("Email", email);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapUser(reader);
            }

            return null;
        }

        public async Task AddAsync(User entity)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            var query = @"
                INSERT INTO users (full_name, phone, email, password_hash, role, preferences, commission_rate)
                VALUES (@FullName, @Phone, @Email, @PasswordHash, @Role, @Preferences, @CommissionRate)
                RETURNING id;";

            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("FullName", entity.FullName);
            cmd.Parameters.AddWithValue("Phone", entity.Phone);
            cmd.Parameters.AddWithValue("Email", entity.Email);
            cmd.Parameters.AddWithValue("PasswordHash", entity.PasswordHash);
            cmd.Parameters.AddWithValue("Role", entity.Role);
            cmd.Parameters.AddWithValue("Preferences", string.IsNullOrEmpty(entity.Preferences) ? (object)DBNull.Value : entity.Preferences);
            cmd.Parameters.AddWithValue("CommissionRate", entity.Role == "Realtor" ? entity.CommissionRate : (object)DBNull.Value);

            var newId = await cmd.ExecuteScalarAsync();
            if (newId != null)
            {
                entity.Id = Convert.ToInt32(newId);
            }
        }

        public async Task UpdateAsync(User entity)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            var query = @"
                UPDATE users 
                SET full_name = @FullName, 
                    phone = @Phone, 
                    email = @Email, 
                    password_hash = @PasswordHash, 
                    role = @Role, 
                    preferences = @Preferences, 
                    commission_rate = @CommissionRate
                WHERE id = @Id;";

            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("FullName", entity.FullName);
            cmd.Parameters.AddWithValue("Phone", entity.Phone);
            cmd.Parameters.AddWithValue("Email", entity.Email);
            cmd.Parameters.AddWithValue("PasswordHash", entity.PasswordHash);
            cmd.Parameters.AddWithValue("Role", entity.Role);
            cmd.Parameters.AddWithValue("Preferences", string.IsNullOrEmpty(entity.Preferences) ? (object)DBNull.Value : entity.Preferences);
            cmd.Parameters.AddWithValue("CommissionRate", entity.Role == "Realtor" ? entity.CommissionRate : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("Id", entity.Id);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            var query = "DELETE FROM users WHERE id = @Id";
            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("Id", id);

            await cmd.ExecuteNonQueryAsync();
        }

        private User MapUser(NpgsqlDataReader reader)
        {
            return new User
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                FullName = reader.GetString(reader.GetOrdinal("full_name")),
                Phone = reader.GetString(reader.GetOrdinal("phone")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                PasswordHash = reader.GetString(reader.GetOrdinal("password_hash")),
                Role = reader.GetString(reader.GetOrdinal("role")),
                Preferences = reader.IsDBNull(reader.GetOrdinal("preferences")) ? string.Empty : reader.GetString(reader.GetOrdinal("preferences")),
                CommissionRate = reader.IsDBNull(reader.GetOrdinal("commission_rate")) ? 0 : reader.GetDecimal(reader.GetOrdinal("commission_rate"))
            };
        }
    }
}
