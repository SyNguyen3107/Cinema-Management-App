using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinema_Management_App.Models;
using Cinema_Management_App.Services;

namespace Cinema_Management_App.Repositories
{
    public class LoaiGheRepository
    {
        private readonly MySQLService _dbService;
        public LoaiGheRepository(MySQLService dbService)
        {
            _dbService = dbService;
        }
        public List<LoaiGhe> GetAllLoaiGhe()
        {
            List<LoaiGhe> danhSach = new List<LoaiGhe>();
            string query = "SELECT * FROM QuanLyPhongChieu.LOAIGHE";
            var dt = _dbService.ExecuteQuery(query);
            foreach (System.Data.DataRow row in dt.Rows)
            {
                danhSach.Add(new LoaiGhe
                {
                    MaLoaiGhe = row["MaLoaiGhe"].ToString(),
                    TenLoaiGhe = row["TenLoaiGhe"].ToString(),
                    DonGia = Convert.ToDecimal(row["DonGia"])
                });
            }
            return danhSach;
        }
        public List<LoaiGhe> GetAllLoaiGheByMaLoaiPhong(string maLoaiPhong)
        {
            List<LoaiGhe> danhSach = new List<LoaiGhe>();
            string query = @"SELECT lg.MaLoaiGhe, lg.TenLoaiGhe, lg.DonGia 
                             FROM QuanLyPhongChieu.LOAIGHE lg
                             JOIN QuanLyPhongChieu.QUYDINH_LOAIGHE qdlg ON lg.MaLoaiGhe = qdlg.MaLoaiGhe
                             WHERE qdlg.MaLoaiPhong = @maLoaiPhong";
            var parameters = new MySql.Data.MySqlClient.MySqlParameter[]
            {
                new MySql.Data.MySqlClient.MySqlParameter("@maLoaiPhong", maLoaiPhong)
            };
            var dt = _dbService.ExecuteQuery(query, parameters);
            foreach (System.Data.DataRow row in dt.Rows)
            {
                danhSach.Add(new LoaiGhe
                {
                    MaLoaiGhe = row["MaLoaiGhe"].ToString(),
                    TenLoaiGhe = row["TenLoaiGhe"].ToString(),
                    DonGia = Convert.ToDecimal(row["DonGia"])
                });
            }
            return danhSach;
        }
    }
}
