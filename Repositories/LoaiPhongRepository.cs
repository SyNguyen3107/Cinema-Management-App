using Cinema_Management_App.DTOs;
using Cinema_Management_App.Interfaces;
using Cinema_Management_App.Models;
using Cinema_Management_App.Services;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

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
            string query = "SELECT * FROM QuanLyRapPhim.LOAIPHONG";

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
            string query = "SELECT * FROM QuanLyRapPhim.LOAIPHONG WHERE MaLoaiPhong = @mlp";
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
        public async Task<IEnumerable<BaoCaoDoanhThuLoaiPhongLoaiGheDTO>> GetRevenueReportByRoomTypeAndSeatTypeAsync(int month, int year)
        {
            var danhSach = new List<BaoCaoDoanhThuLoaiPhongLoaiGheDTO>();

            string query = @"
                    SELECT 
                        TenLoaiPhong,
                        TenLoaiGhe,
                        DoanhThu,
                        DoanhThu / NULLIF(SUM(DoanhThu) OVER(), 0) AS Tyle
                    FROM (
                        SELECT 
                            LP.TenLoaiPhong,
                            LG.TenLoaiGhe,
                            SUM(LG.DonGia) AS DoanhThu
                        FROM QuanLyRapPhim.VE V
                        JOIN QuanLyRapPhim.SUATCHIEU SC ON V.MaSuatChieu = SC.MaSuatChieu
                        JOIN QuanLyRapPhim.PHONGCHIEU PC ON SC.MaPhong = PC.MaPhong
                        JOIN QuanLyRapPhim.LOAIPHONG LP ON PC.MaLoaiPhong = LP.MaLoaiPhong
                        JOIN QuanLyRapPhim.CHITIETBANVE CT ON V.MaVe = CT.MaVe
                        JOIN QuanLyRapPhim.GHE G ON CT.MaGhe = G.MaGhe
                        JOIN QuanLyRapPhim.LOAIGHE LG ON G.MaLoaiGhe = LG.MaLoaiGhe
                        WHERE MONTH(V.NgayBan) = @thang AND YEAR(V.NgayBan) = @nam
                        GROUP BY LP.TenLoaiPhong, LG.TenLoaiGhe
                    ) AS DoanhThuNhom
                    ORDER BY TenLoaiPhong, TenLoaiGhe";

            var parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@thang", month),
                new MySqlParameter("@nam", year)
            };

            var dt = await _dbService.ExecuteQueryAsync(query, parameters.ToArray());

            int stt = 1;
            foreach (DataRow row in dt.Rows)
            {
                danhSach.Add(new BaoCaoDoanhThuLoaiPhongLoaiGheDTO
                {
                    STT = stt++,
                    TenLoaiPhong = row["TenLoaiPhong"].ToString() ?? "",
                    TenLoaiGhe = row["TenLoaiGhe"].ToString() ?? "",
                    DoanhThu = row["DoanhThu"] != DBNull.Value ? Convert.ToDecimal(row["DoanhThu"]) : 0,
                    TyLeDongGopDoanhThu = row["TyLe"] != DBNull.Value ? Convert.ToDouble(row["TyLe"]) : 0
                });
            }

            return danhSach;
        }
    }
}