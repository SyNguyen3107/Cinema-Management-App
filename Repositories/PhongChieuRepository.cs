using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinema_Management_App.Services;
using MySql.Data.MySqlClient;
using Cinema_Management_App.Viewmodels;

namespace Cinema_Management_App.Repositories
{
    public class PhongChieuRepository
    {
        private readonly MySQLService _db = new MySQLService();

        public bool SaveFullRoom(LapDanhSachPhongChieuViewmodel vm)
        {
            try
            {
                // 1. Lưu Phòng Chiếu
                string queryPC = "INSERT INTO PHONGCHIEU (MaPhong, TenPhong, TenLoaiPhong, TenTinhTrang, GhiChu, TongGiaTri) " +
                                 "VALUES (@ma, @ten, @loai, @tt, @note, @tong)";
                MySqlParameter[] pPC = {
                    new MySqlParameter("@ma", vm.MaPhong),
                    new MySqlParameter("@ten", vm.TenPhong),
                    new MySqlParameter("@loai", vm.ChonLoaiPhong),
                    new MySqlParameter("@tt", vm.ChonTinhTrang),
                    new MySqlParameter("@note", vm.GhiChu),
                    new MySqlParameter("@tong", vm.TongGiaTri)
                };
                _db.ExecuteNonQuery(queryPC, pPC);

                // 2. Lưu danh sách Ghế
                foreach (var ghe in vm.DanhSachGhe)
                {
                    string queryGhe = "INSERT INTO GHE (MaGhe, MaPhong, TenLoaiGhe, DonGia) VALUES (@maghe, @maphong, @loaighe, @gia)";
                    MySqlParameter[] pGhe = {
                        new MySqlParameter("@maghe", ghe.MaGhe),
                        new MySqlParameter("@maphong", vm.MaPhong),
                        new MySqlParameter("@loaighe", ghe.TenLoaiGhe),
                        new MySqlParameter("@gia", ghe.DonGia)
                    };
                    _db.ExecuteNonQuery(queryGhe, pGhe);
                }
                return true;
            }
            catch (Exception) { return false; }
        }
    }
}