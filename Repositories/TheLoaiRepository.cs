using Cinema_Management_App.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace Cinema_Management_App.Services
{
    public class TheLoaiRepository
    {
        private readonly MySQLService _db;

        public TheLoaiRepository(MySQLService db)
        {
            _db = db;
        }

        // ─── LẤY DANH SÁCH ───────────────────────────────────────────

        public List<TheLoai> GetAllTheLoai()
        {
            string query = "SELECT MaTheLoai, TenTheLoai FROM TheLoai ORDER BY TenTheLoai";
            DataTable dt = _db.ExecuteQuery(query);

            var list = new List<TheLoai>();
            foreach (DataRow row in dt.Rows)
                list.Add(MapRowToTheLoai(row));

            return list;
        }

        public TheLoai GetTheLoaiById(int maTheLoai)
        {
            string query = "SELECT MaTheLoai, TenTheLoai FROM TheLoai WHERE MaTheLoai = @MaTheLoai";
            var parameters = new[] { new MySqlParameter("@MaTheLoai", maTheLoai) };

            DataTable dt = _db.ExecuteQuery(query, parameters);
            return dt.Rows.Count == 0 ? null : MapRowToTheLoai(dt.Rows[0]);
        }

        // ─── THÊM ─────────────────────────────────────────────────────

        public bool AddTheLoai(TheLoai theLoai)
        {
            string query = "INSERT INTO TheLoai (TenTheLoai) VALUES (@TenTheLoai)";
            var parameters = new[] { new MySqlParameter("@TenTheLoai", theLoai.TenTheLoai) };
            return _db.ExecuteNonQuery(query, parameters) > 0;
        }

        // ─── SỬA ──────────────────────────────────────────────────────

        public bool UpdateTheLoai(TheLoai theLoai)
        {
            string query = @"
                UPDATE TheLoai
                SET TenTheLoai = @TenTheLoai
                WHERE MaTheLoai = @MaTheLoai";

            var parameters = new[]
            {
                new MySqlParameter("@TenTheLoai", theLoai.TenTheLoai),
                new MySqlParameter("@MaTheLoai",  theLoai.MaTheLoai)
            };

            return _db.ExecuteNonQuery(query, parameters) > 0;
        }

        // ─── XÓA ──────────────────────────────────────────────────────

        public bool DeleteTheLoai(int maTheLoai)
        {
            // Kiểm tra thể loại có đang được dùng bởi phim nào không
            string checkQuery = "SELECT COUNT(*) FROM PhimTheLoai WHERE MaTheLoai = @MaTheLoai";
            var checkParams = new[] { new MySqlParameter("@MaTheLoai", maTheLoai) };
            DataTable dt = _db.ExecuteQuery(checkQuery, checkParams);

            int soPhimDangDung = Convert.ToInt32(dt.Rows[0][0]);
            if (soPhimDangDung > 0) return false; // Không xóa nếu đang được dùng

            string query = "DELETE FROM TheLoai WHERE MaTheLoai = @MaTheLoai";
            var parameters = new[] { new MySqlParameter("@MaTheLoai", maTheLoai) };
            return _db.ExecuteNonQuery(query, parameters) > 0;
        }

        // ─── HELPER ───────────────────────────────────────────────────

        private static TheLoai MapRowToTheLoai(DataRow row)
        {
            return new TheLoai
            {
                MaTheLoai = Convert.ToInt32(row["MaTheLoai"]),
                TenTheLoai = row["TenTheLoai"].ToString()
            };
        }
    }
}