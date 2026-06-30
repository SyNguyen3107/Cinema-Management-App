using Cinema_Management_App.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Cinema_Management_App.Viewmodels
{
    public partial class DashboardViewmodel : ObservableObject
    {
        private readonly IVeRepository _veRepo;
        private readonly IWindowService _windowService;
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        private decimal _todayRevenue;

        public DashboardViewmodel(
            IVeRepository veRepo,
            IWindowService windowService,
            IDialogService dialogService)
        {
            _veRepo = veRepo;
            _windowService = windowService;
            _dialogService = dialogService;

            _ = RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            try
            {
                TodayRevenue =
                    await _veRepo
                    .GetTotalSellingbyDayAsync(
                        DateTime.Now);
            }
            catch (Exception ex)
            {
                _dialogService.ShowError(
                    ex.Message,
                    "Lỗi");
            }
        }

        [RelayCommand]
        private async Task OpenBanVe()
        {
            bool? result =
                _windowService
                .ShowDialog<BanVeViewmodel>();

            if (result == true)
            {
                await RefreshAsync();
            }
        }

        [RelayCommand]
        private async Task OpenLapSuatChieuAsync()
        {
            bool? result =
                _windowService
                .ShowDialog<LapSuatChieuViewmodel>();

            
            await RefreshAsync();
            
        }

        [RelayCommand]
        private async Task OpenTiepNhanPhimAsync()
        {
            bool? result =
                _windowService
                .ShowDialog<TiepNhanPhimViewmodel>();

            if (result == true)
            {
                await RefreshAsync();
            }
        }
    }
}