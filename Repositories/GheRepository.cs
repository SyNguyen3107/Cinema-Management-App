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
                            cmdGhe.CommandText = @"INSERT INTO QuanLyPhongChieu.GHE
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
            string query = @"SELECT COUNT(1) FROM QuanLyPhongChieu.GHE WHERE MaGhe = @mg;";
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
            string query = @"SELECT * FROM QuanLyPhongChieu.GHE";
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
            string query = "SELECT * FROM QuanLyPhongChieu.GHE WHERE MaGhe = @mg";
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

        public async Task<Ghe?> GetByRoomIdAsync(string maPhong)
        {
            string query = "SELECT * FROM QuanLyPhongChieu.GHE WHERE MaPhong = @mp";
            DbParameter[] parameters = new DbParameter[]
            {
                new MySqlParameter("@mp", maPhong)
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
    }
}