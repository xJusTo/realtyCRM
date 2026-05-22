using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace RealtyCRM.Api.Repositories
{
    /// <summary>
    /// Класс для инициализации базы данных PostgreSQL (создание таблиц и начальных данных).
    /// </summary>
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Строка подключения 'DefaultConnection' не найдена.");

            // 1. Проверяем и создаем саму базу данных
            await EnsureDatabaseCreatedAsync(connectionString);

            // 2. Создаем таблицы и заполняем демонстрационными данными
            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                // Таблица пользователей
                var createUsersTable = @"
                    CREATE TABLE IF NOT EXISTS users (
                        id SERIAL PRIMARY KEY,
                        full_name VARCHAR(255) NOT NULL,
                        phone VARCHAR(50) NOT NULL,
                        email VARCHAR(255) NOT NULL UNIQUE,
                        password_hash VARCHAR(255) NOT NULL,
                        role VARCHAR(50) NOT NULL,
                        preferences TEXT DEFAULT '',
                        commission_rate NUMERIC DEFAULT 0
                    );";

                // Таблица недвижимости
                var createPropertiesTable = @"
                    CREATE TABLE IF NOT EXISTS properties (
                        id SERIAL PRIMARY KEY,
                        address VARCHAR(255) NOT NULL,
                        description TEXT NOT NULL,
                        price NUMERIC NOT NULL,
                        area DOUBLE PRECISION NOT NULL,
                        type INTEGER NOT NULL,
                        status INTEGER NOT NULL,
                        realtor_id INTEGER REFERENCES users(id) ON DELETE SET NULL,
                        photo_url TEXT DEFAULT ''
                    );";

                // Таблица заявок
                var createBookingsTable = @"
                    CREATE TABLE IF NOT EXISTS bookings (
                        id SERIAL PRIMARY KEY,
                        property_id INTEGER NOT NULL REFERENCES properties(id) ON DELETE CASCADE,
                        client_name VARCHAR(255) NOT NULL,
                        client_phone VARCHAR(50) NOT NULL,
                        client_email VARCHAR(255) NOT NULL,
                        booking_date TIMESTAMP NOT NULL DEFAULT NOW(),
                        status VARCHAR(50) NOT NULL DEFAULT 'Pending'
                    );";

                // Таблица сделок
                var createDealsTable = @"
                    CREATE TABLE IF NOT EXISTS deals (
                        id SERIAL PRIMARY KEY,
                        property_id INTEGER NOT NULL REFERENCES properties(id) ON DELETE CASCADE,
                        client_id INTEGER REFERENCES users(id) ON DELETE SET NULL,
                        realtor_id INTEGER REFERENCES users(id) ON DELETE SET NULL,
                        deal_date TIMESTAMP NOT NULL DEFAULT NOW(),
                        final_price NUMERIC NOT NULL
                    );";

                using (var cmd = new NpgsqlCommand(createUsersTable, connection, transaction))
                    await cmd.ExecuteNonQueryAsync();

                using (var cmd = new NpgsqlCommand(createPropertiesTable, connection, transaction))
                    await cmd.ExecuteNonQueryAsync();

                using (var cmd = new NpgsqlCommand(createBookingsTable, connection, transaction))
                    await cmd.ExecuteNonQueryAsync();

                using (var cmd = new NpgsqlCommand(createDealsTable, connection, transaction))
                    await cmd.ExecuteNonQueryAsync();

                // Сид начальных данных (пользователи)
                var countCmd = new NpgsqlCommand("SELECT COUNT(*) FROM users", connection, transaction);
                var userCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

                if (userCount == 0)
                {
                    // Создаем риэлтора и клиента по умолчанию
                    var insertUser = @"
                        INSERT INTO users (full_name, phone, email, password_hash, role, preferences, commission_rate)
                        VALUES (@FullName, @Phone, @Email, @PasswordHash, @Role, @Preferences, @CommissionRate)
                        RETURNING id;";

                    // Риэлтор
                    int realtorId;
                    using (var cmd = new NpgsqlCommand(insertUser, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("FullName", "Дмитрий Смирнов");
                        cmd.Parameters.AddWithValue("Phone", "+79991112233");
                        cmd.Parameters.AddWithValue("Email", "realtor@realty.com");
                        cmd.Parameters.AddWithValue("PasswordHash", HashPassword("realtor123"));
                        cmd.Parameters.AddWithValue("Role", "Realtor");
                        cmd.Parameters.AddWithValue("Preferences", DBNull.Value);
                        cmd.Parameters.AddWithValue("CommissionRate", 2.5m);
                        realtorId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                    }

                    // Клиент
                    using (var cmd = new NpgsqlCommand(insertUser, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("FullName", "Иван Иванов");
                        cmd.Parameters.AddWithValue("Phone", "+79994445566");
                        cmd.Parameters.AddWithValue("Email", "client@realty.com");
                        cmd.Parameters.AddWithValue("PasswordHash", HashPassword("client123"));
                        cmd.Parameters.AddWithValue("Role", "Client");
                        cmd.Parameters.AddWithValue("Preferences", "Ищет 2-комнатную квартиру в центре");
                        cmd.Parameters.AddWithValue("CommissionRate", DBNull.Value);
                        await cmd.ExecuteNonQueryAsync();
                    }

                    // Сид начальных данных (недвижимость)
                    var insertProperty = @"
                        INSERT INTO properties (address, description, price, area, type, status, realtor_id, photo_url)
                        VALUES (@Address, @Description, @Price, @Area, @Type, @Status, @RealtorId, @PhotoUrl);";

                    using (var cmd = new NpgsqlCommand(insertProperty, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("Address", "ул. Ленина, д. 45, кв. 12");
                        cmd.Parameters.AddWithValue("Description", "Светлая 2-комнатная квартира с евроремонтом. Полностью меблирована.");
                        cmd.Parameters.AddWithValue("Price", 6200000m);
                        cmd.Parameters.AddWithValue("Area", 54.5);
                        cmd.Parameters.AddWithValue("Type", 0); // Apartment
                        cmd.Parameters.AddWithValue("Status", 0); // Available
                        cmd.Parameters.AddWithValue("RealtorId", realtorId);
                        cmd.Parameters.AddWithValue("PhotoUrl", "https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?auto=format&fit=crop&w=800&q=80");
                        await cmd.ExecuteNonQueryAsync();
                    }

                    using (var cmd = new NpgsqlCommand(insertProperty, connection, transaction))
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("Address", "п. Южный, ул. Садовая, д. 8");
                        cmd.Parameters.AddWithValue("Description", "Просторный двухэтажный коттедж с участком 10 соток. Есть гараж и баня.");
                        cmd.Parameters.AddWithValue("Price", 12500000m);
                        cmd.Parameters.AddWithValue("Area", 180.0);
                        cmd.Parameters.AddWithValue("Type", 1); // House
                        cmd.Parameters.AddWithValue("Status", 0); // Available
                        cmd.Parameters.AddWithValue("RealtorId", realtorId);
                        cmd.Parameters.AddWithValue("PhotoUrl", "https://images.unsplash.com/photo-1580587771525-78b9dba3b914?auto=format&fit=crop&w=800&q=80");
                        await cmd.ExecuteNonQueryAsync();
                    }

                    using (var cmd = new NpgsqlCommand(insertProperty, connection, transaction))
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("Address", "ул. Красная, д. 110, офис 4");
                        cmd.Parameters.AddWithValue("Description", "Офисное помещение на первом этаже коммерческого здания. Высокий пешеходный трафик.");
                        cmd.Parameters.AddWithValue("Price", 8900000m);
                        cmd.Parameters.AddWithValue("Area", 75.0);
                        cmd.Parameters.AddWithValue("Type", 2); // Commercial
                        cmd.Parameters.AddWithValue("Status", 0); // Available
                        cmd.Parameters.AddWithValue("RealtorId", realtorId);
                        cmd.Parameters.AddWithValue("PhotoUrl", "https://images.unsplash.com/photo-1497366216548-37526070297c?auto=format&fit=crop&w=800&q=80");
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Ошибка инициализации БД: {ex.Message}");
                throw;
            }
        }

        private static async Task EnsureDatabaseCreatedAsync(string connectionString)
        {
            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            var targetDb = builder.Database;

            if (string.IsNullOrEmpty(targetDb)) return;

            // Подключаемся к системной базе postgres
            builder.Database = "postgres";
            var checkString = builder.ConnectionString;

            try
            {
                using var conn = new NpgsqlConnection(checkString);
                await conn.OpenAsync();

                var checkCmd = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname = '{targetDb}'", conn);
                var exists = await checkCmd.ExecuteScalarAsync();

                if (exists == null)
                {
                    var createCmd = new NpgsqlCommand($"CREATE DATABASE \"{targetDb}\"", conn);
                    await createCmd.ExecuteNonQueryAsync();
                    Console.WriteLine($"База данных '{targetDb}' успешно создана.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось проверить/создать БД '{targetDb}': {ex.Message}");
                // Игнорируем ошибку, если у пользователя нет прав на создание БД, 
                // так как БД может быть уже создана вручную.
            }
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToHexString(hash).ToLower();
        }
    }
}
