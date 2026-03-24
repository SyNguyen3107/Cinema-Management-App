using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Cinema_Management_App.Services;
using Cinema_Management_App.Repositories;
using Cinema_Management_App.Viewmodels;
using Cinema_Management_App.Views;
using Application = System.Windows.Application;

namespace Cinema_Management_App
{
    public partial class App : Application
    {
        // Tạo bộ cung cấp dịch vụ toàn cục
        public IServiceProvider Services { get; }

        public new static App Current => (App)Application.Current;

        public App()
        {
            Services = ConfigureServices();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // 1. Đăng ký Services
            services.AddSingleton<MySQLService>();

            // 2. Đăng ký Repositories
            services.AddTransient<PhimRepository>();

            // 3. Đăng ký ViewModels
            services.AddTransient<CapNhatPhimViewmodel>();
            services.AddTransient<TiepNhanPhimViewmodel>();
            services.AddTransient<XoaPhimViewmodel>();

            // 4. Đăng ký Views
            services.AddTransient<TiepNhanPhimView>();
            services.AddTransient<CapNhatPhimView>();
            services.AddTransient<XoaPhimView>();

            return services.BuildServiceProvider();
        }
    }
}