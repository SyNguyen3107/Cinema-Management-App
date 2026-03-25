using Cinema_Management_App.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace Cinema_Management_App.Services
{
    public class NhanPhimRepository
    {
        private readonly MySQLService _db;

        public NhanPhimRepository(MySQLService db)
        {
            _db = db;
        }

        // ─── LẤY DANH SÁCH ───────────────────────────────────────────

        public List<NhanPhim> GetAllNhanPhim()
        {
            string query = "SELECT MaNhanPhim, TenNhanPhim FROM NHANPHIM ORDER BY TenNhanPhim";
            DataTable dt = _db.ExecuteQuery(query);

            var list = new List<NhanPhim>();
            foreach (DataRow row in dt.Rows)
                list.Add(MapRowToNhanPhim(row));

            return list;
        }

        public NhanPhim GetNhanPhimById(int maNhanPhim)
        {
            string query = "SELECT MaNhanPhim, TenNhanPhim FROM NHANPHIM WHERE MaNhanPhim = @MaNhanPhim";
            var parameters = new[] { new MySqlParameter("@MaNhanPhim", maNhanPhim) };

            DataTable dt = _db.ExecuteQuery(query, parameters);
            return dt.Rows.Count == 0 ? null : MapRowToNhanPhim(dt.Rows[0]);
        }

        // ─── THÊM ─────────────────────────────────────────────────────

        public bool AddNhanPhim(NhanPhim nhanPhim)
        {
            string query = "INSERT INTO NHANPHIM (TenNhanPhim) VALUES (@TenNhanPhim)";
            var parameters = new[] { new MySqlParameter("@TenNhanPhim", nhanPhim.TenNhanPhim) };
            return _db.ExecuteNonQuery(query, parameters) > 0;
        }

        // ─── SỬA ──────────────────────────────────────────────────────

        public bool UpdateNhanPhim(NhanPhim nhanPhim)
        {
            string query = @"
                UPDATE NHANPHIM
                SET TenNhanPhim = @TenNhanPhim
                WHERE MaNhanPhim = @MaNhanPhim";

            var parameters = new[]
            {
                new MySqlParameter("@TenNhanPhim", nhanPhim.TenNhanPhim),
                new MySqlParameter("@MaNhanPhim",  nhanPhim.MaNhanPhim)
            };

            return _db.ExecuteNonQuery(query, parameters) > 0;
        }

        // ─── XÓA ──────────────────────────────────────────────────────

        public bool DeleteNhanPhim(int maNhanPhim)
        {
            // Kiểm tra nhãn phim có phim nào đang dùng không
            string checkQuery = "SELECT COUNT(*) FROM PHIM WHERE MaNhanPhim = @MaNhanPhim";
            var checkParams = new[] { new MySqlParameter("@MaNhanPhim", maNhanPhim) };
            DataTable dt = _db.ExecuteQuery(checkQuery, checkParams);

            int soPhimDangDung = Convert.ToInt32(dt.Rows[0][0]);
            if (soPhimDangDung > 0) return false; // Không xóa nếu đang được dùng

            string query = "DELETE FROM NHANPHIM WHERE MaNhanPhim = @MaNhanPhim";
            var parameters = new[] { new MySqlParameter("@MaNhanPhim", maNhanPhim) };
            return _db.ExecuteNonQuery(query, parameters) > 0;
        }

        // ─── HELPER ───────────────────────────────────────────────────

        private static NhanPhim MapRowToNhanPhim(DataRow row)
        {
            return new NhanPhim
            {
                MaNhanPhim = Convert.ToInt32(row["MaNhanPhim"]),
                TenNhanPhim = row["TenNhanPhim"].ToString()
            };
        }
    }
}