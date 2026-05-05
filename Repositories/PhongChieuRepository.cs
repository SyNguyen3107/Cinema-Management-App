using MySql.Data.MySqlClient;
using System.Configuration;
using System.Collections.Generic;
using Cinema_Management_App.Models;
using System;

namespace Cinema_Management_App.Repositories
{
    public class PhongChieuRepository
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["AivenMySQL"].ConnectionString;

        public bool LuuPhongChieu(PhongChieu phong, IEnumerable<Ghe> dsGhe)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        string queryPhong = @"INSERT INTO PHONGCHIEU (TenPhong, MaLoaiPhong, MaTinhTrang, GhiChu, TongGiaTri) 
                                            VALUES (@ten, @loai, @tt, @gc, @tong); SELECT LAST_INSERT_ID();";

                        MySqlCommand cmdPhong = new MySqlCommand(queryPhong, conn, trans);
                        cmdPhong.Parameters.AddWithValue("@ten", phong.TenPhong);
                        cmdPhong.Parameters.AddWithValue("@loai", phong.MaLoaiPhong);
                        cmdPhong.Parameters.AddWithValue("@tt", phong.MaTinhTrang);
                        cmdPhong.Parameters.AddWithValue("@gc", phong.GhiChu);
                        cmdPhong.Parameters.AddWithValue("@tong", phong.TongGiaTri);
                        int newMaPhong = Convert.ToInt32(cmdPhong.ExecuteScalar());
                        foreach (var ghe in dsGhe)
                        {
                            string queryGhe = @"INSERT INTO GHE (MaSoGhe, MaPhong, MaLoaiGhe) 
                                              VALUES (@ms, @mp, @mlg)";
                            MySqlCommand cmdGhe = new MySqlCommand(queryGhe, conn, trans);
                            cmdGhe.Parameters.AddWithValue("@ms", ghe.MaGhe);
                            cmdGhe.Parameters.AddWithValue("@mp", newMaPhong);
                            cmdGhe.Parameters.AddWithValue("@mlg", ghe.TenLoaiGhe);
                            cmdGhe.ExecuteNonQuery();
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
        }
    }
}