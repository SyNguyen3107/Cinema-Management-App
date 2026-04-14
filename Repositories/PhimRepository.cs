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
            string query = "SELECT * FROM PHIM";
            DataTable dt = _dbService.ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                Phim phim = new Phim
                {
                    MaPhim = Convert.ToInt32(row["MaPhim"]),
                    TenPhim = row["TenPhim"].ToString(),
                    ThoiLuong = Convert.ToInt32(row["ThoiLuong"]),
                    MaNhanPhim = Convert.ToInt32(row["MaNhanPhim"]),
                    MaTheLoai = Convert.ToInt32(row["MaTheLoai"]),
                    TenDaoDien = row["TenDaoDien"].ToString(),
                    TenDienVienChinh = row["TenDienVienChinh"].ToString(),
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
    INSERT INTO PHIM (TenPhim, ThoiLuong, MaNhanPhim, MaTheLoai, TenDaoDien, TenDienVienChinh, NgayKhoiChieu) 
    VALUES (@TenPhim, @ThoiLuong, @MaNhanPhim, @MaTheLoai, @TenDaoDien, @TenDienVienChinh, @NgayKhoiChieu);
    SELECT LAST_INSERT_ID();";

                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@TenPhim", phimMoi.TenPhim),
                    new MySqlParameter("@ThoiLuong", phimMoi.ThoiLuong),
                    new MySqlParameter("@MaNhanPhim", phimMoi.MaNhanPhim),
                    new MySqlParameter("@MaTheLoai", phimMoi.MaTheLoai),
                    new MySqlParameter("@TenDaoDien", phimMoi.TenDaoDien),
                    new MySqlParameter("@TenDienVienChinh", phimMoi.TenDienVienChinh),
                    new MySqlParameter("@NgayKhoiChieu", phimMoi.NgayKhoiChieu)
                };

                object result = _dbService.ExecuteScalar(queryInsertPhim, parameters);

                if (result != null)
                {
                    // Ép kiểu object về int
                    int newMaPhim = Convert.ToInt32(result);
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}