using Cinema_Management_App.Interfaces;
using Cinema_Management_App.Viewmodels;
using Cinema_Management_App.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace Cinema_Management_App.Services
{
    public class WindowService : IWindowService
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly Dictionary<Type, Type> _mappings
            = new();

        public WindowService(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Register<TViewModel, TView>()
            where TView : Window
        {
            _mappings[typeof(TViewModel)] =
                typeof(TView);
        }

        public bool? ShowDialog<TViewModel>()
            where TViewModel : class
        {
            var vmType = typeof(TViewModel);

            if (!_mappings.TryGetValue(
                    vmType,
                    out var viewType))
            {
                throw new InvalidOperationException(
                    $"Chưa đăng ký View cho {vmType.Name}");
            }

            var window = (Window)
                _serviceProvider.GetRequiredService(
                    viewType);

            window.Owner =
                Application.Current.MainWindow;

            return window.ShowDialog();
        }
    }
}