using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using RealtyCRM.Api.Interfaces;
using RealtyCRM.Api.Models;

namespace RealtyCRM.Api.Repositories
{
    public class SqlPropertyRepository : IPropertyRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;

        public SqlPropertyRepository(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<Property>> GetAllAsync()
        {
            var list = new List<Property>();
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            var query = "SELECT id, address, description, price, area, type, status, realtor_id, photo_url FROM properties";
            using var cmd = new NpgsqlCommand(query, connection);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(MapProperty(reader));
            }

            return list;
        }

        public async Task<Property?> GetByIdAsync(int id)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            var query = "SELECT id, address, description, price, area, type, status, realtor_id, photo_url FROM properties WHERE id = @Id";
            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapProperty(reader);
            }

            return null;
        }

        public async Task AddAsync(Property entity)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            var query = @"
                INSERT INTO properties (address, description, price, area, type, status, realtor_id, photo_url)
                VALUES (@Address, @Description, @Price, @Area, @Type, @Status, @RealtorId, @PhotoUrl)
                RETURNING id;";

            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("Address", entity.Address);
            cmd.Parameters.AddWithValue("Description", entity.Description);
            cmd.Parameters.AddWithValue("Price", entity.Price);
            cmd.Parameters.AddWithValue("Area", entity.Area);
            cmd.Parameters.AddWithValue("Type", (int)entity.Type);
            cmd.Parameters.AddWithValue("Status", (int)entity.Status);
            cmd.Parameters.AddWithValue("RealtorId", entity.RealtorId > 0 ? entity.RealtorId : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("PhotoUrl", string.IsNullOrEmpty(entity.PhotoUrl) ? (object)DBNull.Value : entity.PhotoUrl);

            var newId = await cmd.ExecuteScalarAsync();
            if (newId != null)
            {
                entity.Id = Convert.ToInt32(newId);
            }
        }

        public async Task UpdateAsync(Property entity)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            var query = @"
                UPDATE properties 
                SET address = @Address, 
                    description = @Description, 
                    price = @Price, 
                    area = @Area, 
                    type = @Type, 
                    status = @Status, 
                    realtor_id = @RealtorId, 
                    photo_url = @PhotoUrl
                WHERE id = @Id;";

            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("Address", entity.Address);
            cmd.Parameters.AddWithValue("Description", entity.Description);
            cmd.Parameters.AddWithValue("Price", entity.Price);
            cmd.Parameters.AddWithValue("Area", entity.Area);
            cmd.Parameters.AddWithValue("Type", (int)entity.Type);
            cmd.Parameters.AddWithValue("Status", (int)entity.Status);
            cmd.Parameters.AddWithValue("RealtorId", entity.RealtorId > 0 ? entity.RealtorId : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("PhotoUrl", string.IsNullOrEmpty(entity.PhotoUrl) ? (object)DBNull.Value : entity.PhotoUrl);
            cmd.Parameters.AddWithValue("Id", entity.Id);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            var query = "DELETE FROM properties WHERE id = @Id";
            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("Id", id);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<IEnumerable<Property>> GetFilteredAsync(
            PropertyType? type = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            double? minArea = null,
            double? maxArea = null,
            PropertyStatus? status = null,
            int? realtorId = null)
        {
            var list = new List<Property>();
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sqlBuilder = new StringBuilder("SELECT id, address, description, price, area, type, status, realtor_id, photo_url FROM properties WHERE 1=1");
            var cmd = new NpgsqlCommand();

            if (type.HasValue)
            {
                sqlBuilder.Append(" AND type = @Type");
                cmd.Parameters.AddWithValue("Type", (int)type.Value);
            }

            if (minPrice.HasValue)
            {
                sqlBuilder.Append(" AND price >= @MinPrice");
                cmd.Parameters.AddWithValue("MinPrice", minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                sqlBuilder.Append(" AND price <= @MaxPrice");
                cmd.Parameters.AddWithValue("MaxPrice", maxPrice.Value);
            }

            if (minArea.HasValue)
            {
                sqlBuilder.Append(" AND area >= @MinArea");
                cmd.Parameters.AddWithValue("MinArea", minArea.Value);
            }

            if (maxArea.HasValue)
            {
                sqlBuilder.Append(" AND area <= @MaxArea");
                cmd.Parameters.AddWithValue("MaxArea", maxArea.Value);
            }

            if (status.HasValue)
            {
                sqlBuilder.Append(" AND status = @Status");
                cmd.Parameters.AddWithValue("Status", (int)status.Value);
            }

            if (realtorId.HasValue)
            {
                sqlBuilder.Append(" AND realtor_id = @RealtorId");
                cmd.Parameters.AddWithValue("RealtorId", realtorId.Value);
            }

            cmd.CommandText = sqlBuilder.ToString();
            cmd.Connection = connection;

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapProperty(reader));
            }

            return list;
        }

        private Property MapProperty(NpgsqlDataReader reader)
        {
            return new Property
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Address = reader.GetString(reader.GetOrdinal("address")),
                Description = reader.GetString(reader.GetOrdinal("description")),
                Price = reader.GetDecimal(reader.GetOrdinal("price")),
                Area = reader.GetDouble(reader.GetOrdinal("area")),
                Type = (PropertyType)reader.GetInt32(reader.GetOrdinal("type")),
                Status = (PropertyStatus)reader.GetInt32(reader.GetOrdinal("status")),
                RealtorId = reader.IsDBNull(reader.GetOrdinal("realtor_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("realtor_id")),
                PhotoUrl = reader.IsDBNull(reader.GetOrdinal("photo_url")) ? string.Empty : reader.GetString(reader.GetOrdinal("photo_url"))
            };
        }
    }
}
