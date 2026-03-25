using Cinema_Management_App.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Cinema_Management_App.Services
{
    public class PhimRepository
    {
        private readonly MySQLService _db;

        public PhimRepository(MySQLService db)
        {
            _db = db;
        }

        // ─── LẤY DANH SÁCH ───────────────────────────────────────────

        public List<Phim> GetAllPhim()
        {
            string query = @"
                SELECT p.MaPhim, p.TenPhim, p.ThoiLuong, p.MaNhanPhim,
                       p.TenDaoDien, p.TenDienVienChinh, p.NgayKhoiChieu
                FROM PHIM p";

            DataTable dt = _db.ExecuteQuery(query);
            var list = new List<Phim>();

            foreach (DataRow row in dt.Rows)
            {
                var phim = MapRowToPhim(row);
                phim.DanhSachMaTheLoai = GetDanhSachMaTheLoai(phim.MaPhim);
                list.Add(phim);
            }

            return list;
        }

        public Phim GetPhimById(int maPhim)
        {
            string query = @"
                SELECT MaPhim, TenPhim, ThoiLuong, MaNhanPhim,
                       TenDaoDien, TenDienVienChinh, NgayKhoiChieu
                FROM PHIM
                WHERE MaPhim = @MaPhim";

            var parameters = new[]
            {
                new MySqlParameter("@MaPhim", maPhim)
            };

            DataTable dt = _db.ExecuteQuery(query, parameters);
            if (dt.Rows.Count == 0) return null;

            var phim = MapRowToPhim(dt.Rows[0]);
            phim.DanhSachMaTheLoai = GetDanhSachMaTheLoai(maPhim);
            return phim;
        }

        private List<int> GetDanhSachMaTheLoai(int maPhim)
        {
            string query = "SELECT MaTheLoai FROM CHITIETTHELOAI WHERE MaPhim = @MaPhim";
            var parameters = new[] { new MySqlParameter("@MaPhim", maPhim) };
            DataTable dt = _db.ExecuteQuery(query, parameters);

            var result = new List<int>();
            foreach (DataRow row in dt.Rows)
                result.Add(Convert.ToInt32(row["MaTheLoai"]));

            return result;
        }

        // ─── THÊM ─────────────────────────────────────────────────────

        public bool AddPhim(Phim phim)
        {
            string query = @"
                INSERT INTO PHIM (TenPhim, ThoiLuong, MaNhanPhim, TenDaoDien, TenDienVienChinh, NgayKhoiChieu)
                VALUES (@TenPhim, @ThoiLuong, @MaNhanPhim, @TenDaoDien, @TenDienVienChinh, @NgayKhoiChieu);
                SELECT LAST_INSERT_ID();";

            var parameters = BuildPhimParameters(phim);
            DataTable dt = _db.ExecuteQuery(query, parameters);

            if (dt.Rows.Count == 0) return false;

            int newId = Convert.ToInt32(dt.Rows[0][0]);
            phim.MaPhim = newId;

            return SaveTheLoai(phim.MaPhim, phim.DanhSachMaTheLoai);
        }

        // ─── SỬA ──────────────────────────────────────────────────────

        public bool UpdatePhim(Phim phim)
        {
            string query = @"
                UPDATE PHIM
                SET TenPhim          = @TenPhim,
                    ThoiLuong        = @ThoiLuong,
                    MaNhanPhim       = @MaNhanPhim,
                    TenDaoDien       = @TenDaoDien,
                    TenDienVienChinh = @TenDienVienChinh,
                    NgayKhoiChieu    = @NgayKhoiChieu
                WHERE MaPhim = @MaPhim";

            var parameters = BuildPhimParameters(phim, includeId: true);
            int affected = _db.ExecuteNonQuery(query, parameters);

            if (affected == 0) return false;

            // Xóa thể loại cũ rồi lưu lại
            DeleteTheLoai(phim.MaPhim);
            return SaveTheLoai(phim.MaPhim, phim.DanhSachMaTheLoai);
        }

        // ─── XÓA ──────────────────────────────────────────────────────

        public bool DeletePhim(int maPhim)
        {
            DeleteTheLoai(maPhim); // Xóa liên kết thể loại trước

            string query = "DELETE FROM PHIM WHERE MaPhim = @MaPhim";
            var parameters = new[] { new MySqlParameter("@MaPhim", maPhim) };
            return _db.ExecuteNonQuery(query, parameters) > 0;
        }

        // ─── HELPER ───────────────────────────────────────────────────

        private bool SaveTheLoai(int maPhim, List<int> danhSachMaTheLoai)
        {
            if (danhSachMaTheLoai == null || danhSachMaTheLoai.Count == 0)
                return true;

            var sb = new System.Text.StringBuilder();
            sb.Append("INSERT INTO CHITIETTHELOAI (MaPhim, MaTheLoai) VALUES ");

            var parameters = new List<MySqlParameter>();
            for (int i = 0; i < danhSachMaTheLoai.Count; i++)
            {
                sb.Append(i > 0 ? ", " : "");
                sb.Append($"(@MaPhim{i}, @MaTheLoai{i})");
                parameters.Add(new MySqlParameter($"@MaPhim{i}", maPhim));
                parameters.Add(new MySqlParameter($"@MaTheLoai{i}", danhSachMaTheLoai[i]));
            }

            return _db.ExecuteNonQuery(sb.ToString(), parameters.ToArray()) > 0;
        }

        private void DeleteTheLoai(int maPhim)
        {
            string query = "DELETE FROM CHITIETTHELOAI WHERE MaPhim = @MaPhim";
            var parameters = new[] { new MySqlParameter("@MaPhim", maPhim) };
            _db.ExecuteNonQuery(query, parameters);
        }

        private static Phim MapRowToPhim(DataRow row)
        {
            return new Phim
            {
                MaPhim = Convert.ToInt32(row["MaPhim"]),
                TenPhim = row["TenPhim"].ToString(),
                ThoiLuong = Convert.ToInt32(row["ThoiLuong"]),
                MaNhanPhim = Convert.ToInt32(row["MaNhanPhim"]),
                TenDaoDien = row["TenDaoDien"].ToString(),
                TenDienVienChinh = row["TenDienVienChinh"].ToString(),
                NgayKhoiChieu = Convert.ToDateTime(row["NgayKhoiChieu"])
            };
        }

        private static MySqlParameter[] BuildPhimParameters(Phim phim, bool includeId = false)
        {
            var list = new List<MySqlParameter>
            {
                new MySqlParameter("@TenPhim",          phim.TenPhim),
                new MySqlParameter("@ThoiLuong",        phim.ThoiLuong),
                new MySqlParameter("@MaNhanPhim",       phim.MaNhanPhim),
                new MySqlParameter("@TenDaoDien",       phim.TenDaoDien),
                new MySqlParameter("@TenDienVienChinh", phim.TenDienVienChinh),
                new MySqlParameter("@NgayKhoiChieu",    phim.NgayKhoiChieu)
            };

            if (includeId)
                list.Add(new MySqlParameter("@MaPhim", phim.MaPhim));

            return list.ToArray();
        }
    }
}