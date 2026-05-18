using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace Cinema_Management_App.Services
{
    public class MySQLService
    {
        private readonly string _connectionString;

        public MySQLService()
        {
            // Ưu tiên lấy từ App.config (ConfigurationManager)
            var connStringFromConfig = System.Configuration.ConfigurationManager.ConnectionStrings["AivenMySQL"]?.ConnectionString;

            if (!string.IsNullOrEmpty(connStringFromConfig))
            {
                _connectionString = connStringFromConfig;
            }
            else
            {
                // Backup: lấy từ appsettings.json
                var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();

                _connectionString = config.GetConnectionString("AivenMySQL") ??
                    throw new InvalidOperationException("Connection string 'AivenMySQL' not found.");
            }
        }
        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        public DataTable ExecuteQuery(string query, MySqlParameter[] parameters = null)
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

        public int ExecuteNonQuery(string query, MySqlParameter[] parameters = null)
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

        public object ExecuteScalar(string query, MySqlParameter[] parameters = null)
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
    }
}