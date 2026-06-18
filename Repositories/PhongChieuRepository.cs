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
                        using (var cmdPhong = conn.CreateCommand())
                        {
                            cmdPhong.Transaction = trans;
                            cmdPhong.CommandText = @"INSERT INTO QuanLyRapPhim.PHONGCHIEU 
                                                    (MaPhong, TenPhong, MaLoaiPhong, MaTinhTrangPhong, GhiChu) 
                                                    VALUES (@mp, @ten, @loai, @tt, @gc);";

                            cmdPhong.AddParameterWithValue("@mp", phong.MaPhong);
                            cmdPhong.AddParameterWithValue("@ten", phong.TenPhong);
                            cmdPhong.AddParameterWithValue("@loai", phong.MaLoaiPhong);
                            cmdPhong.AddParameterWithValue("@tt", phong.MaTinhTrang);
                            cmdPhong.AddParameterWithValue("@gc", string.IsNullOrWhiteSpace(phong.GhiChu) ? DBNull.Value : (object)phong.GhiChu);

                            await cmdPhong.ExecuteNonQueryAsync();
                        }

                        using (var cmdGhe = conn.CreateCommand())
                        {
                            cmdGhe.Transaction = trans;
                            cmdGhe.CommandText = @"INSERT INTO QuanLyRapPhim.GHE 
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
                DELETE FROM QuanLyRapPhim.GHE WHERE MaPhong = @mp;
                DELETE FROM QuanLyRapPhim.PHONGCHIEU WHERE MaPhong = @mp;";

            DbParameter[] parameters = new DbParameter[]
            {
                new MySqlParameter("@mp", maPhong)
            };

            int result = await _dbService.ExecuteNonQueryAsync(query, parameters);
            return result > 0;
        }

        public async Task<bool> ExistsAsync(string maPhong)
        {
            string query = "SELECT COUNT(1) FROM QuanLyRapPhim.PHONGCHIEU WHERE MaPhong = @mp";
            DbParameter[] parameters = new DbParameter[]
            {
                new MySqlParameter("@mp", maPhong)
            };

            var result = await _dbService.ExecuteScalarAsync(query, parameters);
            return Convert.ToInt32(result) > 0;
        }

        public async Task<string> GenerateMaPhong()
        {
            string maPhong;
            do
            {
                maPhong = "PC" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            } while (await ExistsAsync(maPhong));
          
            return maPhong;
        }

        public async Task<IEnumerable<PhongChieu>> GetAllAsync()
        {
            string query = "SELECT * FROM QuanLyRapPhim.PHONGCHIEU";
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
            string query = "SELECT * FROM QuanLyRapPhim.PHONGCHIEU WHERE MaPhong = @mp";
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

        public async Task<IEnumerable<TraCuuPhongChieuDTO>> TraCuuPhongChieuAsync(
    string? maPhong, string? tenPhong, string? loaiPhong, string? tinhTrang, string? ghiChu,
    int? soGheTu, int? soGheDen, decimal? tongTienTu, decimal? tongTienDen)
        {
            var danhSach = new List<TraCuuPhongChieuDTO>();

            string query = @"
        SELECT 
            P.MaPhong,
            P.TenPhong,
            LP.TenLoaiPhong AS LoaiPhong,
            TT.TenTinhTrangPhong AS TinhTrangPhong,
            P.GhiChu,
            COUNT(G.MaGhe) AS SoLuongGhe,
            COALESCE(SUM(LG.DonGia), 0) AS TongThanhTien
        FROM QuanLyRapPhim.PHONGCHIEU P
        LEFT JOIN QuanLyRapPhim.LOAIPHONG LP ON P.MaLoaiPhong = LP.MaLoaiPhong
        LEFT JOIN QuanLyRapPhim.TINHTRANGPHONG TT ON P.MaTinhTrangPhong = TT.MaTinhTrangPhong
        LEFT JOIN QuanLyRapPhim.GHE G ON P.MaPhong = G.MaPhong
        LEFT JOIN QuanLyRapPhim.LOAIGHE LG ON G.MaLoaiGhe = LG.MaLoaiGhe
        WHERE 1=1 ";

            var parameters = new List<MySqlParameter>();

            if (!string.IsNullOrWhiteSpace(maPhong))
            {
                query += " AND P.MaPhong LIKE @maPhong ";
                parameters.Add(new MySqlParameter("@maPhong", $"%{maPhong}%"));
            }
            if (!string.IsNullOrWhiteSpace(tenPhong))
            {
                query += " AND P.TenPhong LIKE @tenPhong ";
                parameters.Add(new MySqlParameter("@tenPhong", $"%{tenPhong}%"));
            }
            if (!string.IsNullOrEmpty(loaiPhong))
            {
                query += " AND LP.TenLoaiPhong LIKE @tenLoaiPhong ";
                parameters.Add(new MySqlParameter("@tenLoaiPhong", $"%{loaiPhong}%"));
            }
            if (!string.IsNullOrEmpty(tinhTrang))
            {
                query += " AND TT.TenTinhTrangPhong LIKE @tenTinhTrangPhong ";
                parameters.Add(new MySqlParameter("@tenTinhTrangPhong", $"%{tinhTrang}%"));
            }
            if (!string.IsNullOrEmpty(ghiChu))
            {
                query += " AND P.GhiChu LIKE @ghiChu ";
                parameters.Add(new MySqlParameter("@ghiChu", $"%{ghiChu}%"));
            }

            query += " GROUP BY P.MaPhong, P.TenPhong, LP.TenLoaiPhong, TT.TenTinhTrangPhong, P.GhiChu ";

            bool hasHaving = false;

            if (soGheTu.HasValue)
            {
                query += " HAVING SoLuongGhe >= @soGheTu ";
                parameters.Add(new MySqlParameter("@soGheTu", soGheTu.Value));
                hasHaving = true;
            }
            if (soGheDen.HasValue)
            {
                if (hasHaving)
                {
                    query += " AND SoLuongGhe <= @soGheDen ";
                }
                else
                {
                    query += " HAVING SoLuongGhe <= @soGheDen ";
                    hasHaving = true;
                }
                parameters.Add(new MySqlParameter("@soGheDen", soGheDen.Value));
            }

            if (tongTienTu.HasValue)
            {
                if (hasHaving)
                {
                    query += " AND TongThanhTien >= @tongThanhTienTu ";
                }
                else
                {
                    query += " HAVING TongThanhTien >= @tongThanhTienTu ";
                    hasHaving = true;
                }
                parameters.Add(new MySqlParameter("@tongThanhTienTu", tongTienTu.Value));
            }
            if (tongTienDen.HasValue)
            {
                if (hasHaving)
                {
                    query += " AND TongThanhTien <= @tongThanhTienDen ";
                }
                else
                {
                    query += " HAVING TongThanhTien <= @tongThanhTienDen ";
                    hasHaving = true;
                }
                parameters.Add(new MySqlParameter("@tongThanhTienDen", tongTienDen.Value));
            }

            var dt = await _dbService.ExecuteQueryAsync(query, parameters.ToArray());

            int stt = 1;
            foreach (DataRow row in dt.Rows)
            {
                danhSach.Add(new TraCuuPhongChieuDTO
                {
                    STT = stt++,
                    MaPhong = row["MaPhong"].ToString() ?? "",
                    TenPhong = row["TenPhong"].ToString() ?? "",
                    LoaiPhong = row["LoaiPhong"].ToString() ?? "",
                    TinhTrangPhong = row["TinhTrangPhong"].ToString() ?? "",
                    SoLuongGhe = Convert.ToInt32(row["SoLuongGhe"]),
                    TongThanhTien = Convert.ToDecimal(row["TongThanhTien"]),
                    GhiChu = row["GhiChu"].ToString() ?? ""
                });
            }

            return danhSach;
        }

        public async Task<bool> UpdateAsync(PhongChieu phong)
        {
            string query = @"
                UPDATE QuanLyRapPhim.PHONGCHIEU 
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