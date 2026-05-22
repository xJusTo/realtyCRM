using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using RealtyCRM.Api.Interfaces;
using RealtyCRM.Api.Models;

namespace RealtyCRM.Api.Repositories
{
    public class SqlBookingRepository : IBookingRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;

        public SqlBookingRepository(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            var list = new List<Booking>();
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            var query = "SELECT id, property_id, client_name, client_phone, client_email, booking_date, status FROM bookings";
            using var cmd = new NpgsqlCommand(query, connection);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(MapBooking(reader));
            }

            return list;
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            var query = "SELECT id, property_id, client_name, client_phone, client_email, booking_date, status FROM bookings WHERE id = @Id";
            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapBooking(reader);
            }

            return null;
        }

        public async Task AddAsync(Booking entity)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            var query = @"
                INSERT INTO bookings (property_id, client_name, client_phone, client_email, booking_date, status)
                VALUES (@PropertyId, @ClientName, @ClientPhone, @ClientEmail, @BookingDate, @Status)
                RETURNING id;";

            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("PropertyId", entity.PropertyId);
            cmd.Parameters.AddWithValue("ClientName", entity.ClientName);
            cmd.Parameters.AddWithValue("ClientPhone", entity.ClientPhone);
            cmd.Parameters.AddWithValue("ClientEmail", entity.ClientEmail);
            cmd.Parameters.AddWithValue("BookingDate", entity.BookingDate);
            cmd.Parameters.AddWithValue("Status", entity.Status);

            var newId = await cmd.ExecuteScalarAsync();
            if (newId != null)
            {
                entity.Id = Convert.ToInt32(newId);
            }
        }

        public async Task UpdateAsync(Booking entity)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            var query = @"
                UPDATE bookings 
                SET property_id = @PropertyId, 
                    client_name = @ClientName, 
                    client_phone = @ClientPhone, 
                    client_email = @ClientEmail, 
                    booking_date = @BookingDate, 
                    status = @Status
                WHERE id = @Id;";

            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("PropertyId", entity.PropertyId);
            cmd.Parameters.AddWithValue("ClientName", entity.ClientName);
            cmd.Parameters.AddWithValue("ClientPhone", entity.ClientPhone);
            cmd.Parameters.AddWithValue("ClientEmail", entity.ClientEmail);
            cmd.Parameters.AddWithValue("BookingDate", entity.BookingDate);
            cmd.Parameters.AddWithValue("Status", entity.Status);
            cmd.Parameters.AddWithValue("Id", entity.Id);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            var query = "DELETE FROM bookings WHERE id = @Id";
            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("Id", id);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<IEnumerable<Booking>> GetByRealtorIdAsync(int realtorId)
        {
            var list = new List<Booking>();
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            // Соединяем с таблицей недвижимости, чтобы найти те, где ответственный риэлтор = realtorId
            var query = @"
                SELECT b.id, b.property_id, b.client_name, b.client_phone, b.client_email, b.booking_date, b.status 
                FROM bookings b
                INNER JOIN properties p ON b.property_id = p.id
                WHERE p.realtor_id = @RealtorId
                ORDER BY b.booking_date DESC;";

            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("RealtorId", realtorId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapBooking(reader));
            }

            return list;
        }

        public async Task<IEnumerable<Booking>> GetByClientEmailAsync(string clientEmail)
        {
            var list = new List<Booking>();
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            var query = @"
                SELECT id, property_id, client_name, client_phone, client_email, booking_date, status 
                FROM bookings 
                WHERE LOWER(client_email) = LOWER(@ClientEmail)
                ORDER BY booking_date DESC;";

            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("ClientEmail", clientEmail);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapBooking(reader));
            }

            return list;
        }

        private Booking MapBooking(NpgsqlDataReader reader)
        {
            return new Booking
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                PropertyId = reader.GetInt32(reader.GetOrdinal("property_id")),
                ClientName = reader.GetString(reader.GetOrdinal("client_name")),
                ClientPhone = reader.GetString(reader.GetOrdinal("client_phone")),
                ClientEmail = reader.GetString(reader.GetOrdinal("client_email")),
                BookingDate = reader.GetDateTime(reader.GetOrdinal("booking_date")),
                Status = reader.GetString(reader.GetOrdinal("status"))
            };
        }
    }
}
