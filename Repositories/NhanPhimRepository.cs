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
    public class NhanPhimRepository : INhanPhimRepository
    {
        private readonly IDatabaseService _dbService;

        // Inject the abstract database service to ensure loose coupling
        public NhanPhimRepository(IDatabaseService dbService)
        {
            _dbService = dbService;
        }

        public async Task<IEnumerable<NhanPhim>> GetAllNhanPhimAsync()
        {
            var danhSach = new List<NhanPhim>();
            string query = "SELECT * FROM QuanLyPhim.NHANPHIM";

            DataTable dt = await _dbService.ExecuteQueryAsync(query);
            foreach (DataRow row in dt.Rows)
            {
                danhSach.Add(new NhanPhim
                {
                    MaNhanPhim = row["MaNhanPhim"].ToString() ?? string.Empty,
                    TenNhanPhim = row["TenNhanPhim"].ToString() ?? string.Empty
                });
            }
            return danhSach;
        }

        public async Task<NhanPhim?> GetByIdAsync(int maNhanPhim)
        {
            string query = "SELECT * FROM QuanLyPhim.NHANPHIM WHERE MaNhanPhim = @mnp";
            DbParameter[] parameters = new DbParameter[]
            {
                new MySqlParameter("@mnp", maNhanPhim)
            };

            DataTable dt = await _dbService.ExecuteQueryAsync(query, parameters);
            if (dt.Rows.Count == 0) return null;

            DataRow row = dt.Rows[0];
            return new NhanPhim
            {
                MaNhanPhim = row["MaNhanPhim"].ToString() ?? string.Empty,
                TenNhanPhim = row["TenNhanPhim"].ToString() ?? string.Empty
            };
        }
    }
}