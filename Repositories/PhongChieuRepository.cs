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
        public bool LuuPhongChieu(PhongChieu phong, IEnumerable<Ghe> dsGhe)
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
                            string queryPhong = @"INSERT INTO PHONGCHIEU (TenPhong, MaLoaiPhong, MaTinhTrang, GhiChu, TongGiaTri) 
                                                VALUES (@ten, @loai, @tt, @gc, @tong); 
                                                SELECT LAST_INSERT_ID();";

                            int newMaPhong;
                            using (MySqlCommand cmdPhong = new MySqlCommand(queryPhong, conn, trans))
                            {
                                cmdPhong.Parameters.AddWithValue("@ten", phong.TenPhong);
                                cmdPhong.Parameters.AddWithValue("@loai", phong.MaLoaiPhong);
                                cmdPhong.Parameters.AddWithValue("@tt", phong.MaTinhTrang);
                                cmdPhong.Parameters.AddWithValue("@gc", (object)phong.GhiChu ?? DBNull.Value);
                                cmdPhong.Parameters.AddWithValue("@tong", phong.TongGiaTri);

                                newMaPhong = Convert.ToInt32(cmdPhong.ExecuteScalar());
                            }
                            string queryGhe = @"INSERT INTO GHE (MaSoGhe, MaPhong, MaLoaiGhe) VALUES (@ms, @mp, @mlg)";
                            using (MySqlCommand cmdGhe = new MySqlCommand(queryGhe, conn, trans))
                            {
                                cmdGhe.Parameters.Add("@ms", MySqlDbType.VarChar);
                                cmdGhe.Parameters.Add("@mp", MySqlDbType.Int32);
                                cmdGhe.Parameters.Add("@mlg", MySqlDbType.VarChar);

                                foreach (var ghe in dsGhe)
                                {
                                    cmdGhe.Parameters["@ms"].Value = ghe.MaGhe;
                                    cmdGhe.Parameters["@mp"].Value = newMaPhong;
                                    cmdGhe.Parameters["@mlg"].Value = ghe.TenLoaiGhe;
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