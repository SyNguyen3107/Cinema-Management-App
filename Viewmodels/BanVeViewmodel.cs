using Cinema_Management_App.Models;
using Cinema_Management_App.DTOs;
using Cinema_Management_App.Interfaces;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cinema_Management_App.Services;

namespace Cinema_Management_App.Viewmodels
{
    public partial class ChiTietGheChon : ObservableObject
    {
        public Action? OnGheChanged { get; set; }
        [ObservableProperty]
        private int _sTT;

        private GheDTO? _gheDuocChon;
        public GheDTO? GheDuocChon
        {
            get => _gheDuocChon;
            set
            {
                if (SetProperty(ref _gheDuocChon, value))
                {
                    OnGheChanged?.Invoke();
                }
            }
        }
    }
        public partial class BanVeViewmodel : ObservableObject
    {

        private readonly IVeRepository _veRepo;
        private readonly ISuatChieuRepository _suatChieuRepo;
        private readonly IPhimRepository _phimRepo;
        private readonly IPhongChieuRepository _phongChieuRepo;
        private readonly IGheRepository _gheRepo;
        private readonly IThamSoRepository _thamSoRepo;
        private readonly IDialogService _dialogService;
        private readonly IWindowService _windowService;

        public Action? RequestClose;

        [ObservableProperty] private ObservableCollection<SuatChieu> _danhSachSuatChieu = new();
        [ObservableProperty] private ObservableCollection<GheDTO> _danhSachGheTrong = new();

        [ObservableProperty] private SuatChieu? _suatChieuDuocChon;
        [ObservableProperty] private string _maVe = string.Empty;
        [ObservableProperty] private string _tenPhim = string.Empty;
        [ObservableProperty] private string _tenPhong = string.Empty;
        [ObservableProperty] private DateTime? _ngayChieu;
        [ObservableProperty] private TimeSpan? _gioBatDau;
        [ObservableProperty] private TimeSpan? _gioKetThuc;

        [ObservableProperty] private DateTime _ngayBan = DateTime.Now;
        [ObservableProperty] private string _tenNhanVienBan = string.Empty;
        [ObservableProperty] private string? _hinhThucThanhToan = string.Empty;
        [ObservableProperty] private decimal _tongTien = 0;

        [ObservableProperty] private ObservableCollection<ChiTietGheChon> _danhSachGheChon = new();

        public BanVeViewmodel(
            IVeRepository veRepo,
            ISuatChieuRepository suatChieuRepo,
            IPhimRepository phimRepo,
            IPhongChieuRepository phongChieuRepo,
            IGheRepository gheRepo,
            IThamSoRepository thamSoRepo,
            IDialogService dialogService,
            IWindowService windowService)
        {
            _veRepo = veRepo;
            _suatChieuRepo = suatChieuRepo;
            _phimRepo = phimRepo;
            _phongChieuRepo = phongChieuRepo;
            _gheRepo = gheRepo;
            _thamSoRepo = thamSoRepo;
            _dialogService = dialogService;
            _windowService = windowService;

            _ = LoadAsync();
            
        }

        private async Task LoadAsync()
        {
            try
            {
                // Generate Ticket ID
                MaVe = await _veRepo.GenerateMaVeAsync();

                // Load available screenings
                var dsSuatChieu = await _suatChieuRepo.GetAllAsync();
                DanhSachSuatChieu = new ObservableCollection<SuatChieu>(dsSuatChieu);
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi tải dữ liệu khởi tạo:\n{ex.Message}", "Lỗi hệ thống");
            }
        }

        async partial void OnSuatChieuDuocChonChanged(SuatChieu? value)
        {
            // Reset form details when screening changes
            DanhSachGheChon.Clear();
            DanhSachGheTrong.Clear();
            TinhTongTien();

            if (value != null)
            {
                try
                {
                    // Fetch related info (Movie Name, Room Name)
                    var phim = await _phimRepo.GetByIdAsync(value.MaPhim);
                    var phong = await _phongChieuRepo.GetByIdAsync(value.MaPhong);

                    TenPhim = phim?.TenPhim ?? "Không xác định";
                    TenPhong = phong?.TenPhong ?? "Không xác định";

                    NgayChieu = value.NgayChieu;
                    GioBatDau = value.GioBatDau;
                    GioKetThuc = value.GioKetThuc;

                    bool quyDinhGhe1Lan = await _thamSoRepo.LayThamSoDungSaiAsync("TS_GHE1LAN");
                    
                    IEnumerable<GheDTO> danhSachGheKhaDung;

                    if (quyDinhGhe1Lan)
                    {
                        danhSachGheKhaDung = await _gheRepo.GetAvailableByRoomId(value.MaPhong, value.MaSuatChieu);
                    }
                    else
                    {
                        danhSachGheKhaDung = await _gheRepo.GetAllGheDTOByRoomIdAsync(value.MaPhong);
                    }
                    
                    foreach (var ghe in danhSachGheKhaDung)
                    {
                        DanhSachGheTrong.Add(ghe);
                    }
                }
                catch (Exception ex)
                {
                    _dialogService.ShowError($"Lỗi khi tải thông tin suất chiếu:\n{ex.Message}", "Lỗi");
                }
            }
            else
            {
                TenPhim = string.Empty;
                TenPhong = string.Empty;
                NgayChieu = null;
                GioBatDau = null;
                GioKetThuc = null;
            }
        }

        [RelayCommand]
        private void ThemGhe()
        {
            if (SuatChieuDuocChon == null)
            {
                _dialogService.ShowWarning("Vui lòng chọn suất chiếu trước khi thêm ghế!", "Cảnh báo");
                return;
            }

            var dongMoi = new ChiTietGheChon
            {
                STT = DanhSachGheChon.Count + 1,
                OnGheChanged = TinhTongTien
            };

            DanhSachGheChon.Add(dongMoi);
        }

        [RelayCommand]
        private void XoaGhe(ChiTietGheChon? item)
        {
            if (item != null)
            {
                DanhSachGheChon.Remove(item);

                for (int i = 0; i < DanhSachGheChon.Count; i++)
                {
                    DanhSachGheChon[i].STT = i + 1;
                }

                TinhTongTien(); 
            }
        }

        // Helper function to recalculate the total price of selected seats
        private void TinhTongTien()
        {
            TongTien = DanhSachGheChon
                .Where(x => x.GheDuocChon != null)
                .Sum(x => x.GheDuocChon!.DonGia);
        }

        [RelayCommand]
        private async Task BanVeAsync()
        {
            if (!await KiemTraTruocKhiBanAsync())
                return;

            try
            {
                // Create Ticket Object
                var veMoi = new Ve
                {
                    MaVe = this.MaVe,
                    NgayBan = DateTime.Now,
                    TenNhanVienBan = this.TenNhanVienBan.Trim(),
                    HinhThucThanhToan = this.HinhThucThanhToan ?? string.Empty,
                    MaSuatChieu = this.SuatChieuDuocChon!.MaSuatChieu,
                    TongTien = this.TongTien
                };

                // Extract list of Seat IDs
                var dsMaGhe = DanhSachGheChon.Select(x => x.GheDuocChon!.MaGhe).ToList();

                // Save to database using the Transactional repository method
                bool success = await _veRepo.AddVeAsync(veMoi, dsMaGhe);

                if (success)
                {
                    _dialogService.ShowMessage("Bán vé thành công!", "Thành công");
                    await ResetFormAsync(); // Reset after successful sale
                }
                else
                {
                    _dialogService.ShowError("Có lỗi xảy ra khi lưu vé, vui lòng thử lại.", "Lỗi");
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi hệ thống khi bán vé:\n{ex.Message}", "Lỗi nghiêm trọng");
            }
        }

        private async Task<bool> KiemTraTruocKhiBanAsync()
        {
            // 1. Basic UI Validations
            if (SuatChieuDuocChon == null)
            {
                _dialogService.ShowWarning("Vui lòng chọn suất chiếu!", "Cảnh báo");
                return false;
            }
            if (string.IsNullOrWhiteSpace(TenNhanVienBan))
            {
                _dialogService.ShowWarning("Vui lòng nhập tên nhân viên bán vé!", "Cảnh báo");
                return false;
            }
            if (DanhSachGheChon.Count == 0)
            {
                _dialogService.ShowWarning("Vui lòng thêm ít nhất 1 ghế cho vé này!", "Cảnh báo");
                return false;
            }

            // 2. Validate seat selections
            foreach (var item in DanhSachGheChon)
            {
                if (item.GheDuocChon == null)
                {
                    _dialogService.ShowWarning($"Dòng số {item.STT} chưa được chọn ghế!", "Cảnh báo");
                    return false;
                }
            }

            // 3. Check for duplicate seats within the current form
            var gheTrung = DanhSachGheChon.GroupBy(g => g.GheDuocChon!.MaGhe).FirstOrDefault(g => g.Count() > 1);
            if (gheTrung != null)
            {
                _dialogService.ShowWarning($"Ghế '{gheTrung.First().GheDuocChon!.MaSoGhe}' đang bị chọn trùng lặp nhiều lần!", "Cảnh báo");
                return false;
            }

            // 4. BUSINESS RULE: Do not sell tickets for screenings that have already started
            bool khongBanVeDaChieu = await _thamSoRepo.LayThamSoDungSaiAsync("TS_VECHUABATDAU");
            if (khongBanVeDaChieu)
            {
                // Calculate absolute start time
                DateTime thoiDiemBatDau = SuatChieuDuocChon.NgayChieu.Date + SuatChieuDuocChon.GioBatDau;

                if (_ngayBan >= thoiDiemBatDau)
                {
                    _dialogService.ShowError("Suất chiếu này đã bắt đầu chiếu. Không thể bán vé!", "Từ chối bán vé");
                    return false;
                }
            }

            return true;
        }

        private async Task ResetFormAsync()
        {
            MaVe = await _veRepo.GenerateMaVeAsync();
            TenNhanVienBan = string.Empty;
            SuatChieuDuocChon = null; // This will trigger OnSuatChieuDuocChonChanged and clear the form automatically
            HinhThucThanhToan = string.Empty;
        }

        [RelayCommand]
        private void TraCuuPhongChieu()
        {
            bool? result =
                _windowService
                .ShowDialog<TraCuuPhongChieuViewmodel>();
        }

        [RelayCommand]
        private void Thoat()
        {
            RequestClose?.Invoke();
        }
    }
}
