using Cinema_Management_App.Models;
using MySql.Data.MySqlClient;
using System.Data;

namespace Cinema_Management_App.Services
{
    public class ThamSoRepository
    {
        private readonly MySQLService _db;

        public ThamSoRepository(MySQLService db)
        {
            _db = db;
        }

        public ThamSo GetThamSo()
        {
            string query = "SELECT MaThamSo, SoLuongTheLoaiToiDa FROM THAMSO LIMIT 1";
            DataTable dt = _db.ExecuteQuery(query);

            if (dt.Rows.Count == 0) return null;

            return new ThamSo
            {
                MaThamSo = Convert.ToInt32(dt.Rows[0]["MaThamSo"]),
                SoLuongTheLoaiToiDa = Convert.ToInt32(dt.Rows[0]["SoLuongTheLoaiToiDa"])
            };
        }

        public bool InsertThamSo(ThamSo thamSo)
        {
            string query = "INSERT INTO THAMSO (SoLuongTheLoaiToiDa) VALUES (@SoLuongTheLoaiToiDa)";
            var parameters = new[]
            {
                new MySqlParameter("@SoLuongTheLoaiToiDa", thamSo.SoLuongTheLoaiToiDa)
            };
            return _db.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool UpdateThamSo(ThamSo thamSo)
        {
            string query = @"
                UPDATE THAMSO
                SET SoLuongTheLoaiToiDa = @SoLuongTheLoaiToiDa
                WHERE MaThamSo = @MaThamSo";

            var parameters = new[]
            {
                new MySqlParameter("@SoLuongTheLoaiToiDa", thamSo.SoLuongTheLoaiToiDa),
                new MySqlParameter("@MaThamSo",            thamSo.MaThamSo)
            };

            return _db.ExecuteNonQuery(query, parameters) > 0;
        }
    }
}