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
        private async Task OpenLapSuatChieu()
        {
            bool? result =
                _windowService
                .ShowDialog<LapSuatChieuViewmodel>();


            if (result == true)
            {
                await RefreshAsync();
            }
        }

        [RelayCommand]
        private async Task OpenTiepNhanPhim()
        {
            bool? result =
                _windowService
                .ShowDialog<TiepNhanPhimViewmodel>();

            if (result == true)
            {
                await RefreshAsync();
            }
        }
        [RelayCommand]
        private async Task OpenTraCuuPhongChieu()
        {
            bool? result =
                _windowService
                .ShowDialog<TraCuuPhongChieuViewmodel>();

            if (result == true)
            {
                await RefreshAsync();
            }
        }
        [RelayCommand]
        private async Task OpenBaoCaoDoanhThuPhimTheoThang()
        {
            bool? result =
                _windowService
                .ShowDialog<BaoCaoDoanhThuPhimTheoThangViewmodel>();

            if (result == true)
            {
                await RefreshAsync();
            }
        }
        [RelayCommand]
        private async Task OpenBaoCaoDoanhThuTheoLoaiPhong()
        {
            bool? result =
                _windowService
                .ShowDialog<BaoCaoDoanhThuTheoLoaiPhongViewmodel>();

            if (result == true)
            {
                await RefreshAsync();
            }
        }
        [RelayCommand]
        private async Task OpenLapDanhSachPhongChieu()
        {
            bool? result =
                _windowService
                .ShowDialog<LapDanhSachPhongChieuViewmodel>();

            if (result == true)
            {
                await RefreshAsync();
            }
        }
    }
}