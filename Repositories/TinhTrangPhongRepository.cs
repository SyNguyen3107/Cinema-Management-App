using Cinema_Management_App.Models;
using Cinema_Management_App.Services;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema_Management_App.Repositories
{
    public class TinhTrangPhongRepository
    {
        private readonly MySQLService _dbService;

        public TinhTrangPhongRepository(MySQLService dbService)
        {
            _dbService = dbService;
        }

        public List<TinhTrangPhong> GetAllTinhTrangPhong()
        {
            List<TinhTrangPhong> tinhTrangPhongs = new List<TinhTrangPhong>();
            string query = "SELECT MaTinhTrangPhong, TenTinhTrangPhong FROM QuanLyPhongChieu.TINHTRANGPHONG";
            DataTable dt = _dbService.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                TinhTrangPhong tinhTrangPhong = new TinhTrangPhong
                {
                    MaTinhTrang = row["MaTinhTrangPhong"].ToString(),
                    TenTinhTrang = row["TenTinhTrangPhong"].ToString()
                };
                tinhTrangPhongs.Add(tinhTrangPhong);
            }
            return tinhTrangPhongs;
        }
    }
}
