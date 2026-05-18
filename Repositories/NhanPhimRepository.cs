using Cinema_Management_App.Models;
using Cinema_Management_App.Services;
using System;
using System.Collections.Generic;
using System.Data;

namespace Cinema_Management_App.Repositories
{
    public class NhanPhimRepository
    {
        private readonly MySQLService _dbService;

        public NhanPhimRepository(MySQLService dbService)
        {
            _dbService = dbService;
        }

        public List<NhanPhim> GetAllNhanPhim()
        {
            List<NhanPhim> danhSach = new List<NhanPhim>();
            string query = "SELECT * FROM QuanLyPhim.NHANPHIM";
            DataTable dt = _dbService.ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                danhSach.Add(new NhanPhim
                {
                    MaNhanPhim = Convert.ToInt32(row["MaNhanPhim"]),
                    TenNhanPhim = row["TenNhanPhim"].ToString() ?? string.Empty
                });
            }
            return danhSach;
        }
    }
}