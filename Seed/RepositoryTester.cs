using Cinema_Management_App.Models;
using Cinema_Management_App.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema_Management_App.Seed
{
    public static class RepositoryTester
    {
        // Helper: ghi ra cả Debug output (Visual Studio) lẫn Console (dotnet run)
        private static void Log(string msg)
        {
            Debug.WriteLine(msg);
            Console.WriteLine(msg);
        }

        /// <summary>
        /// Hàm này chạy tất cả các thao tác CRUD cơ bản để test toàn bộ các hàm trong Repositories.
        /// Chạy hàm này ở App.xaml.cs hoặc MainWindow.xaml.cs để kiểm tra.
        /// (Nên xem kết quả ở cửa sổ Output/Debug trong Visual Studio hoặc Console khi dotnet run).
        /// </summary>
        public static void RunAllTests()
        {
            try
            {
                MySQLService db = new MySQLService();
                
                if (!db.TestConnection())
                {
                    Log("=== KẾT NỐI DB THẤT BẠI ===");
                    return;
                }
                Log("=== KẾT NỐI DB THÀNH CÔNG ===");

                NhanPhimRepository nhanPhimRepo = new NhanPhimRepository(db);
                TheLoaiRepository theLoaiRepo = new TheLoaiRepository(db);
                PhimRepository phimRepo = new PhimRepository(db);
                ThamSoRepository thamSoRepo = new ThamSoRepository(db);

                // --- 1. TEST ThamSoRepository ---
                Log("\n--- TEST THAM SỐ ---");
                ThamSo ts = thamSoRepo.GetThamSo();
                if (ts != null)
                {
                    Log($"Tham Số hiện tại: MaThamSo={ts.MaThamSo}, SoLuongTheLoaiToiDa={ts.SoLuongTheLoaiToiDa}");
                    ts.SoLuongTheLoaiToiDa = ts.SoLuongTheLoaiToiDa == 5 ? 4 : 5; // thay đổi để test update
                    bool updateTs = thamSoRepo.UpdateThamSo(ts);
                    Log($"Cập nhật Tham Số: {(updateTs ? "THÀNH CÔNG" : "THẤT BẠI")}");
                }
                else
                {
                    Log("Không tìm thấy Tham Số nào trong DB.");
                }

                // --- 2. TEST NhanPhimRepository ---
                Log("\n--- TEST NHÃN PHIM ---");
                NhanPhim newNhanPhim = new NhanPhim { TenNhanPhim = "Nhãn Phim Test " + DateTime.Now.Ticks };
                bool addNp = nhanPhimRepo.AddNhanPhim(newNhanPhim);
                Log($"Thêm Nhãn Phim: {(addNp ? "THÀNH CÔNG" : "THẤT BẠI")}");

                var allNhanPhim = nhanPhimRepo.GetAllNhanPhim();
                var insertedNhanPhim = allNhanPhim.LastOrDefault(); // Lấy phần tử vừa được insert để lấy id sinh ra
                if (insertedNhanPhim != null)
                {
                    Log($"Lấy tất cả Nhãn Phim: OK, tổng số = {allNhanPhim.Count}");
                    
                    var fetchedNp = nhanPhimRepo.GetNhanPhimById(insertedNhanPhim.MaNhanPhim);
                    Log($"Lấy Nhãn Phim theo ID ({insertedNhanPhim.MaNhanPhim}): {(fetchedNp != null ? "THÀNH CÔNG" : "THẤT BẠI")}");

                    if (fetchedNp != null)
                    {
                         fetchedNp.TenNhanPhim += " (Updated)";
                         bool updateNp = nhanPhimRepo.UpdateNhanPhim(fetchedNp);
                         Log($"Cập nhật Nhãn Phim: {(updateNp ? "THÀNH CÔNG" : "THẤT BẠI")}");
                    }
                }

                // --- 3. TEST TheLoaiRepository ---
                Log("\n--- TEST THỂ LOẠI ---");
                TheLoai newTheLoai = new TheLoai { TenTheLoai = "Thể Loại Test " + DateTime.Now.Ticks };
                bool addTl = theLoaiRepo.AddTheLoai(newTheLoai);
                Log($"Thêm Thể Loại: {(addTl ? "THÀNH CÔNG" : "THẤT BẠI")}");

                var allTheLoai = theLoaiRepo.GetAllTheLoai();
                var insertedTheLoai = allTheLoai.LastOrDefault();
                if (insertedTheLoai != null)
                {
                    Log($"Lấy tất cả Thể Loại: OK, tổng số = {allTheLoai.Count}");

                    var fetchedTl = theLoaiRepo.GetTheLoaiById(insertedTheLoai.MaTheLoai);
                    Log($"Lấy Thể Loại theo ID ({insertedTheLoai.MaTheLoai}): {(fetchedTl != null ? "THÀNH CÔNG" : "THẤT BẠI")}");

                    if (fetchedTl != null)
                    {
                         fetchedTl.TenTheLoai += " (Updated)";
                         bool updateTl = theLoaiRepo.UpdateTheLoai(fetchedTl);
                         Log($"Cập nhật Thể Loại: {(updateTl ? "THÀNH CÔNG" : "THẤT BẠI")}");
                    }
                }

                // --- 4. TEST PhimRepository ---
                Log("\n--- TEST PHIM ---");
                if (insertedNhanPhim != null && insertedTheLoai != null)
                {
                    Phim newPhim = new Phim
                    {
                        TenPhim = "Phim Test " + DateTime.Now.Ticks,
                        ThoiLuong = 120,
                        MaNhanPhim = insertedNhanPhim.MaNhanPhim,
                        TenDaoDien = "Đạo Diễn Test",
                        TenDienVienChinh = "Diễn Viên Test",
                        NgayKhoiChieu = DateTime.Now.Date,
                        DanhSachMaTheLoai = new List<int> { insertedTheLoai.MaTheLoai }
                    };

                    bool addPhim = phimRepo.AddPhim(newPhim);
                    Log($"Thêm Phim (mã sinh tự động = {newPhim.MaPhim}): {(addPhim ? "THÀNH CÔNG" : "THẤT BẠI")}");

                    var allPhim = phimRepo.GetAllPhim();
                    Log($"Lấy tất cả Phim: OK, tổng số = {allPhim.Count}");

                    if (newPhim.MaPhim > 0)
                    {
                        var fetchedPhim = phimRepo.GetPhimById(newPhim.MaPhim);
                        Log($"Lấy Phim theo ID ({newPhim.MaPhim}): {(fetchedPhim != null ? "THÀNH CÔNG" : "THẤT BẠI")}");

                        if (fetchedPhim != null)
                        {
                            fetchedPhim.TenPhim += " (Updated)";
                            fetchedPhim.ThoiLuong = 150;
                            bool updatePhim = phimRepo.UpdatePhim(fetchedPhim);
                            Log($"Cập nhật Phim: {(updatePhim ? "THÀNH CÔNG" : "THẤT BẠI")}");
                        }

                        // Xóa các dữ liệu Test để không làm rác DB (Delete theo thứ tự Phim -> NhanPhim, TheLoai)
                        Log("\n--- CLEANUP DỮ LIỆU TEST ---");
                        bool deletePhim = phimRepo.DeletePhim(newPhim.MaPhim);
                        Log($"Xóa Phim ({newPhim.MaPhim}): {(deletePhim ? "THÀNH CÔNG" : "THẤT BẠI")}");
                    }
                }
                else
                {
                    Log("Bỏ qua Test Phim vì không tạo được Nhãn Phim hoặc Thể Loại.");
                }

                // Cleanup Thể loại và Nhãn phim
                if (insertedNhanPhim != null)
                {
                    bool deleteNp = nhanPhimRepo.DeleteNhanPhim(insertedNhanPhim.MaNhanPhim);
                    Log($"Xóa Nhãn Phim ({insertedNhanPhim.MaNhanPhim}): {(deleteNp ? "THÀNH CÔNG" : "THẤT BẠI")}");
                }

                if (insertedTheLoai != null)
                {
                    bool deleteTl = theLoaiRepo.DeleteTheLoai(insertedTheLoai.MaTheLoai);
                    Log($"Xóa Thể Loại ({insertedTheLoai.MaTheLoai}): {(deleteTl ? "THÀNH CÔNG" : "THẤT BẠI")}");
                }

                Log("\n=== HOÀN TẤT KIỂM TRA TẤT CẢ REPOSITORIES ===");
            }
            catch (Exception ex)
            {
                Log($"LỖI TRONG QUÁ TRÌNH TEST: {ex.Message}");
                Log(ex.StackTrace);
            }
        }

        /// <summary>
        /// Seed dữ liệu mẫu vào DB nếu chưa có.
        /// Kiểm tra trùng lặp trước khi insert để có thể chạy lại nhiều lần mà không bị duplicate.
        /// </summary>
        public static void SeedData()
        {
            try
            {
                MySQLService db = new MySQLService();
                if (!db.TestConnection())
                {
                    Log("=== KẾT NỐI DB THẤT BẠI - BỎ QUA SEED ===");
                    return;
                }
                Log("=== BẮT ĐẦU SEED DỮ LIỆU ===");

                NhanPhimRepository nhanPhimRepo = new NhanPhimRepository(db);
                TheLoaiRepository theLoaiRepo = new TheLoaiRepository(db);
                PhimRepository phimRepo = new PhimRepository(db);
                ThamSoRepository thamSoRepo = new ThamSoRepository(db);

                // ── 1. Seed Tham Số ────────────────────────────────────────
                Log("\n[1/4] Kiểm tra và seed Tham Số...");
                ThamSo ts = thamSoRepo.GetThamSo();
                if (ts == null)
                {
                    bool inserted = thamSoRepo.InsertThamSo(new ThamSo { SoLuongTheLoaiToiDa = 5 });
                    Log($"  Tạo mới Tham Số: {(inserted ? "THÀNH CÔNG" : "THẤT BẠI")}");
                }
                else
                {
                    Log($"  Tham Số đã tồn tại (MaThamSo={ts.MaThamSo}, SoLuongTheLoaiToiDa={ts.SoLuongTheLoaiToiDa}) - bỏ qua.");
                }

                // ── 2. Seed Nhãn Phim ─────────────────────────────────────
                Log("\n[2/4] Seed Nhãn Phim...");
                var existingNp = nhanPhimRepo.GetAllNhanPhim();
                var tenNhanPhimDaSeed = new HashSet<string>(existingNp.Select(x => x.TenNhanPhim), StringComparer.OrdinalIgnoreCase);

                var nhanPhimData = new List<NhanPhim>
                {
                    new NhanPhim { TenNhanPhim = "P (Mọi lứa tuổi)" },
                    new NhanPhim { TenNhanPhim = "K (Dưới 13 tuổi với người giám hộ)" },
                    new NhanPhim { TenNhanPhim = "T13 (Dưới 13 tuổi)" },
                    new NhanPhim { TenNhanPhim = "T16 (Dưới 16 tuổi)" },
                    new NhanPhim { TenNhanPhim = "T18 (Dưới 18 tuổi)" }
                };

                int addedNp = 0;
                foreach (var np in nhanPhimData)
                {
                    if (!tenNhanPhimDaSeed.Contains(np.TenNhanPhim))
                    {
                        nhanPhimRepo.AddNhanPhim(np);
                        addedNp++;
                        Log($"  + Thêm: {np.TenNhanPhim}");
                    }
                    else
                    {
                        Log($"  ~ Bỏ qua (đã có): {np.TenNhanPhim}");
                    }
                }
                Log($"  => Đã thêm {addedNp}/{nhanPhimData.Count} nhãn phim.");

                // ── 3. Seed Thể Loại ──────────────────────────────────────
                Log("\n[3/4] Seed Thể Loại...");
                var existingTl = theLoaiRepo.GetAllTheLoai();
                var tenTheLoaiDaSeed = new HashSet<string>(existingTl.Select(x => x.TenTheLoai), StringComparer.OrdinalIgnoreCase);

                var theLoaiData = new List<TheLoai>
                {
                    new TheLoai { TenTheLoai = "Hành động" },
                    new TheLoai { TenTheLoai = "Hài kịch" },
                    new TheLoai { TenTheLoai = "Kinh dị" },
                    new TheLoai { TenTheLoai = "Tâm lý" },
                    new TheLoai { TenTheLoai = "Viễn tưởng" },
                    new TheLoai { TenTheLoai = "Hoạt hình" },
                    new TheLoai { TenTheLoai = "Lãng mạn" }
                };

                int addedTl = 0;
                foreach (var tl in theLoaiData)
                {
                    if (!tenTheLoaiDaSeed.Contains(tl.TenTheLoai))
                    {
                        theLoaiRepo.AddTheLoai(tl);
                        addedTl++;
                        Log($"  + Thêm: {tl.TenTheLoai}");
                    }
                    else
                    {
                        Log($"  ~ Bỏ qua (đã có): {tl.TenTheLoai}");
                    }
                }
                Log($"  => Đã thêm {addedTl}/{theLoaiData.Count} thể loại.");

                // ── 4. Seed Phim Mẫu ──────────────────────────────────────
                Log("\n[4/4] Seed Phim Mẫu...");
                var allNp  = nhanPhimRepo.GetAllNhanPhim();
                var allTl  = theLoaiRepo.GetAllTheLoai();
                var allPhim = phimRepo.GetAllPhim();
                var tenPhimDaSeed = new HashSet<string>(allPhim.Select(x => x.TenPhim), StringComparer.OrdinalIgnoreCase);

                if (allNp.Count == 0 || allTl.Count < 2)
                {
                    Log("  Không đủ Nhãn Phim hoặc Thể Loại. Bỏ qua seed phim.");
                }
                else
                {
                    // Tra cứu ID theo tên để seed đúng bất kể thứ tự AUTO_INCREMENT
                    Func<string, int> maNhanPhim = ten => allNp.FirstOrDefault(x => x.TenNhanPhim == ten)?.MaNhanPhim ?? allNp[0].MaNhanPhim;
                    Func<string, int> maTheLoai  = ten => allTl.FirstOrDefault(x => x.TenTheLoai == ten)?.MaTheLoai  ?? allTl[0].MaTheLoai;

                    var phimData = new List<Phim>
                    {
                        new Phim
                        {
                            TenPhim          = "Avengers: Endgame",
                            ThoiLuong        = 181,
                            MaNhanPhim       = maNhanPhim("P (Mọi lứa tuổi)"),
                            TenDaoDien       = "Anthony Russo, Joe Russo",
                            TenDienVienChinh = "Robert Downey Jr., Chris Evans",
                            NgayKhoiChieu    = new DateTime(2019, 4, 26),
                            DanhSachMaTheLoai = new List<int> { maTheLoai("Hành động"), maTheLoai("Viễn tưởng") }
                        },
                        new Phim
                        {
                            TenPhim          = "The Conjuring",
                            ThoiLuong        = 112,
                            MaNhanPhim       = maNhanPhim("T18 (Dưới 18 tuổi)"),
                            TenDaoDien       = "James Wan",
                            TenDienVienChinh = "Vera Farmiga, Patrick Wilson",
                            NgayKhoiChieu    = new DateTime(2013, 7, 19),
                            DanhSachMaTheLoai = new List<int> { maTheLoai("Kinh dị") }
                        },
                        new Phim
                        {
                            TenPhim          = "Your Name (Kimi no Na wa)",
                            ThoiLuong        = 107,
                            MaNhanPhim       = maNhanPhim("P (Mọi lứa tuổi)"),
                            TenDaoDien       = "Makoto Shinkai",
                            TenDienVienChinh = "Ryunosuke Kamiki, Mone Kamishiraishi",
                            NgayKhoiChieu    = new DateTime(2016, 8, 26),
                            DanhSachMaTheLoai = new List<int> { maTheLoai("Hoạt hình"), maTheLoai("Lãng mạn") }
                        },
                        new Phim
                        {
                            TenPhim          = "Parasite (Ký sinh trùng)",
                            ThoiLuong        = 132,
                            MaNhanPhim       = maNhanPhim("T18 (Dưới 18 tuổi)"),
                            TenDaoDien       = "Bong Joon-ho",
                            TenDienVienChinh = "Song Kang-ho, Lee Sun-kyun",
                            NgayKhoiChieu    = new DateTime(2019, 5, 30),
                            DanhSachMaTheLoai = new List<int> { maTheLoai("Tâm lý"), maTheLoai("Hài kịch") }
                        }
                    };

                    int addedPhim = 0;
                    foreach (var phim in phimData)
                    {
                        if (!tenPhimDaSeed.Contains(phim.TenPhim))
                        {
                            phimRepo.AddPhim(phim);
                            addedPhim++;
                            Log($"  + Thêm: {phim.TenPhim} (MaPhim={phim.MaPhim})");
                        }
                        else
                        {
                            Log($"  ~ Bỏ qua (đã có): {phim.TenPhim}");
                        }
                    }
                    Log($"  => Đã thêm {addedPhim}/{phimData.Count} phim.");
                }

                Log("\n=== SEED DỮ LIỆU HOÀN TẤT ===");
            }
            catch (Exception ex)
            {
                Log($"LỖI SEED DỮ LIỆU: {ex.Message}");
                Log(ex.StackTrace);
            }
        }

        public static async Task SeedDataAsync()
        {
            await Task.Run(() => SeedData());
        }

        public static async Task ClearAllDataAsync()
        {
            await Task.Run(() => {
                try
                {
                    MySQLService db = new MySQLService();
                    // Lưu ý: Thứ tự xóa quan trọng vì khóa ngoại
                    // CHITIETTHELOAI -> PHIM -> NHANPHIM, THELOAI
                    db.ExecuteNonQuery("DELETE FROM CHITIETTHELOAI;");
                    db.ExecuteNonQuery("DELETE FROM PHIM;");
                    db.ExecuteNonQuery("DELETE FROM NHANPHIM;");
                    db.ExecuteNonQuery("DELETE FROM THELOAI;");
                    // db.ExecuteNonQuery("DELETE FROM THAMSO;"); // Thường không nên xóa tham số

                    // Reset Auto Increment nếu cần (MySQL)
                    db.ExecuteNonQuery("ALTER TABLE PHIM AUTO_INCREMENT = 1;");
                    db.ExecuteNonQuery("ALTER TABLE NHANPHIM AUTO_INCREMENT = 1;");
                    db.ExecuteNonQuery("ALTER TABLE THELOAI AUTO_INCREMENT = 1;");

                    Log("=== ĐÃ XÓA TOÀN BỘ DỮ LIỆU DB THÀNH CÔNG ===");
                }
                catch (Exception ex)
                {
                    Log($"LỖI KHI XÓA DỮ LIỆU: {ex.Message}");
                }
            });
        }
    }
}
