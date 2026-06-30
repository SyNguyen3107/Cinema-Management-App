using Cinema_Management_App.Interfaces;
using Cinema_Management_App.Repositories;
using Cinema_Management_App.Services;
using Cinema_Management_App.Viewmodels;
using Cinema_Management_App.Views;
using DotNetEnv.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;
using Application = System.Windows.Application;

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

            try
            {
                DotNetEnv.Env.Load();
            }
            catch { }

            var configuration = new ConfigurationBuilder()
.SetBasePath(Directory.GetCurrentDirectory())
.AddDotNetEnv()
.Build();

            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<IDatabaseService, MySQLService>();

            services.AddSingleton<IDialogService, WpfDialogService>();

            services.AddTransient<IPhimRepository, PhimRepository>();
            services.AddTransient<INhanPhimRepository, NhanPhimRepository>();
            services.AddTransient<ITheLoaiRepository, TheLoaiRepository>();
            services.AddTransient<IPhongChieuRepository, PhongChieuRepository>();
            services.AddTransient<ILoaiPhongRepository, LoaiPhongRepository>();
            services.AddTransient<ILoaiGheRepository, LoaiGheRepository>();
            services.AddTransient<ITinhTrangPhongRepository, TinhTrangPhongRepository>();
            services.AddTransient<IGheRepository, GheRepository>();
            services.AddTransient<ISuatChieuRepository, SuatChieuRepository>();
            services.AddTransient<IThamSoRepository, ThamSoRepository>();
            services.AddTransient<IVeRepository, VeRepository>();
            services.AddSingleton<IWindowService, WindowService>();

            services.AddTransient<TiepNhanPhimViewmodel>();
            services.AddTransient<LapDanhSachPhongChieuViewmodel>();
            services.AddTransient<TraCuuPhongChieuViewmodel>();
            services.AddTransient<LapSuatChieuViewmodel>();
            services.AddTransient<BanVeViewmodel>();
            services.AddTransient<BaoCaoDoanhThuPhimTheoThangViewmodel>();
            services.AddTransient<BaoCaoDoanhThuTheoLoaiPhongViewmodel>();
            services.AddTransient<DashboardViewmodel>();
            services.AddSingleton<MainViewmodel>();

            services.AddTransient<TiepNhanPhimView>();
            services.AddTransient<LapDanhSachPhongChieuView>();
            services.AddTransient<TraCuuPhongChieuView>();
            services.AddTransient<LapSuatChieuView>();
            services.AddTransient<BanVeView>();
            services.AddTransient<BaoCaoDoanhThuPhimTheoThangView>();
            services.AddTransient<BaoCaoDoanhThuTheoLoaiPhongView>();
            services.AddSingleton<MainView>();

            var serviceProvider = services.BuildServiceProvider();

            var windowService = serviceProvider.GetRequiredService<IWindowService>();

            windowService.Register<TiepNhanPhimViewmodel, TiepNhanPhimView>();

            windowService.Register<LapDanhSachPhongChieuViewmodel, LapDanhSachPhongChieuView>();

            windowService.Register<TraCuuPhongChieuViewmodel, TraCuuPhongChieuView>();

            windowService.Register<LapSuatChieuViewmodel, LapSuatChieuView>();

            windowService.Register<BanVeViewmodel, BanVeView>();

            windowService.Register<BaoCaoDoanhThuPhimTheoThangViewmodel, BaoCaoDoanhThuPhimTheoThangView>();

            windowService.Register<BaoCaoDoanhThuTheoLoaiPhongViewmodel, BaoCaoDoanhThuTheoLoaiPhongView>();


            return serviceProvider;
        }
        protected override void OnStartup(
    StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow =
                Services.GetRequiredService<MainView>();

            mainWindow.Show();
        }
    }
}