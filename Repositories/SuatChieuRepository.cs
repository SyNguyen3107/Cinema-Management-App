using Cinema_Management_App.Extensions;
using Cinema_Management_App.Interfaces;
using Cinema_Management_App.Models;
using Cinema_Management_App.Services;
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
    public class SuatChieuRepository : ISuatChieuRepository
    {
        private readonly IDatabaseService _dbService;

        public SuatChieuRepository(IDatabaseService dbService)
        {
            _dbService = dbService;
        }

        public async Task<bool> AddSuatChieuAsync(SuatChieu suatChieu)
        {
            using (var conn = _dbService.CreateConnection())
            {
                await conn.OpenAsync();
                using (var trans = await conn.BeginTransactionAsync())
                {
                    try
                    {
                        // ĐÃ SỬA: Xóa tiền tố Database và thêm dấu phẩy phân cách ở VALUES
                        string queryInsertSuatChieu = @"
                            INSERT INTO QuanLyRapPhim.SUATCHIEU
                            (MaSuatChieu, NgayChieu, GioBatDau, GioKetThuc, MaPhong, MaPhim) 
                            VALUES 
                            (@MaSuatChieu, @NgayChieu, @GioBatDau, @GioKetThuc, @MaPhong, @MaPhim);";

                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = queryInsertSuatChieu;
                            cmd.Transaction = trans; // Đừng quên gán Transaction cho Command nhé!

                            cmd.AddParameterWithValue("@MaSuatChieu", suatChieu.MaSuatChieu);
                            cmd.AddParameterWithValue("@NgayChieu", suatChieu.NgayChieu);
                            cmd.AddParameterWithValue("@GioBatDau", suatChieu.GioBatDau);
                            cmd.AddParameterWithValue("@GioKetThuc", suatChieu.GioKetThuc);
                            cmd.AddParameterWithValue("@MaPhong", suatChieu.MaPhong);
                            cmd.AddParameterWithValue("@MaPhim", suatChieu.MaPhim);

                            int rowsAffected = await cmd.ExecuteNonQueryAsync();
                            await trans.CommitAsync(); // Nếu Add thành công thì Commit
                            return rowsAffected > 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        await trans.RollbackAsync();
                        throw new Exception("Error occurred while adding: " + ex.Message, ex);
                    }
                }
            }
        }

        public async Task<bool> ExistsAsync(string maSuatchieu)
        {
            // ĐÃ SỬA: Xóa tiền tố QuanLyRapPhim.
            string query = "SELECT COUNT(1) FROM QuanLyRapPhim.SUATCHIEU WHERE MaSuatChieu = @msc";
            DbParameter[] parameters = new DbParameter[]
            {
                new MySqlParameter("@msc", maSuatchieu)
            };

            var result = await _dbService.ExecuteScalarAsync(query, parameters);
            return Convert.ToInt32(result) > 0;
        }

        public async Task<string> GenerateMaSuatChieu()
        {
            string maSuatChieu;
            do
            {
                maSuatChieu = "SC" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            } while (await ExistsAsync(maSuatChieu));

            return maSuatChieu;
        }

        public async Task<IEnumerable<SuatChieu>> GetAllAsync()
        {
            string query = "SELECT * FROM QuanLyRapPhim.SUATCHIEU";
            var dt = await _dbService.ExecuteQueryAsync(query);

            var dsSuatChieu = new List<SuatChieu>();

            foreach (DataRow row in dt.Rows)
            {
                TimeSpan gioBatDau = row["GioBatDau"] is TimeSpan tsBatDau
                    ? tsBatDau
                    : TimeSpan.Parse(row["GioBatDau"].ToString() ?? "00:00:00");

                TimeSpan gioKetThuc = row["GioKetThuc"] is TimeSpan tsKetThuc
                    ? tsKetThuc
                    : TimeSpan.Parse(row["GioKetThuc"].ToString() ?? "00:00:00");

                dsSuatChieu.Add(new SuatChieu
                {
                    MaSuatChieu = row["MaSuatChieu"].ToString() ?? string.Empty,
                    NgayChieu = Convert.ToDateTime(row["NgayChieu"]),
                    GioBatDau = gioBatDau,
                    GioKetThuc = gioKetThuc,
                    MaPhong = row["MaPhong"]?.ToString() ?? string.Empty,
                    MaPhim = row["MaPhim"]?.ToString() ?? string.Empty
                });
            }

            return dsSuatChieu;
        }

        public async Task<SuatChieu?> GetByIdAsync(string maSuatchieu)
        {
            string query = "SELECT * FROM QuanLyRapPhim.SUATCHIEU WHERE MaSuatChieu = @msc";
            DbParameter[] parameters = new DbParameter[]
            {
                new MySqlParameter("@msc", maSuatchieu)
            };

            var dt = await _dbService.ExecuteQueryAsync(query, parameters);

            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];
            TimeSpan gioBatDau = row["GioBatDau"] is TimeSpan tsBatDau
                    ? tsBatDau
                    : TimeSpan.Parse(row["GioBatDau"].ToString() ?? "00:00:00");

            TimeSpan gioKetThuc = row["GioKetThuc"] is TimeSpan tsKetThuc
                ? tsKetThuc
                : TimeSpan.Parse(row["GioKetThuc"].ToString() ?? "00:00:00");

            return new SuatChieu
            {
                MaSuatChieu = row["MaSuatChieu"].ToString() ?? string.Empty,
                NgayChieu = Convert.ToDateTime(row["NgayChieu"]),
                GioBatDau = gioBatDau,
                GioKetThuc = gioKetThuc,
                MaPhong = row["MaPhong"]?.ToString() ?? string.Empty,
                MaPhim = row["MaPhim"]?.ToString() ?? string.Empty
            };
        }

        public async Task<IEnumerable<SuatChieu>> GetByRoomIdAsync(string maPhong)
        {
            string query = "SELECT * FROM QuanLyRapPhim.SUATCHIEU WHERE MaPhong = @mp";
            DbParameter[] parameters = new DbParameter[]
            {
                new MySqlParameter("@mp", maPhong)
            };
            var dt = await _dbService.ExecuteQueryAsync(query, parameters);

            var dsSuatChieu = new List<SuatChieu>();

            foreach (DataRow row in dt.Rows)
            {
                TimeSpan gioBatDau = row["GioBatDau"] is TimeSpan tsBatDau
                    ? tsBatDau
                    : TimeSpan.Parse(row["GioBatDau"].ToString() ?? "00:00:00");

                TimeSpan gioKetThuc = row["GioKetThuc"] is TimeSpan tsKetThuc
                    ? tsKetThuc
                    : TimeSpan.Parse(row["GioKetThuc"].ToString() ?? "00:00:00");

                dsSuatChieu.Add(new SuatChieu
                {
                    MaSuatChieu = row["MaSuatChieu"].ToString() ?? string.Empty,
                    NgayChieu = Convert.ToDateTime(row["NgayChieu"]),
                    GioBatDau = gioBatDau,
                    GioKetThuc = gioKetThuc,
                    MaPhong = row["MaPhong"]?.ToString() ?? string.Empty,
                    MaPhim = row["MaPhim"]?.ToString() ?? string.Empty
                });
            }

            return dsSuatChieu;
        }
    }
}