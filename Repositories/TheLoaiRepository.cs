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
    public class TheLoaiRepository : ITheLoaiRepository
    {
        private readonly IDatabaseService _dbService;

        // Inject the abstract database service to maintain loose coupling
        public TheLoaiRepository(IDatabaseService dbService)
        {
            _dbService = dbService;
        }

        public async Task<IEnumerable<TheLoai>> GetAllTheLoaiAsync()
        {
            var danhSach = new List<TheLoai>();
            string query = "SELECT * FROM QuanLyPhim.THELOAI";

            DataTable dt = await _dbService.ExecuteQueryAsync(query);
            foreach (DataRow row in dt.Rows)
            {
                danhSach.Add(new TheLoai
                {
                    MaTheLoai = row["MaTheLoai"].ToString() ?? string.Empty,
                    TenTheLoai = row["TenTheLoai"].ToString() ?? string.Empty
                });
            }
            return danhSach;
        }

        public async Task<TheLoai?> GetByIdAsync(string maTheLoai)
        {
            string query = "SELECT * FROM QuanLyPhim.THELOAI WHERE MaTheLoai = @mtl";
            DbParameter[] parameters = new DbParameter[]
            {
                new MySqlParameter("@mtl", maTheLoai)
            };

            DataTable dt = await _dbService.ExecuteQueryAsync(query, parameters);
            if (dt.Rows.Count == 0) return null;

            DataRow row = dt.Rows[0];
            return new TheLoai
            {
                MaTheLoai = row["MaTheLoai"].ToString() ?? string.Empty,
                TenTheLoai = row["TenTheLoai"].ToString() ?? string.Empty
            };
        }
    }
}