using Cinema_Management_App.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using Cinema_Management_App.Models;
using Cinema_Management_App.Repositories;

namespace RapPhimManagement
{
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

            // 3. Đăng ký ViewModels
            //services.AddTransient<ThemPhimViewModel>();
            //services.AddTransient<MainViewModel>();

            return services.BuildServiceProvider();
        }
    }
}