using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
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

            // Services
            services.AddSingleton<MySQLService>();

            // Repositories
            services.AddTransient<PhimRepository>();
            services.AddTransient<NhanPhimRepository>();
            services.AddTransient<TheLoaiRepository>();

            // ViewModels
            services.AddTransient<CapNhatPhimViewmodel>();
            services.AddTransient<TiepNhanPhimViewmodel>();
            services.AddTransient<XoaPhimViewmodel>();

            // Views
            services.AddTransient<TiepNhanPhimView>();
            services.AddTransient<CapNhatPhimView>();
            services.AddTransient<XoaPhimView>();

            return services.BuildServiceProvider();
        }
    }
}