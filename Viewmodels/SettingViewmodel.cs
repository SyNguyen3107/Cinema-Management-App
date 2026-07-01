using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;
using Cinema_Management_App.Interfaces;

namespace Cinema_Management_App.Viewmodels
{
    public partial class SettingViewmodel : ObservableObject
    {
        private readonly IThamSoRepository _thamSoRepo;

        // Cờ kiểm soát để tránh việc tự động lưu khi đang tải dữ liệu lúc khởi tạo
        private bool _isLoading;

        [ObservableProperty]
        private bool _thamSoChiBanGheMotLan;

        [ObservableProperty]
        private bool _thamSoBanVeSuatDaBatDau;

        [ObservableProperty] // Đã sửa _ThamSo... thành _thamSo... cho chuẩn quy ước
        private bool _thamSoSuatChieuTrungNhau;

        [ObservableProperty]
        private bool _thamSoSuatChieuPhaiGianCach;

        [ObservableProperty]
        private double? _thamSoThoiGianToiThieuGiuaSuatChieu;

        public SettingViewmodel(IThamSoRepository thamSoRepo)
        {
            _thamSoRepo = thamSoRepo;

            _ = LoadAsync();
        }

        private async Task LoadAsync()
        {
            _isLoading = true;

            ThamSoChiBanGheMotLan = await _thamSoRepo.LayThamSoDungSaiAsync("TS_GHE1LAN");
            ThamSoBanVeSuatDaBatDau = await _thamSoRepo.LayThamSoDungSaiAsync("TS_VECHUABATDAU");
            ThamSoSuatChieuTrungNhau = await _thamSoRepo.LayThamSoDungSaiAsync("TS_TRUNG");
            ThamSoSuatChieuPhaiGianCach = await _thamSoRepo.LayThamSoDungSaiAsync("TS_CACH");

            ThamSoThoiGianToiThieuGiuaSuatChieu = await _thamSoRepo.LayThamSoGiaTriAsync("TS_NGHI");

            _isLoading = false;
        }


        partial void OnThamSoChiBanGheMotLanChanged(bool value)
        {
            if (_isLoading) return;
            _ = _thamSoRepo.CapNhatThamSoDungSaiAsync("TS_GHE1LAN", value);
        }

        partial void OnThamSoBanVeSuatDaBatDauChanged(bool value)
        {
            if (_isLoading) return;
            _ = _thamSoRepo.CapNhatThamSoDungSaiAsync("TS_VECHUABATDAU", value);
        }

        partial void OnThamSoSuatChieuTrungNhauChanged(bool value)
        {
            if (_isLoading) return;
            _ = _thamSoRepo.CapNhatThamSoDungSaiAsync("TS_TRUNG", value);
        }

        partial void OnThamSoSuatChieuPhaiGianCachChanged(bool value)
        {
            if (_isLoading) return;
            _ = _thamSoRepo.CapNhatThamSoDungSaiAsync("TS_CACH", value);

            if (value && !ThamSoThoiGianToiThieuGiuaSuatChieu.HasValue)
            {
                ThamSoThoiGianToiThieuGiuaSuatChieu = 0;
            }
        }

        partial void OnThamSoThoiGianToiThieuGiuaSuatChieuChanged(double? value)
        {
            if (_isLoading || !value.HasValue) return;
            _ = _thamSoRepo.CapNhatThamSoGiaTriAsync("TS_NGHI", value.Value);
        }
    }
}