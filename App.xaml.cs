using System.Configuration;
using System.Data;
using System.Windows;
using Cinema_Management_App.Models;
using Cinema_Management_App.Repositories;

namespace Cinema_Management_App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
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
            services.AddTransient<CapNhatPhimViewmodel>();
            services.AddTransient<TiepNhanPhimViewmodel>();
            services.AddTransient<XoaPhimViewmodel>();
            //4. Đăng ký Views
            services.AddTransient<TiepNhanPhimView>();
            services.AddTransient<CapNhatPhimView>();
            services.AddTransient<XoaPhimView>();
            return services.BuildServiceProvider();
        }
    }
}