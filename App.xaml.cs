using System;
using System.IO;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Cinema_Management_App.Services;
using Cinema_Management_App.Repositories;
using Cinema_Management_App.Views;
using Application = System.Windows.Application;
using Cinema_Management_App.Viewmodels;

namespace Cinema_Management_App
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; }

        public new static App Current => (App)Application.Current;

        public App()
        {
            Services = ConfigureServices();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            services.AddSingleton<IDatabaseService, MySQLService>();

            // Repositories
            services.AddTransient<PhimRepository>();
            services.AddTransient<NhanPhimRepository>();
            services.AddTransient<TheLoaiRepository>();
            services.AddTransient<PhongChieuRepository>();
            services.AddTransient<LoaiPhongRepository>();
            services.AddTransient<LoaiGheRepository>();
            services.AddTransient<TinhTrangPhongRepository>();

            // ViewModels
            services.AddTransient<TiepNhanPhimViewmodel>();
            services.AddTransient<LapDanhSachPhongChieuViewmodel>();

            // Views
            services.AddTransient<TiepNhanPhimView>();
            services.AddTransient<CapNhatPhimView>();
            services.AddTransient<XoaPhimView>();
            services.AddTransient<LapDanhSachPhongChieuView>();
            services.AddTransient<TraCuuPhongChieuView>();

            return services.BuildServiceProvider();
        }
    }
}