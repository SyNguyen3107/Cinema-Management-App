using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Cinema_Management_App.Models;
using Cinema_Management_App.Services;
using Cinema_Management_App.Interfaces;
using Cinema_Management_App.Extensions;

namespace Cinema_Management_App.Repositories
{
    public class PhimRepository : IPhimRepository
    {
        private readonly IDatabaseService _dbService;

        // Inject the abstract database service instead of a concrete class
        public PhimRepository(IDatabaseService dbService)
        {
            _dbService = dbService;
        }

        public async Task<IEnumerable<Phim>> GetAllAsync()
        {
            string query = "SELECT * FROM QuanLyPhim.PHIM";
            DataTable dt = await _dbService.ExecuteQueryAsync(query);

            var danhSachPhim = new List<Phim>();
            foreach (DataRow row in dt.Rows)
            {
                danhSachPhim.Add(new Phim
                {
                    MaPhim = row["MaPhim"].ToString() ?? string.Empty,
                    TenPhim = row["TenPhim"].ToString() ?? string.Empty,
                    ThoiLuong = Convert.ToInt32(row["ThoiLuong"]),
                    MaNhanPhim = row["MaNhanPhim"].ToString() ?? string.Empty,
                    MaTheLoai = row["MaTheLoai"].ToString() ?? string.Empty,
                    TenDaoDien = row["TenDaoDien"].ToString() ?? string.Empty,
                    TenDienVienChinh = row["TenDienVienChinh"].ToString() ?? string.Empty,
                    NgayKhoiChieu = Convert.ToDateTime(row["NgayKhoiChieu"])
                });
            }

            return danhSachPhim;
        }

        public async Task<Phim?> GetByIdAsync(string maPhim)
        {
            string query = "SELECT * FROM QuanLyPhim.PHIM WHERE MaPhim = @MaPhim";
            DbParameter[] parameters = new DbParameter[]
            {
                new MySqlParameter("@MaPhim", maPhim)
            };

            DataTable dt = await _dbService.ExecuteQueryAsync(query, parameters);
            if (dt.Rows.Count == 0) return null;

            DataRow row = dt.Rows[0];
            return new Phim
            {
                MaPhim = row["MaPhim"].ToString() ?? string.Empty,
                TenPhim = row["TenPhim"].ToString() ?? string.Empty,
                ThoiLuong = Convert.ToInt32(row["ThoiLuong"]),
                MaNhanPhim = row["MaNhanPhim"].ToString() ?? string.Empty,
                MaTheLoai = row["MaTheLoai"].ToString() ?? string.Empty,
                TenDaoDien = row["TenDaoDien"].ToString() ?? string.Empty,
                TenDienVienChinh = row["TenDienVienChinh"].ToString() ?? string.Empty,
                NgayKhoiChieu = Convert.ToDateTime(row["NgayKhoiChieu"])
            };
        }

        public async Task<bool> AddPhimAsync(Phim phimMoi)
        {
            try
            {
                string queryInsertPhim = @"
                    INSERT INTO QuanLyPhim.PHIM 
                    (MaPhim, TenPhim, ThoiLuong, MaNhanPhim, MaTheLoai, TenDaoDien, TenDienVienChinh, NgayKhoiChieu) 
                    VALUES 
                    (@MaPhim, @TenPhim, @ThoiLuong, @MaNhanPhim, @MaTheLoai, @TenDaoDien, @TenDienVienChinh, @NgayKhoiChieu);";

                // Using Connection and Command via using block for strict resource management
                using (var conn = _dbService.CreateConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = queryInsertPhim;

                        // Utilizing the implemented extension method for cleaner code
                        cmd.AddParameterWithValue("@MaPhim", phimMoi.MaPhim);
                        cmd.AddParameterWithValue("@TenPhim", phimMoi.TenPhim);
                        cmd.AddParameterWithValue("@ThoiLuong", phimMoi.ThoiLuong);
                        cmd.AddParameterWithValue("@MaNhanPhim", phimMoi.MaNhanPhim);
                        cmd.AddParameterWithValue("@MaTheLoai", phimMoi.MaTheLoai);
                        cmd.AddParameterWithValue("@TenDaoDien", string.IsNullOrWhiteSpace(phimMoi.TenDaoDien) ? DBNull.Value : (object)phimMoi.TenDaoDien);
                        cmd.AddParameterWithValue("@TenDienVienChinh", string.IsNullOrWhiteSpace(phimMoi.TenDienVienChinh) ? DBNull.Value : (object)phimMoi.TenDienVienChinh);
                        cmd.AddParameterWithValue("@NgayKhoiChieu", phimMoi.NgayKhoiChieu == default ? DBNull.Value : (object)phimMoi.NgayKhoiChieu);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while adding movie: " + ex.Message, ex);
            }
        }

        public async Task<bool> UpdateAsync(Phim phim)
        {
            string query = @"
                UPDATE QuanLyPhim.PHIM 
                SET TenPhim = @TenPhim, ThoiLuong = @ThoiLuong, MaNhanPhim = @MaNhanPhim, 
                    MaTheLoai = @MaTheLoai, TenDaoDien = @TenDaoDien, TenDienVienChinh = @TenDienVienChinh, 
                    NgayKhoiChieu = @NgayKhoiChieu 
                WHERE MaPhim = @MaPhim";

            DbParameter[] parameters = new DbParameter[]
            {
                new MySqlParameter("@TenPhim", phim.TenPhim),
                new MySqlParameter("@ThoiLuong", phim.ThoiLuong),
                new MySqlParameter("@MaNhanPhim", phim.MaNhanPhim),
                new MySqlParameter("@MaTheLoai", phim.MaTheLoai),
                new MySqlParameter("@TenDaoDien", string.IsNullOrWhiteSpace(phim.TenDaoDien) ? DBNull.Value : (object)phim.TenDaoDien),
                new MySqlParameter("@TenDienVienChinh", string.IsNullOrWhiteSpace(phim.TenDienVienChinh) ? DBNull.Value : (object)phim.TenDienVienChinh),
                new MySqlParameter("@NgayKhoiChieu", phim.NgayKhoiChieu == default ? DBNull.Value : (object)phim.NgayKhoiChieu),
                new MySqlParameter("@MaPhim", phim.MaPhim)
            };

            int rowsAffected = await _dbService.ExecuteNonQueryAsync(query, parameters);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(string maPhim)
        {
            string query = "DELETE FROM QuanLyPhim.PHIM WHERE MaPhim = @MaPhim";
            DbParameter[] parameters = new DbParameter[]
            {
                new MySqlParameter("@MaPhim", maPhim)
            };

            int rowsAffected = await _dbService.ExecuteNonQueryAsync(query, parameters);
            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(string maPhim)
        {
            string query = "SELECT COUNT(1) FROM QuanLyPhim.PHIM WHERE MaPhim = @MaPhim";
            DbParameter[] parameters = new DbParameter[]
            {
                new MySqlParameter("@MaPhim", maPhim)
            };

            var result = await _dbService.ExecuteScalarAsync(query, parameters);
            return Convert.ToInt32(result) > 0;
        }

        public async Task<string> GenerateMaPhimAsync()
        {
            string maPhim;

            do
            {
                // 1. Sinh một mã ngẫu nhiên trước
                maPhim = "PM" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();

                // 2. Chờ kết quả kiểm tra dưới Database xem mã này đã tồn tại chưa
            }
            while (await GetByIdAsync(maPhim) != null); // Nếu kết quả khác null (tức là trùng), lặp lại để tạo mã khác

            // Vòng lặp chỉ thoát ra khi tìm được mã chưa tồn tại (kết quả bằng null)
            return maPhim;
        }
    }
}