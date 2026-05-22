using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace RealtyCRM.Api.Repositories
{
    /// <summary>
    /// Фабрика для создания подключений к базе данных PostgreSQL.
    /// </summary>
    public class DbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new System.InvalidOperationException("Строка подключения 'DefaultConnection' не найдена.");
        }

        /// <summary>
        /// Создает новое подключение к PostgreSQL.
        /// </summary>
        public NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}
