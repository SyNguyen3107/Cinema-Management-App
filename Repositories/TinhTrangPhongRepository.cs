using Cinema_Management_App.Models;
using Cinema_Management_App.Services;
using Cinema_Management_App.Interfaces;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Cinema_Management_App.Repositories
{
    public class TinhTrangPhongRepository : ITinhTrangPhongRepository
    {
        private readonly IDatabaseService _dbService;

        // Inject the abstract database service to enforce loose coupling
        public TinhTrangPhongRepository(IDatabaseService dbService)
        {
            _dbService = dbService;
        }

        public async Task<IEnumerable<TinhTrangPhong>> GetAllTinhTrangPhongAsync()
        {
            var tinhTrangPhongs = new List<TinhTrangPhong>();
            string query = "SELECT MaTinhTrangPhong, TenTinhTrangPhong FROM QuanLyRapPhim.TINHTRANGPHONG";

            DataTable dt = await _dbService.ExecuteQueryAsync(query);
            foreach (DataRow row in dt.Rows)
            {
                var tinhTrangPhong = new TinhTrangPhong
                {
                    MaTinhTrangPhong = row["MaTinhTrangPhong"].ToString() ?? string.Empty,
                    TenTinhTrangPhong = row["TenTinhTrangPhong"].ToString() ?? string.Empty
                };
                tinhTrangPhongs.Add(tinhTrangPhong);
            }
            return tinhTrangPhongs;
        }

        public async Task<TinhTrangPhong?> GetByIdAsync(string maTinhTrangPhong)
        {
            string query = "SELECT MaTinhTrangPhong, TenTinhTrangPhong FROM QuanLyRapPhim.TINHTRANGPHONG WHERE MaTinhTrangPhong = @mtp";
            DbParameter[] parameters = new DbParameter[]
            {
                new MySqlParameter("@mtp", maTinhTrangPhong)
            };

            DataTable dt = await _dbService.ExecuteQueryAsync(query, parameters);
            if (dt.Rows.Count == 0) return null;

            DataRow row = dt.Rows[0];
            return new TinhTrangPhong
            {
                MaTinhTrangPhong = row["MaTinhTrangPhong"].ToString() ?? string.Empty,
                TenTinhTrangPhong = row["TenTinhTrangPhong"].ToString() ?? string.Empty
            };
        }
    }
}