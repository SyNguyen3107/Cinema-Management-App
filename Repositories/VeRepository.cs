using Cinema_Management_App.Extensions;
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
    public class VeRepository : IVeRepository
    {
        private readonly IDatabaseService _dbService;

        public VeRepository(IDatabaseService dbService)
        {
            _dbService = dbService;
        }

        public async Task<bool> AddVeAsync(Ve ve, IEnumerable<string> danhSachMaGhe)
        {
            using (var conn = _dbService.CreateConnection())
            {
                await conn.OpenAsync();
                using (var trans = await conn.BeginTransactionAsync())
                {
                    try
                    {
                        // 1. Insert into VE table
                        string queryInsertVe = @"
                            INSERT INTO VE (MaVe, NgayBan, TenNhanVienBan, HinhThucThanhToan, MaSuatChieu, TongTien) 
                            VALUES (@MaVe, @NgayBan, @TenNhanVienBan, @HinhThucThanhToan, @MaSuatChieu, @TongTien);";

                        using (var cmdVe = conn.CreateCommand())
                        {
                            cmdVe.CommandText = queryInsertVe;
                            cmdVe.Transaction = trans;

                            cmdVe.AddParameterWithValue("@MaVe", ve.MaVe);
                            cmdVe.AddParameterWithValue("@NgayBan", ve.NgayBan);
                            cmdVe.AddParameterWithValue("@TenNhanVienBan", ve.TenNhanVienBan);
                            cmdVe.AddParameterWithValue("@HinhThucThanhToan", ve.HinhThucThanhToan);
                            cmdVe.AddParameterWithValue("@MaSuatChieu", ve.MaSuatChieu);
                            cmdVe.AddParameterWithValue("@TongTien", ve.TongTien);

                            await cmdVe.ExecuteNonQueryAsync();
                        }

                        // 2. Insert into CHITIETBANVE table for each selected seat
                        string queryInsertChiTiet = @"
                            INSERT INTO CHITIETBANVE (MaVe, MaGhe) 
                            VALUES (@MaVe, @MaGhe);";

                        foreach (var maGhe in danhSachMaGhe)
                        {
                            using (var cmdChiTiet = conn.CreateCommand())
                            {
                                cmdChiTiet.CommandText = queryInsertChiTiet;
                                cmdChiTiet.Transaction = trans;

                                cmdChiTiet.AddParameterWithValue("@MaVe", ve.MaVe);
                                cmdChiTiet.AddParameterWithValue("@MaGhe", maGhe);

                                await cmdChiTiet.ExecuteNonQueryAsync();
                            }
                        }

                        // Commit transaction if all inserts succeeded
                        await trans.CommitAsync();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction on any error to maintain data integrity
                        await trans.RollbackAsync();
                        throw new Exception("Lỗi xảy ra khi thực hiện bán vé: " + ex.Message, ex);
                    }
                }
            }
        }

        public async Task<bool> ExistsAsync(string maVe)
        {
            string query = "SELECT COUNT(1) FROM VE WHERE MaVe = @mv";
            DbParameter[] parameters = new DbParameter[]
            {
                new MySqlParameter("@mv", maVe)
            };

            var result = await _dbService.ExecuteScalarAsync(query, parameters);
            return Convert.ToInt32(result) > 0;
        }

        public async Task<string> GenerateMaVeAsync()
        {
            string maVe;
            do
            {
                // Generate format: VE + 8 random alphanumeric characters
                maVe = "VE" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            } while (await ExistsAsync(maVe));

            return maVe;
        }

        public async Task<IEnumerable<Ve>> GetAllAsync()
        {
            string query = "SELECT * FROM VE";
            var dt = await _dbService.ExecuteQueryAsync(query);
            var dsVe = new List<Ve>();

            foreach (DataRow row in dt.Rows)
            {
                dsVe.Add(new Ve
                {
                    MaVe = row["MaVe"].ToString() ?? string.Empty,
                    NgayBan = Convert.ToDateTime(row["NgayBan"]),
                    TenNhanVienBan = row["TenNhanVienBan"]?.ToString() ?? string.Empty,
                    HinhThucThanhToan = row["HinhThucThanhToan"]?.ToString() ?? string.Empty,
                    MaSuatChieu = row["MaSuatChieu"]?.ToString() ?? string.Empty,
                    TongTien = Convert.ToDecimal(row["TongTien"])
                });
            }
            return dsVe;
        }

        public async Task<Ve?> GetByIdAsync(string maVe)
        {
            string query = "SELECT * FROM VE WHERE MaVe = @mv";
            DbParameter[] parameters = new DbParameter[]
            {
                new MySqlParameter("@mv", maVe)
            };

            var dt = await _dbService.ExecuteQueryAsync(query, parameters);
            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];
            return new Ve
            {
                MaVe = row["MaVe"].ToString() ?? string.Empty,
                NgayBan = Convert.ToDateTime(row["NgayBan"]),
                TenNhanVienBan = row["TenNhanVienBan"]?.ToString() ?? string.Empty,
                HinhThucThanhToan = row["HinhThucThanhToan"]?.ToString() ?? string.Empty,
                MaSuatChieu = row["MaSuatChieu"]?.ToString() ?? string.Empty,
                TongTien = Convert.ToDecimal(row["TongTien"])
            };
        }
        public async Task<IEnumerable<string>> GetDanhSachGheDaBanAsync(string maSuatChieu)
        {
            string query = @"
        SELECT ct.MaGhe 
        FROM CHITIETBANVE ct
        JOIN VE v ON ct.MaVe = v.MaVe
        WHERE v.MaSuatChieu = @msc";

            DbParameter[] parameters = new DbParameter[]
            {
        new MySqlParameter("@msc", maSuatChieu)
            };

            var dt = await _dbService.ExecuteQueryAsync(query, parameters);
            var danhSachGheDaBan = new List<string>();

            foreach (DataRow row in dt.Rows)
            {
                danhSachGheDaBan.Add(row["MaGhe"].ToString() ?? string.Empty);
            }

            return danhSachGheDaBan;
        }
    }
}