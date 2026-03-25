using System;
using System.Windows;
using Cinema_Management_App.Models;
using Cinema_Management_App.Services;   
using Microsoft.Extensions.DependencyInjection;
using Cinema_Management_App.Seed;

namespace Cinema_Management_App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Tạo một bộ cung cấp dịch vụ (Service Provider) toàn cục
        public IServiceProvider Services { get; }

        public new static App Current => (App)Application.Current;

        public App()
        {
            Services = ConfigureServices();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // 1. Đăng ký Services (Dùng AddSingleton: Tạo 1 lần, dùng chung mãi mãi)
            services.AddSingleton<MySQLService>();

            // 2. Đăng ký Repositories (Dùng AddTransient: Cần lúc nào tạo lúc đó)
            services.AddTransient<PhimRepository>();
            services.AddTransient<ThamSoRepository>();
            services.AddTransient<TheLoaiRepository>();
            services.AddTransient<NhanPhimRepository>();

            // 3. Đăng ký ViewModels
            //services.AddTransient<ThemPhimViewModel>();
            //services.AddTransient<MainViewModel>();

            return services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _ = InitAppAsync();
        }

        private async Task InitAppAsync()
        {
            var dbService = Services.GetService<MySQLService>();

            // Kiểm tra kết nối cơ sở dữ liệu
            try
            {
                if (await dbService.TestConnectionAsync())
                {
                    // MessageBox.Show("Kết nối cơ sở dữ liệu thành công!");
                    // Tùy chọn: Xóa dữ liệu cũ trước khi seed để tránh trùng lặp
                    // await RepositoryTester.ClearAllDataAsync();
                    
                    await RepositoryTester.SeedDataAsync();
                    
                    // Tùy chọn: Chạy test repository sau khi seed
                    // RepositoryTester.RunAllTests();
                }
                else
                {
                    MessageBox.Show("Kết nối cơ sở dữ liệu thất bại!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }
    }
}
