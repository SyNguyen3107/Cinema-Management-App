using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Cinema_Management_App.Enums;
using System;

namespace Cinema_Management_App.Viewmodels
{
    public partial class MainViewmodel : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;
        [ObservableProperty]
        private object _currentViewModel = null!;

        public MainViewmodel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            CurrentViewModel = _serviceProvider.GetRequiredService<DashboardViewmodel>();
        }

        [RelayCommand]
        private void ChuyenTrang(PageType type)
        {
            switch (type)
            {
                case PageType.DASHBOARD:
                    CurrentViewModel = _serviceProvider.GetRequiredService<DashboardViewmodel>();
                    break;

            }
        }
    }
}