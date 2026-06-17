using Cinema_Management_App.Models;
using Cinema_Management_App.Services;
using Cinema_Management_App.Interfaces;
using Cinema_Management_App.Extensions;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Cinema_Management_App.Repositories
{
    public class PhongChieuRepository : IPhongChieuRepository
    {
        private readonly IDatabaseService _dbService;

        public PhongChieuRepository(IDatabaseService dbService)
        {
            _dbService = dbService;
        }

        public async Task<bool> AddPhongChieuAsync(PhongChieu phong, IEnumerable<Ghe> dsGhe)
        {
            using (var conn = _dbService.CreateConnection())
            {
                await conn.OpenAsync();

                using (var trans = await conn.BeginTransactionAsync())
                {
                    try
                    {
                        // 1. Insert Room (PhongChieu) information
                        using (var cmdPhong = conn.CreateCommand())
                        {
                            cmdPhong.Transaction = trans;
                            cmdPhong.CommandText = @"INSERT INTO QuanLyPhongChieu.PHONGCHIEU 
                                                    (MaPhong, TenPhong, MaLoaiPhong, MaTinhTrangPhong, GhiChu) 
                                                    VALUES (@mp, @ten, @loai, @tt, @gc);";

                            cmdPhong.AddParameterWithValue("@mp", phong.MaPhong);
                            cmdPhong.AddParameterWithValue("@ten", phong.TenPhong);
                            cmdPhong.AddParameterWithValue("@loai", phong.MaLoaiPhong);
                            cmdPhong.AddParameterWithValue("@tt", phong.MaTinhTrang);
                            cmdPhong.AddParameterWithValue("@gc", string.IsNullOrWhiteSpace(phong.GhiChu) ? DBNull.Value : (object)phong.GhiChu);

                            await cmdPhong.ExecuteNonQueryAsync();
                        }

                        // 2. Insert Seats (Ghe) information linked to the Room
                        using (var cmdGhe = conn.CreateCommand())
                        {
                            cmdGhe.Transaction = trans;
                            cmdGhe.CommandText = @"INSERT INTO QuanLyPhongChieu.GHE 
                                                  (MaGhe, MaSoGhe, MaPhong, MaLoaiGhe) 
                                                  VALUES (@mg, @ms, @mp, @mlg);";

                            foreach (var ghe in dsGhe)
                            {
                                // Clear previous parameters to avoid duplicate key errors in the loop
                                cmdGhe.Parameters.Clear();

                                cmdGhe.AddParameterWithValue("@mg", ghe.MaGhe);
                                cmdGhe.AddParameterWithValue("@ms", ghe.MaSoGhe);
                                cmdGhe.AddParameterWithValue("@mp", phong.MaPhong);
                                cmdGhe.AddParameterWithValue("@mlg", ghe.MaLoaiGhe);

                                await cmdGhe.ExecuteNonQueryAsync();
                            }
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

        public async Task<bool> DeleteAsync(string maPhong)
        {
            // Delete associated seats first due to Foreign Key constraints, then delete the room
            string query = @"
                DELETE FROM QuanLyPhongChieu.GHE WHERE MaPhong = @mp;
                DELETE FROM QuanLyPhongChieu.PHONGCHIEU WHERE MaPhong = @mp;";

            DbParameter[] parameters = new DbParameter[]
            {
                new MySqlParameter("@mp", maPhong)
            };

            int result = await _dbService.ExecuteNonQueryAsync(query, parameters);
            return result > 0;
        }

        public async Task<bool> ExistsAsync(string maPhong)
        {
            string query = "SELECT COUNT(1) FROM QuanLyPhongChieu.PHONGCHIEU WHERE MaPhong = @mp";
            DbParameter[] parameters = new DbParameter[]
            {
                new MySqlParameter("@mp", maPhong)
            };

            var result = await _dbService.ExecuteScalarAsync(query, parameters);
            return Convert.ToInt32(result) > 0;
        }

        public string GenerateMaPhong()
        {
            // Generate a shortened UUID prefixed with "PC"
            return "PC" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        }

        public async Task<IEnumerable<PhongChieu>> GetAllAsync()
        {
            string query = "SELECT * FROM QuanLyPhongChieu.PHONGCHIEU";
            var dt = await _dbService.ExecuteQueryAsync(query);

            var dSPhongChieu = new List<PhongChieu>();
            foreach (DataRow row in dt.Rows)
            {
                dSPhongChieu.Add(new PhongChieu
                {
                    MaPhong = row["MaPhong"].ToString() ?? string.Empty,
                    TenPhong = row["TenPhong"].ToString() ?? string.Empty,
                    MaLoaiPhong = row["MaLoaiPhong"].ToString() ?? string.Empty,
                    MaTinhTrang = row["MaTinhTrangPhong"]?.ToString() ?? string.Empty,
                    GhiChu = row["GhiChu"]?.ToString() ?? string.Empty
                });
            }
            return dSPhongChieu;
        }

        public async Task<PhongChieu?> GetByIdAsync(string maPhong)
        {
            string query = "SELECT * FROM QuanLyPhongChieu.PHONGCHIEU WHERE MaPhong = @mp";
            DbParameter[] parameters = new DbParameter[]
            {
                new MySqlParameter("@mp", maPhong)
            };

            var dt = await _dbService.ExecuteQueryAsync(query, parameters);

            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];
            return new PhongChieu
            {
                MaPhong = row["MaPhong"].ToString() ?? string.Empty,
                TenPhong = row["TenPhong"].ToString() ?? string.Empty,
                MaLoaiPhong = row["MaLoaiPhong"].ToString() ?? string.Empty,
                MaTinhTrang = row["MaTinhTrangPhong"]?.ToString() ?? string.Empty,
                GhiChu = row["GhiChu"]?.ToString() ?? string.Empty
            };
        }

        public async Task<bool> UpdateAsync(PhongChieu phong)
        {
            string query = @"
                UPDATE QuanLyPhongChieu.PHONGCHIEU 
                SET TenPhong = @ten, MaLoaiPhong = @loai, MaTinhTrangPhong = @tt, GhiChu = @gc 
                WHERE MaPhong = @mp";

            DbParameter[] parameters = new DbParameter[]
            {
                new MySqlParameter("@ten", phong.TenPhong),
                new MySqlParameter("@loai", phong.MaLoaiPhong),
                new MySqlParameter("@tt", phong.MaTinhTrang),
                new MySqlParameter("@gc", string.IsNullOrWhiteSpace(phong.GhiChu) ? DBNull.Value : (object)phong.GhiChu),
                new MySqlParameter("@mp", phong.MaPhong)
            };

            int result = await _dbService.ExecuteNonQueryAsync(query, parameters);
            return result > 0;
        }
    }
}