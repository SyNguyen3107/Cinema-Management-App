using Cinema_Management_App.Models;
using Cinema_Management_App.Services;
using System;
using System.Collections.Generic;
using System.Data;

namespace Cinema_Management_App.Repositories
{
    public class TheLoaiRepository
    {
        private readonly MySQLService _dbService;

        public TheLoaiRepository(MySQLService dbService)
        {
            _dbService = dbService;
        }

        public List<TheLoai> GetAllTheLoai()
        {
            List<TheLoai> danhSach = new List<TheLoai>();
            string query = "SELECT * FROM QuanLyPhim.THELOAI";

            DataTable dt = _dbService.ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                danhSach.Add(new TheLoai
                {
                    MaTheLoai = row["MaTheLoai"].ToString() ?? string.Empty,
                    TenTheLoai = row["TenTheLoai"].ToString(),
                });
            }
            return danhSach;
        }
    }
}