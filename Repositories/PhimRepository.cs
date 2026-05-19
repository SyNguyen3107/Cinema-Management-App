using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Cinema_Management_App.Models;
using Cinema_Management_App.Services;

namespace Cinema_Management_App.Repositories
{
    public class PhimRepository
    {
        private readonly MySQLService _dbService;
        public PhimRepository(MySQLService dbService)
        {
            _dbService = dbService;
        }
        public List<Phim> GetAllPhim()
        {
            List<Phim> danhSachPhim = new List<Phim>();
            string query = "SELECT * FROM QuanLyPhim.PHIM";
            DataTable dt = _dbService.ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                Phim phim = new Phim
                {
                    MaPhim = row["MaPhim"].ToString() ?? string.Empty,
                    TenPhim = row["TenPhim"].ToString() ?? string.Empty,
                    ThoiLuong = Convert.ToInt32(row["ThoiLuong"]),
                    MaNhanPhim = Convert.ToInt32(row["MaNhanPhim"]),
                    MaTheLoai = row["MaTheLoai"].ToString() ?? string.Empty,
                    TenDaoDien = row["TenDaoDien"].ToString() ?? string.Empty,
                    TenDienVienChinh = row["TenDienVienChinh"].ToString() ?? string.Empty,
                    NgayKhoiChieu = Convert.ToDateTime(row["NgayKhoiChieu"])
                };

                danhSachPhim.Add(phim);
            }

            return danhSachPhim;
        }
        public bool AddPhim(Phim phimMoi)
        {
            try
            {
                string queryInsertPhim = @"
        INSERT INTO QuanLyPhim.PHIM 
        (MaPhim, TenPhim, ThoiLuong, MaNhanPhim, MaTheLoai, TenDaoDien, TenDienVienChinh, NgayKhoiChieu) 
        VALUES 
        (@MaPhim, @TenPhim, @ThoiLuong, @MaNhanPhim, @MaTheLoai, @TenDaoDien, @TenDienVienChinh, @NgayKhoiChieu);";

                MySqlParameter[] parameters = new MySqlParameter[]
                {
            new MySqlParameter("@MaPhim", phimMoi.MaPhim),
            new MySqlParameter("@TenPhim", phimMoi.TenPhim),
            new MySqlParameter("@ThoiLuong", phimMoi.ThoiLuong),
            new MySqlParameter("@MaNhanPhim", phimMoi.MaNhanPhim),
            new MySqlParameter("@MaTheLoai", phimMoi.MaTheLoai),
            new MySqlParameter("@TenDaoDien", (object)phimMoi.TenDaoDien ?? DBNull.Value),
            new MySqlParameter("@TenDienVienChinh", (object)phimMoi.TenDienVienChinh ?? DBNull.Value),
            new MySqlParameter("@NgayKhoiChieu", (object)phimMoi.NgayKhoiChieu ?? DBNull.Value)
                };

                int rowsAffected = _dbService.ExecuteNonQuery(queryInsertPhim, parameters);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm phim: " + ex.Message, ex);
            }
        }
        public string GetNewMaPhim()
        {
            return "P" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }
    }
}