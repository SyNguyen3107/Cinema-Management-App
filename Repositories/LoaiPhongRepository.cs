using Cinema_Management_App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinema_Management_App.Models;

namespace Cinema_Management_App.Repositories
{
    public class LoaiPhongRepository
    {
        private readonly MySQLService _dbService;
        public LoaiPhongRepository(MySQLService dbService)
        {
            _dbService = dbService;
        }
        public List<LoaiPhong> GetAllLoaiPhong()
        {
            List<LoaiPhong> danhSach = new List<LoaiPhong>();
            string query = "SELECT * FROM QuanLyPhongChieu.LOAIPHONG";
            var dt = _dbService.ExecuteQuery(query);
            foreach (System.Data.DataRow row in dt.Rows)
            {
                danhSach.Add(new LoaiPhong
                {
                    MaLoaiPhong = row["MaLoaiPhong"].ToString(),
                    TenLoaiPhong = row["TenLoaiPhong"].ToString()
                });
            }
            return danhSach;
        }
    }
}
