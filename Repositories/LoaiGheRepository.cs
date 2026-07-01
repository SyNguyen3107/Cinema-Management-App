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
            string query = "SELECT * FROM QuanLyRapPhim.LOAIGHE";

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
                             FROM QuanLyRapPhim.LOAIGHE lg
                             JOIN QuanLyRapPhim.QUYDINH_LOAIGHE qdlg ON lg.MaLoaiGhe = qdlg.MaLoaiGhe
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

        public async Task<LoaiGhe?> GetByIdAsync(string maLoaiGhe)
        {
            string query = "SELECT * FROM QuanLyRapPhim.LOAIGHE WHERE MaLoaiGhe = @mlg";
            DbParameter[] parameters = new DbParameter[]
            {
                new MySqlParameter("@mlg", maLoaiGhe)
            };

            var dt = await _dbService.ExecuteQueryAsync(query, parameters);

            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];
            return new LoaiGhe
            {
                MaLoaiGhe = row["MaLoaiGhe"].ToString() ?? string.Empty,
                TenLoaiGhe = row["TenLoaiGhe"].ToString() ?? string.Empty,
                DonGia= Convert.ToDecimal(row["DonGian"])
            };
        }

        public async Task<bool> KiemTraVaThemQuyDinhLoaiGheAsync(string maLoaiPhong, IEnumerable<string> danhSachMaLoaiGhe)
        {
            if (danhSachMaLoaiGhe == null || !danhSachMaLoaiGhe.Any()) return true;

            var maLoaiGheDistinct = danhSachMaLoaiGhe.Distinct().ToList();

            string query = @"
        INSERT IGNORE INTO QuanLyRapPhim.QUYDINH_LOAIGHE (MaLoaiPhong, MaLoaiGhe) 
        VALUES (@mlp, @mlg);";

            using (var conn = _dbService.CreateConnection())
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = query;

                    cmd.AddParameterWithValue("@mlp", maLoaiPhong);
                    var paramMlg = cmd.CreateParameter();
                    paramMlg.ParameterName = "@mlg";
                    cmd.Parameters.Add(paramMlg);

                    foreach (var maLoaiGhe in maLoaiGheDistinct)
                    {
                        paramMlg.Value = maLoaiGhe;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            return true;
        }
    }
}