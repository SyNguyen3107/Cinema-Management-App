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
using Cinema_Management_App.DTOs;

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
            string query = "SELECT * FROM QuanLyRapPhim.PHIM";
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
            string query = "SELECT * FROM QuanLyRapPhim.PHIM WHERE MaPhim = @MaPhim";
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
                    INSERT INTO QuanLyRapPhim.PHIM 
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
                UPDATE QuanLyRapPhim.PHIM 
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
            string query = "DELETE FROM QuanLyRapPhim.PHIM WHERE MaPhim = @MaPhim";
            DbParameter[] parameters = new DbParameter[]
            {
                new MySqlParameter("@MaPhim", maPhim)
            };

            int rowsAffected = await _dbService.ExecuteNonQueryAsync(query, parameters);
            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(string maPhim)
        {
            string query = "SELECT COUNT(1) FROM QuanLyRapPhim.PHIM WHERE MaPhim = @MaPhim";
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
                maPhim = "PM" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            }
            while (await GetByIdAsync(maPhim) != null);

            return maPhim;
        }

        Task<IEnumerable<Phim>> IPhimRepository.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        Task<Phim?> IPhimRepository.GetByIdAsync(string maPhim)
        {
            throw new NotImplementedException();
        }

        Task<bool> IPhimRepository.AddPhimAsync(Phim phimMoi)
        {
            throw new NotImplementedException();
        }

        Task<bool> IPhimRepository.UpdateAsync(Phim phim)
        {
            throw new NotImplementedException();
        }

        Task<bool> IPhimRepository.DeleteAsync(string maPhim)
        {
            throw new NotImplementedException();
        }

        Task<bool> IPhimRepository.ExistsAsync(string maPhim)
        {
            throw new NotImplementedException();
        }

        Task<string> IPhimRepository.GenerateMaPhimAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<BaoCaoDoanhThuPhimDTO>> GetMoviesRevenueReportsByMonthYear(int thang, int nam)
        {
            var danhSach = new List<BaoCaoDoanhThuPhimDTO>();

            string query = @"
                            SELECT 
                                P.TenPhim,
                                SUM(CTSC.DoanhThuSuatChieu) AS DoanhThu,
                                AVG(CTSC.TongSoGheDaBan / NULLIF(CTSC.TongSoGhePhong, 0)) AS TyLeLapDayGhe
                            FROM (
                                SELECT 
                                    SC.MaPhim,
                                    SC.MaSuatChieu,
                                    SUM(CTVE.TongTien) AS DoanhThuSuatChieu,
                                    SUM(CTVE.SoGheCuaVe) AS TongSoGheDaBan,
                                    (SELECT COUNT(G.MaGhe) FROM QuanLyRapPhim.GHE G WHERE G.MaPhong = SC.MaPhong) AS TongSoGhePhong
                                FROM QuanLyRapPhim.SUATCHIEU SC
                                JOIN (
                                    SELECT 
                                        VE.MaVe, 
                                        VE.MaSuatChieu, 
                                        VE.TongTien,
                                        (SELECT COUNT(CT.MaGhe) FROM QuanLyRapPhim.CHITIETBANVE CT WHERE CT.MaVe = VE.MaVe) AS SoGheCuaVe
                                    FROM QuanLyRapPhim.VE VE
                                    WHERE MONTH(VE.NgayBan) = @thang AND YEAR(VE.NgayBan) = @nam
                                ) AS CTVE ON SC.MaSuatChieu = CTVE.MaSuatChieu
                                GROUP BY SC.MaPhim, SC.MaSuatChieu, SC.MaPhong
                            ) AS CTSC
                            JOIN QuanLyRapPhim.PHIM P ON CTSC.MaPhim = P.MaPhim
                            GROUP BY CTSC.MaPhim, P.TenPhim
                            ORDER BY DoanhThu DESC";

            var parameters = new List<MySqlParameter>
    {
        new MySqlParameter("@thang", thang),
        new MySqlParameter("@nam", nam)
    };

            var dt = await _dbService.ExecuteQueryAsync(query, parameters.ToArray());

            int stt = 1;
            foreach (DataRow row in dt.Rows)
            {
                danhSach.Add(new BaoCaoDoanhThuPhimDTO
                {
                    STT = stt++,
                    TenPhim = row["TenPhim"].ToString() ?? "",
                    
                    DoanhThu = row["DoanhThu"] != DBNull.Value ? Convert.ToDecimal(row["DoanhThu"]) : 0,
                    TyLeLapDayGhe = row["TyLeLapDayGhe"] != DBNull.Value ? Convert.ToDouble(row["TyLeLapDayGhe"]) : 0
                });
            }

            return danhSach;
        }
    }
}