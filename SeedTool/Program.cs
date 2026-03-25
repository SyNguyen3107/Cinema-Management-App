using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Cinema_Management_App.Seed;
using Cinema_Management_App.Services;

namespace SeedTool
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Đảm bảo Debug.WriteLine cũng hiện ra Console (Visual Studio Trace)
            Trace.Listeners.Add(new ConsoleTraceListener());

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("╔══════════════════════════════════════════╗");
            Console.WriteLine("║    CINEMA MANAGEMENT - DATABASE SEEDER   ║");
            Console.WriteLine("╚══════════════════════════════════════════╝");
            Console.WriteLine();

            bool clearFirst = false;
            bool runTests   = false;

            // Đọc tham số dòng lệnh
            foreach (var arg in args)
            {
                if (arg.Equals("--clear", StringComparison.OrdinalIgnoreCase))  clearFirst = true;
                if (arg.Equals("--test",  StringComparison.OrdinalIgnoreCase))  runTests   = true;
            }

            if (clearFirst)
            {
                Console.WriteLine("[!] Tham số --clear được phát hiện. Xóa toàn bộ dữ liệu trước khi seed...");
                await RepositoryTester.ClearAllDataAsync();
                Console.WriteLine();
            }

            try
            {
                var dbService = new MySQLService();

                Console.WriteLine("Đang kiểm tra kết nối đến cơ sở dữ liệu...");
                bool connected = await dbService.TestConnectionAsync();

                if (!connected)
                {
                    Console.WriteLine("[LỖI] Kết nối thất bại! Kiểm tra lại App.config.");
                    Environment.Exit(1);
                    return;
                }

                Console.WriteLine("[OK] Kết nối thành công!");
                Console.WriteLine();

                // ── Seed dữ liệu ──────────────────────────────────────────
                await RepositoryTester.SeedDataAsync();
                Console.WriteLine();

                // ── Xác nhận kết quả ─────────────────────────────────────
                Console.WriteLine("── Kết quả trong Database ──────────────────────────────");
                var theLoaiRepo   = new TheLoaiRepository(dbService);
                var nhanPhimRepo  = new NhanPhimRepository(dbService);
                var phimRepo      = new PhimRepository(dbService);
                var thamSoRepo    = new ThamSoRepository(dbService);

                var listTl  = theLoaiRepo.GetAllTheLoai();
                var listNp  = nhanPhimRepo.GetAllNhanPhim();
                var listPhim = phimRepo.GetAllPhim();
                var ts       = thamSoRepo.GetThamSo();

                Console.WriteLine($"  ThamSo : SoLuongTheLoaiToiDa = {ts?.SoLuongTheLoaiToiDa.ToString() ?? "(null)"}");
                Console.WriteLine($"  NhanPhim          : {listNp.Count} bản ghi");
                Console.WriteLine($"  TheLoai           : {listTl.Count} bản ghi");
                Console.WriteLine($"  Phim              : {listPhim.Count} bản ghi");

                Console.WriteLine();
                Console.WriteLine("  Danh sách Phim:");
                foreach (var p in listPhim)
                    Console.WriteLine($"    [{p.MaPhim}] {p.TenPhim} ({p.ThoiLuong} phút)");

                // ── Chạy test nếu yêu cầu ────────────────────────────────
                if (runTests)
                {
                    Console.WriteLine();
                    Console.WriteLine("── Chạy kiểm tra Repository (--test) ──────────────────");
                    RepositoryTester.RunAllTests();
                }

                Console.WriteLine();
                Console.WriteLine("════════════════════════════════════════════");
                Console.WriteLine("  HOÀN TẤT. Nhấn Enter để thoát...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LỖI NGHIÊM TRỌNG] {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Environment.Exit(1);
            }

            if (Environment.UserInteractive && !Console.IsInputRedirected)
            {
                Console.WriteLine("  Nhấn Enter để thoát...");
                Console.ReadLine();
            }
        }
    }
}
