using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Cinema_Management_App.Models;
using Cinema_Management_App.Services;
using Cinema_Management_App.Interfaces;

namespace Cinema_Management_App.Repositories
{
    public class LoaiPhongRepository : ILoaiPhongRepository
    {
        private readonly IDatabaseService _dbService;

        // Inject the abstract database service to ensure loose coupling
        public LoaiPhongRepository(IDatabaseService dbService)
        {
            _dbService = dbService;
        }

        public async Task<IEnumerable<LoaiPhong>> GetAllLoaiPhongAsync()
        {
            var danhSach = new List<LoaiPhong>();
            string query = "SELECT * FROM QuanLyPhongChieu.LOAIPHONG";

            DataTable dt = await _dbService.ExecuteQueryAsync(query);
            foreach (DataRow row in dt.Rows)
            {
                danhSach.Add(new LoaiPhong
                {
                    MaLoaiPhong = row["MaLoaiPhong"].ToString() ?? string.Empty,
                    TenLoaiPhong = row["TenLoaiPhong"].ToString() ?? string.Empty
                });
            }
            return danhSach;
        }

        public async Task<LoaiPhong?> GetByIdAsync(string maLoaiPhong)
        {
            string query = "SELECT * FROM QuanLyPhongChieu.LOAIPHONG WHERE MaLoaiPhong = @mlp";
            DbParameter[] parameters = new DbParameter[]
            {
                new MySqlParameter("@mlp", maLoaiPhong)
            };

            DataTable dt = await _dbService.ExecuteQueryAsync(query, parameters);
            if (dt.Rows.Count == 0) return null;

            DataRow row = dt.Rows[0];
            return new LoaiPhong
            {
                MaLoaiPhong = row["MaLoaiPhong"].ToString() ?? string.Empty,
                TenLoaiPhong = row["TenLoaiPhong"].ToString() ?? string.Empty
            };
        }
    }
}