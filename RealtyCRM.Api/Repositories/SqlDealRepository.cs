using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using RealtyCRM.Api.Interfaces;
using RealtyCRM.Api.Models;

namespace RealtyCRM.Api.Repositories
{
    public class SqlDealRepository : IDealRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;

        public SqlDealRepository(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<Deal>> GetAllAsync()
        {
            var list = new List<Deal>();
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            var query = "SELECT id, property_id, client_id, realtor_id, deal_date, final_price FROM deals";
            using var cmd = new NpgsqlCommand(query, connection);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(MapDeal(reader));
            }

            return list;
        }

        public async Task<Deal?> GetByIdAsync(int id)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            var query = "SELECT id, property_id, client_id, realtor_id, deal_date, final_price FROM deals WHERE id = @Id";
            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapDeal(reader);
            }

            return null;
        }

        public async Task AddAsync(Deal entity)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            var query = @"
                INSERT INTO deals (property_id, client_id, realtor_id, deal_date, final_price)
                VALUES (@PropertyId, @ClientId, @RealtorId, @DealDate, @FinalPrice)
                RETURNING id;";

            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("PropertyId", entity.PropertyId);
            cmd.Parameters.AddWithValue("ClientId", entity.ClientId > 0 ? entity.ClientId : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("RealtorId", entity.RealtorId > 0 ? entity.RealtorId : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("DealDate", entity.DealDate);
            cmd.Parameters.AddWithValue("FinalPrice", entity.FinalPrice);

            var newId = await cmd.ExecuteScalarAsync();
            if (newId != null)
            {
                entity.Id = Convert.ToInt32(newId);
            }
        }

        public async Task UpdateAsync(Deal entity)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            var query = @"
                UPDATE deals 
                SET property_id = @PropertyId, 
                    client_id = @ClientId, 
                    realtor_id = @RealtorId, 
                    deal_date = @DealDate, 
                    final_price = @FinalPrice
                WHERE id = @Id;";

            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("PropertyId", entity.PropertyId);
            cmd.Parameters.AddWithValue("ClientId", entity.ClientId > 0 ? entity.ClientId : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("RealtorId", entity.RealtorId > 0 ? entity.RealtorId : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("DealDate", entity.DealDate);
            cmd.Parameters.AddWithValue("FinalPrice", entity.FinalPrice);
            cmd.Parameters.AddWithValue("Id", entity.Id);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            var query = "DELETE FROM deals WHERE id = @Id";
            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("Id", id);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<IEnumerable<Deal>> GetByRealtorIdAsync(int realtorId)
        {
            var list = new List<Deal>();
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            var query = "SELECT id, property_id, client_id, realtor_id, deal_date, final_price FROM deals WHERE realtor_id = @RealtorId ORDER BY deal_date DESC";
            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("RealtorId", realtorId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapDeal(reader));
            }

            return list;
        }

        private Deal MapDeal(NpgsqlDataReader reader)
        {
            return new Deal
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                PropertyId = reader.GetInt32(reader.GetOrdinal("property_id")),
                ClientId = reader.IsDBNull(reader.GetOrdinal("client_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("client_id")),
                RealtorId = reader.IsDBNull(reader.GetOrdinal("realtor_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("realtor_id")),
                DealDate = reader.GetDateTime(reader.GetOrdinal("deal_date")),
                FinalPrice = reader.GetDecimal(reader.GetOrdinal("final_price"))
            };
        }
    }
}
