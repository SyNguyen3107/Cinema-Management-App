using System;
using System.Collections.Generic;
using Cinema_Management_App.Models;
using Cinema_Management_App.Services;
using MySql.Data.MySqlClient;

namespace Cinema_Management_App.Repositories
{
    public class PhongChieuRepository
    {
        private readonly MySQLService _dbService = new MySQLService();
        public bool AddPhongChieu(PhongChieu phong, IEnumerable<Ghe> dsGhe)
        {
            using (var conn = _dbService.GetConnection())
            {
                try
                {
                    conn.Open();
                    using (var trans = conn.BeginTransaction())
                    {
                        try
                        {
                            string queryPhong = @"INSERT INTO QuanLyPhongChieu.PHONGCHIEU 
                    (MaPhong, TenPhong, MaLoaiPhong, MaTinhTrang, GhiChu) 
                    VALUES (@mp, @ten, @loai, @tt, @gc);";
                            // Đã bỏ SELECT LAST_INSERT_ID() và thêm MaPhong vào đầu

                            using (MySqlCommand cmdPhong = new MySqlCommand(queryPhong, conn, trans))
                            {
                                cmdPhong.Parameters.AddWithValue("@mp", phong.MaPhong);
                                cmdPhong.Parameters.AddWithValue("@ten", phong.TenPhong);
                                cmdPhong.Parameters.AddWithValue("@loai", phong.MaLoaiPhong);
                                cmdPhong.Parameters.AddWithValue("@tt", phong.MaTinhTrang);
                                cmdPhong.Parameters.AddWithValue("@gc", (object)phong.GhiChu ?? DBNull.Value);

                                cmdPhong.ExecuteNonQuery();
                            }
                            string queryGhe = @"
                                INSERT INTO QuanLyPhongChieu.GHE 
                                (MaGhe, MaSoGhe, MaPhong, MaLoaiGhe) 
                                VALUES (@mg, @ms, @mp, @mlg);";

                            using (var cmdGhe = new MySqlCommand(queryGhe, conn, trans))
                            {
                                cmdGhe.Parameters.Add("@mg", MySqlDbType.VarChar);
                                cmdGhe.Parameters.Add("@ms", MySqlDbType.VarChar);
                                cmdGhe.Parameters.Add("@mp", MySqlDbType.VarChar);
                                cmdGhe.Parameters.Add("@mlg", MySqlDbType.VarChar);

                                foreach (var ghe in dsGhe)
                                {
                                    cmdGhe.Parameters["@mg"].Value = ghe.MaGhe;
                                    cmdGhe.Parameters["@ms"].Value = ghe.MaSoGhe;
                                    cmdGhe.Parameters["@mp"].Value = phong.MaPhong;
                                    cmdGhe.Parameters["@mlg"].Value = ghe.MaLoaiGhe;

                                    cmdGhe.ExecuteNonQuery();
                                }
                            }

                            trans.Commit();
                            return true;
                        }
                        catch (Exception)
                        {
                            trans.Rollback();
                            return false;
                        }
                    }
                }
                catch (Exception)
                {
                    // Lỗi mở kết nối
                    return false;
                }
            }
        }
    }
}