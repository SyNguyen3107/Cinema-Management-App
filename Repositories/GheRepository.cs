using Cinema_Management_App.Extensions;
using Cinema_Management_App.Interfaces;
using Cinema_Management_App.Models;
using Cinema_Management_App.Services;
using Cinema_Management_App.DTOs;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Cinema_Management_App.Repositories
{
    public class GheRepository : IGheRepository
    {
        private readonly IDatabaseService _dbService;

        // Constructor to inject the abstract database service
        public GheRepository(IDatabaseService dbService)
        {
            _dbService = dbService;
        }

        public async Task<bool> AddGheAsync(Ghe ghe, IEnumerable<Ghe> dsGhe)
        {
            using (var conn = _dbService.CreateConnection())
            {
                await conn.OpenAsync();
                using (var trans = await conn.BeginTransactionAsync())
                {
                    try
                    {
                        using (var cmdGhe = conn.CreateCommand())
                        {
                            cmdGhe.Transaction = trans;
                            cmdGhe.CommandText = @"INSERT INTO QuanLyRapPhim.GHE
                                                    (MaGhe, MaSoGhe, MaLoaiGhe, MaPhong) 
                                                    VALUES (@mg, @msg, @mlg, @mp);";

                            cmdGhe.AddParameterWithValue("@mg", ghe.MaGhe);
                            cmdGhe.AddParameterWithValue("@msg", ghe.MaSoGhe);
                            cmdGhe.AddParameterWithValue("@mlg", ghe.MaLoaiGhe);
                            cmdGhe.AddParameterWithValue("@mp", ghe.MaPhong);

                            await cmdGhe.ExecuteNonQueryAsync();
                        }
                        await trans.CommitAsync();
                        return true;
                    }
                    catch (Exception)
                    {
                        await trans.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        public async Task<bool> ExistsAsync(string maGhe)
        {
            string query = @"SELECT COUNT(1) FROM QuanLyRapPhim.GHE WHERE MaGhe = @mg;";
            DbParameter[] parameters =
            {
                new MySqlParameter("@mg", maGhe)
            };
            var result = await _dbService.ExecuteScalarAsync(query, parameters);
            return Convert.ToInt32(result) > 0;
        }

        public async Task<string> GenerateMaGhe()
        {
            string maGhe;

            do
            {
                maGhe = "GH" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            } while (await ExistsAsync(maGhe));
            
            return maGhe;
        }

        public async Task<IEnumerable<Ghe>> GetAllAsync()
        {
            // Fixed: Added '*' operator to the SELECT statement
            string query = @"SELECT * FROM QuanLyRapPhim.GHE";
            var dt = await _dbService.ExecuteQueryAsync(query);

            var dSGhe = new List<Ghe>();
            foreach (DataRow row in dt.Rows)
            {
                var ghe = new Ghe
                {
                    MaGhe = row["MaGhe"].ToString() ?? string.Empty,
                    MaSoGhe = row["MaSoGhe"].ToString() ?? string.Empty,
                    MaPhong = row["MaPhong"].ToString() ?? string.Empty,
                    MaLoaiGhe = row["MaLoaiGhe"].ToString() ?? string.Empty
                };
                dSGhe.Add(ghe);
            }
            return dSGhe;
        }

        public async Task<Ghe?> GetByIdAsync(string maGhe)
        {
            string query = "SELECT * FROM QuanLyRapPhim.GHE WHERE MaGhe = @mg";
            DbParameter[] parameters = new DbParameter[]
            {
                new MySqlParameter("@mg", maGhe)
            };

            var dt = await _dbService.ExecuteQueryAsync(query, parameters);

            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];
            return new Ghe
            {
                MaGhe = row["MaGhe"].ToString() ?? string.Empty,
                MaSoGhe = row["MaSoGhe"].ToString() ?? string.Empty,
                MaPhong = row["MaPhong"]?.ToString() ?? string.Empty,
                MaLoaiGhe = row["MaLoaiGhe"].ToString() ?? string.Empty
            };
        }

        public async Task<IEnumerable<GheDTO>> GetAvailableByRoomId(string maPhong, string maSuatChieu)
        {
            string query = @"SELECT g.MaGhe, g.MaSoGhe, lg.TenLoaiGhe AS TenLoaiGhe, lg.DonGia 
                     FROM GHE g 
                     JOIN LOAIGHE lg ON g.MaLoaiGhe = lg.MaLoaiGhe 
                     WHERE g.MaPhong = @maPhong AND g.MaGhe NOT IN (
                         SELECT ct.MaGhe 
                         FROM CHITIETBANVE ct 
                         JOIN VE v ON ct.MaVe = v.MaVe 
                         WHERE v.MaSuatChieu = @maSuatChieu
                     );";

            DbParameter[] parameters = new DbParameter[]
            {
                new MySqlParameter("@maPhong", maPhong),
                new MySqlParameter("@maSuatChieu", maSuatChieu)
            };

            var dt = await _dbService.ExecuteQueryAsync(query, parameters);

            var danhSachGheTrong = new List<GheDTO>();

            foreach (DataRow row in dt.Rows)
            {
                danhSachGheTrong.Add(new GheDTO
                {
                    MaGhe = row["MaGhe"].ToString() ?? string.Empty,
                    MaSoGhe = row["MaSoGhe"].ToString() ?? string.Empty,
                    TenLoaiGhe = row["TenLoaiGhe"].ToString() ?? string.Empty,
                    DonGia = Convert.ToDecimal(row["DonGia"])
                });
            }

            return danhSachGheTrong;
        }
        public async Task<IEnumerable<GheDTO>> GetAllGheDTOByRoomIdAsync(string maPhong)
        {
            string query = @"SELECT g.MaGhe, g.MaSoGhe, lg.MaLoaiGhe, lg.TenLoaiGhe AS TenLoaiGhe, lg.DonGia 
                     FROM GHE g 
                     JOIN LOAIGHE lg ON g.MaLoaiGhe = lg.MaLoaiGhe 
                     WHERE g.MaPhong = @maPhong;";

            DbParameter[] parameters = new DbParameter[]
            {
        new MySqlParameter("@maPhong", maPhong)
            };

            var dt = await _dbService.ExecuteQueryAsync(query, parameters);
            var danhSachGhe = new List<GheDTO>();

            foreach (DataRow row in dt.Rows)
            {
                danhSachGhe.Add(new GheDTO
                {
                    MaGhe = row["MaGhe"].ToString() ?? string.Empty,
                    MaSoGhe = row["MaSoGhe"].ToString() ?? string.Empty,
                    MaLoaiGhe = row["MaLoaiGhe"].ToString() ?? string.Empty,
                    TenLoaiGhe = row["TenLoaiGhe"].ToString() ?? string.Empty,
                    DonGia = Convert.ToDecimal(row["DonGia"])
                });
            }

            return danhSachGhe;
        }
       
    }
}