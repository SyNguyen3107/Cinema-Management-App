using Cinema_Management_App.Models;
using Cinema_Management_App.Interfaces;
using Cinema_Management_App.Extensions;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Cinema_Management_App.Viewmodels
{
    public partial class TiepNhanPhimViewmodel : ObservableObject
    {
        private readonly IPhimRepository _phimRepository;
        private readonly INhanPhimRepository _nhanPhimRepository;
        private readonly ITheLoaiRepository _theLoaiRepository;
        private readonly IDialogService _dialogService;

        // An event the View can subscribe to in order to close itself
        public Action? RequestClose;

        [ObservableProperty] private string _maPhim = string.Empty;
        [ObservableProperty] private string _tenPhim = string.Empty;
        [ObservableProperty] private string _tenDaoDien = string.Empty;
        [ObservableProperty] private string _tenDienVienChinh = string.Empty;
        [ObservableProperty] private int _thoiLuong;
        [ObservableProperty] private DateTime _ngayKhoiChieu = DateTime.Now;
        [ObservableProperty] private NhanPhim? _nhanPhimDuocChon;
        [ObservableProperty] private TheLoai? _theLoaiDuocChon;

        [ObservableProperty] private ObservableCollection<NhanPhim> _danhSachNhanPhim = new();
        [ObservableProperty] private ObservableCollection<TheLoai> _danhSachTheLoai = new();

        public TiepNhanPhimViewmodel(
            IPhimRepository phimRepository,
            INhanPhimRepository nhanPhimRepository,
            ITheLoaiRepository theLoaiRepository,
            IDialogService dialogService) // Inject the new Dialog Service
        {
            _phimRepository = phimRepository;
            _nhanPhimRepository = nhanPhimRepository;
            _theLoaiRepository = theLoaiRepository;
            _dialogService = dialogService;

            // Fire and forget the initialization (safe pattern for async constructors)
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                MaPhim = await _phimRepository.GenerateMaPhimAsync();
                var listNhanPhim = await _nhanPhimRepository.GetAllNhanPhimAsync();
                var listTheLoai = await _theLoaiRepository.GetAllTheLoaiAsync();

                DanhSachNhanPhim = new ObservableCollection<NhanPhim>(listNhanPhim);
                DanhSachTheLoai = new ObservableCollection<TheLoai>(listTheLoai);
            }
            catch (Exception ex)
            {
                // Capture and display full exception details for debugging
                _dialogService.ShowError($"Lỗi tải dữ liệu từ cơ sở dữ liệu. Vui lòng kiểm tra kết nối.\nChi tiết lỗi: {ex.Message}", "Lỗi");
            }
        }

        [RelayCommand]
        private async Task TiepNhanAsync()
        {
            if (!KiemTraThongTinPhim())
            {
                return;
            }

            var phimMoi = new Phim
            {
                MaPhim = MaPhim,
                TenPhim = this.TenPhim.Trim(),
                TenDaoDien = this.TenDaoDien.Trim(),
                TenDienVienChinh = this.TenDienVienChinh.Trim(),
                ThoiLuong = this.ThoiLuong,
                NgayKhoiChieu = this.NgayKhoiChieu,
                MaNhanPhim = NhanPhimDuocChon?.MaNhanPhim ?? string.Empty,
                MaTheLoai = TheLoaiDuocChon?.MaTheLoai ?? string.Empty
            };

            try
            {
                bool success = await _phimRepository.AddPhimAsync(phimMoi);

                if (success)
                {
                    _dialogService.ShowMessage($"Thêm phim '{TenPhim}' thành công!", "Thành công");
                    await DatLaiAsync();
                }
                else
                {
                    _dialogService.ShowError("Lưu phim thất bại. Vui lòng kiểm tra lại cơ sở dữ liệu.", "Lỗi");
                }
            }
            catch (Exception ex)
            {
                // Display the full stack trace and message for critical save errors
                _dialogService.ShowError($"Đã xảy ra lỗi không mong muốn khi lưu phim:\n{ex.ToString()}", "Lỗi hệ thống");
            }
        }

        [RelayCommand]
        private async Task DatLaiAsync()
        {
            // Generate a new ID for the next potential entry
            MaPhim = await _phimRepository.GenerateMaPhimAsync();

            TenPhim = string.Empty;
            TenDaoDien = string.Empty;
            TenDienVienChinh = string.Empty;
            ThoiLuong = 0;
            NgayKhoiChieu = DateTime.Now;
            NhanPhimDuocChon = null;
            TheLoaiDuocChon = null;
        }

        [RelayCommand]
        private void XoaPhim()
        {
            _dialogService.ShowMessage("Chức năng Xóa phim đang được phát triển.", "Thông tin");
        }

        [RelayCommand]
        private void CapnhatPhim()
        {
            _dialogService.ShowMessage("Chức năng Cập nhật phim đang được phát triển.", "Thông tin");
        }

        [RelayCommand]
        private void TimPhim()
        {
            _dialogService.ShowMessage("Chức năng Tìm kiếm phim đang được phát triển.", "Thông tin");
        }

        [RelayCommand]
        private void Thoat()
        {
            // Trigger the action so the View knows it should close
            RequestClose?.Invoke();
        }

        // Validate all user inputs before saving
        private bool KiemTraThongTinPhim()
        {
            if (string.IsNullOrWhiteSpace(TenPhim))
            {
                _dialogService.ShowWarning("Vui lòng nhập tên phim!", "Cảnh báo");
                return false;
            }
            if (NhanPhimDuocChon == null)
            {
                _dialogService.ShowWarning("Vui lòng chọn nhãn phim!", "Cảnh báo");
                return false;
            }
            if (TheLoaiDuocChon == null)
            {
                _dialogService.ShowWarning("Vui lòng chọn thể loại phim!", "Cảnh báo");
                return false;
            }
            if (ThoiLuong <= 0)
            {
                _dialogService.ShowWarning("Thời lượng phim phải lớn hơn 0!", "Cảnh báo");
                return false;
            }
            if (string.IsNullOrWhiteSpace(TenDaoDien))
            {
                _dialogService.ShowWarning("Vui lòng nhập tên đạo diễn!", "Cảnh báo");
                return false;
            }
            if (string.IsNullOrWhiteSpace(TenDienVienChinh))
            {
                _dialogService.ShowWarning("Vui lòng nhập tên diễn viên chính!", "Cảnh báo");
                return false;
            }
            return true;
        }
    }
}