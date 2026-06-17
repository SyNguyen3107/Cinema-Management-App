using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Cinema_Management_App.Services
{
    public class MySQLService : IDatabaseService
    {
        private readonly string _connectionString;

        // Inject IConfiguration via constructor (Dependency Injection)
        public MySQLService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AivenMySQL") ??
                throw new InvalidOperationException("Connection string 'AivenMySQL' not found in configuration.");
        }

        public DbConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        #region Synchronous Methods

        public DataTable ExecuteQuery(string query, DbParameter[] parameters = null)
        {
            using (MySqlConnection conn = GetConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public int ExecuteNonQuery(string query, DbParameter[] parameters = null)
        {
            using (MySqlConnection conn = GetConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public object ExecuteScalar(string query, DbParameter[] parameters = null)
        {
            using (MySqlConnection conn = GetConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    conn.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }

        #endregion

        #region Asynchronous Methods

        public async Task<DataTable> ExecuteQueryAsync(string query, DbParameter[] parameters = null)
        {
            using (MySqlConnection conn = GetConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);

                    await conn.OpenAsync();

                    // MySqlDataAdapter doesn't have an async Fill method.
                    // Instead, we use ExecuteReaderAsync and load the data into the DataTable.
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        return dt;
                    }
                }
            }
        }

        public async Task<int> ExecuteNonQueryAsync(string query, DbParameter[] parameters = null)
        {
            using (MySqlConnection conn = GetConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    await conn.OpenAsync();
                    return await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<object> ExecuteScalarAsync(string query, DbParameter[] parameters = null)
        {
            using (MySqlConnection conn = GetConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    await conn.OpenAsync();
                    return await cmd.ExecuteScalarAsync();
                }
            }
        }

        #endregion
    }
}