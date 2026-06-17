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
    public class LoaiGheRepository : ILoaiGheRepository
    {
        private readonly IDatabaseService _dbService;

        // Inject the abstract database service to enforce loose coupling
        public LoaiGheRepository(IDatabaseService dbService)
        {
            _dbService = dbService;
        }

        public async Task<IEnumerable<LoaiGhe>> GetAllLoaiGheAsync()
        {
            var danhSach = new List<LoaiGhe>();
            string query = "SELECT * FROM QuanLyPhongChieu.LOAIGHE";

            DataTable dt = await _dbService.ExecuteQueryAsync(query);
            foreach (DataRow row in dt.Rows)
            {
                danhSach.Add(new LoaiGhe
                {
                    MaLoaiGhe = row["MaLoaiGhe"].ToString() ?? string.Empty,
                    TenLoaiGhe = row["TenLoaiGhe"].ToString() ?? string.Empty,
                    DonGia = Convert.ToDecimal(row["DonGia"])
                });
            }
            return danhSach;
        }

        public async Task<IEnumerable<LoaiGhe>> GetAllLoaiGheByMaLoaiPhongAsync(string maLoaiPhong)
        {
            var danhSach = new List<LoaiGhe>();

            // Note: Ensure the table name 'QUYDINH_LOAIGHE' matches your actual MySQL schema
            string query = @"SELECT lg.MaLoaiGhe, lg.TenLoaiGhe, lg.DonGia 
                             FROM QuanLyPhongChieu.LOAIGHE lg
                             JOIN QuanLyPhongChieu.QUYDINH_LOAIGHE qdlg ON lg.MaLoaiGhe = qdlg.MaLoaiGhe
                             WHERE qdlg.MaLoaiPhong = @maLoaiPhong";

            DbParameter[] parameters = new DbParameter[]
            {
                new MySqlParameter("@maLoaiPhong", maLoaiPhong)
            };

            DataTable dt = await _dbService.ExecuteQueryAsync(query, parameters);
            foreach (DataRow row in dt.Rows)
            {
                danhSach.Add(new LoaiGhe
                {
                    MaLoaiGhe = row["MaLoaiGhe"].ToString() ?? string.Empty,
                    TenLoaiGhe = row["TenLoaiGhe"].ToString() ?? string.Empty,
                    DonGia = Convert.ToDecimal(row["DonGia"])
                });
            }
            return danhSach;
        }
    }
}