using Cinema_Management_App.Models;
using Cinema_Management_App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema_Management_App.Repositories
{
    public class GheRepository
    {
        private readonly MySQLService _dbService;
        public GheRepository(MySQLService dbService)
        {
            _dbService = dbService;
        }
        public bool AddGhe(Ghe ghe)
        {
            string query = @"INSERT INTO QuanLyPhongChieu.GHE (MaGhe, MaSoGhe, MaPhong, MaLoaiGhe) 
                             VALUES (@mg, @ms, @mp, @mlg);";
            var parameters = new MySql.Data.MySqlClient.MySqlParameter[]
            {
                new MySql.Data.MySqlClient.MySqlParameter("@mg", ghe.MaGhe),
                new MySql.Data.MySqlClient.MySqlParameter("@ms", ghe.MaSoGhe),
                new MySql.Data.MySqlClient.MySqlParameter("@mp", ghe.MaPhong),
                new MySql.Data.MySqlClient.MySqlParameter("@mlg", ghe.MaLoaiGhe)
            };
            return _dbService.ExecuteNonQuery(query, parameters) > 0;
        }
    }
}
