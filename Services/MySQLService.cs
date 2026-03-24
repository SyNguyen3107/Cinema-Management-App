using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema_Management_App.Services
{
    //Đây là class duy nhất có quyền truy cập vào DB, các class khác muốn truy cập DB phải thông qua class này
    
    public class MySQLService
    {
        private readonly string _connectionString;
        public MySQLService()
        {
            // Lấy chuỗi kết nối từ App.config thông qua tên "AivenMySQL"
            _connectionString = ConfigurationManager.ConnectionStrings["AivenMySQL"].ConnectionString;
        }
        public DataTable ExecuteQuery(string query, MySqlParameter[] parameters = null)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        // Dùng cho INSERT, UPDATE, DELETE (Trả về số dòng bị ảnh hưởng)
        public int ExecuteNonQuery(string query, MySqlParameter[] parameters = null)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
