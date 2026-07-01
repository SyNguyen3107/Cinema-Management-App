using Cinema_Management_App.Models;
using Cinema_Management_App.Interfaces;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cinema_Management_App.Viewmodels
{
    public partial class LapDanhSachPhongChieuViewmodel : ObservableObject
    {
        private readonly IPhongChieuRepository _phongChieuRepo;
        private readonly ILoaiPhongRepository _loaiPhongRepo;
        private readonly ILoaiGheRepository _loaiGheRepo;
        private readonly IGheRepository _gheRepo;
        private readonly ITinhTrangPhongRepository _tinhTrangPhongRepo;
        private readonly IDialogService _dialogService;

        public Action<bool?>? RequestClose;
        bool daLapPhongChieu = false;

        [ObservableProperty]
        private ObservableCollection<LoaiPhong> _danhSachLoaiPhong = new();
        [ObservableProperty]
        private ObservableCollection<TinhTrangPhong> _danhSachTinhTrangPhong = new();

        [ObservableProperty]
        private LoaiPhong? _loaiPhongDuocChon;

        [ObservableProperty]
        private string _maPhong = string.Empty;

        [ObservableProperty]
        private string _tenPhong = string.Empty;

        [ObservableProperty]
        private string _ghiChu = string.Empty;

        [ObservableProperty]
        private TinhTrangPhong? _chonTinhTrang;

        [ObservableProperty]
        private ObservableCollection<LoaiGhe> _listLoaiGheKhaDung = new();

        [ObservableProperty]
        private ObservableCollection<Ghe> _danhSachGhe = new();

        // Inject repositories and services via constructor
        public LapDanhSachPhongChieuViewmodel(
            IPhongChieuRepository phongChieuRepo,
            ILoaiPhongRepository loaiPhongRepo,
            ILoaiGheRepository loaiGheRepo,
            ITinhTrangPhongRepository tinhTrangPhongRepo,
            IGheRepository gheRepo,
            IDialogService dialogService)
        {
            _phongChieuRepo = phongChieuRepo;
            _loaiPhongRepo = loaiPhongRepo;
            _loaiGheRepo = loaiGheRepo;
            _tinhTrangPhongRepo = tinhTrangPhongRepo;
            _gheRepo = gheRepo;
            _dialogService = dialogService;

            _ = LoadAsync();
        }

        private async Task LoadAsync()
        {
            try
            {
                MaPhong = await _phongChieuRepo.GenerateMaPhong();
                var dsTinhTrang = await _tinhTrangPhongRepo.GetAllTinhTrangPhongAsync();
                var dsLoaiPhong = await _loaiPhongRepo.GetAllLoaiPhongAsync();

                DanhSachLoaiPhong = new ObservableCollection<LoaiPhong>(dsLoaiPhong);
                DanhSachTinhTrangPhong = new ObservableCollection<TinhTrangPhong>(dsTinhTrang);
            }
            catch (Exception ex)
            {
                // Capture and display full exception details for debugging
                _dialogService.ShowError($"Lỗi tải dữ liệu từ cơ sở dữ liệu. Vui lòng kiểm tra kết nối và thử lại.\nChi tiết lỗi: {ex.Message}", "Lỗi");
            }
        }

        // Triggered automatically when the selected room type changes
        async partial void OnLoaiPhongDuocChonChanged(LoaiPhong? value)
        {
            ListLoaiGheKhaDung.Clear();
            if (value != null)
            {
                try
                {
                    // Fetch available seat types based on the selected room type
                    var dsLoaiGhe = await _loaiGheRepo.GetAllLoaiGheByMaLoaiPhongAsync(value.MaLoaiPhong);
                    foreach (var lg in dsLoaiGhe)
                    {
                        ListLoaiGheKhaDung.Add(lg);
                    }
                }
                catch (Exception ex)
                {
                    _dialogService.ShowError($"Lỗi khi lấy danh sách loại ghế:\n{ex.ToString()}", "Lỗi");
                }
            }
        }

        // Command to add a new empty seat to the list
        [RelayCommand]
        private void ThemGhe()
        {
            DanhSachGhe.Add(new Ghe
            {
                MaPhong = this.MaPhong
            });
        }

        // Command to remove a specific seat from the list
        [RelayCommand]
        private void XoaGhe(Ghe? ghe)
        {
            if (ghe != null)
            {
                DanhSachGhe.Remove(ghe);
            }
        }

        // Reset the form for a new entry
        [RelayCommand]
        private async Task PhongChieuMoiAsync()
        {
            DanhSachGhe.Clear();
            LoaiPhongDuocChon = null;
            ListLoaiGheKhaDung.Clear();
            TenPhong = string.Empty;
            GhiChu = string.Empty;
            ChonTinhTrang = null;

            if (_phongChieuRepo != null)
            {
                _maPhong = await _phongChieuRepo.GenerateMaPhong();
            }
        }

        // Close the current window/view
        [RelayCommand]
        private void Thoat()
        {
            RequestClose?.Invoke(daLapPhongChieu);
        }

        // Save the room and its associated seats to the database
        [RelayCommand]
        private async Task LuuThongTinAsync()
        {
            if (!KiemTraThongTin())
                return;

            try
            {
                // Construct the room object
                var newPhong = new PhongChieu
                {
                    MaPhong = this.MaPhong,
                    TenPhong = this.TenPhong.Trim(),
                    MaLoaiPhong = LoaiPhongDuocChon!.MaLoaiPhong,
                    MaTinhTrang = ChonTinhTrang!.MaTinhTrangPhong,
                    GhiChu = this.GhiChu?.Trim() ?? string.Empty
                };

                // Generate primary keys for each seat before inserting
                foreach (var ghe in DanhSachGhe)
                {
                    ghe.MaGhe = await _gheRepo.GenerateMaGhe();
                }

                // Execute the transaction via repository
                var success = await _phongChieuRepo.AddPhongChieuAsync(newPhong, DanhSachGhe);
                if (success)
                {
                    daLapPhongChieu = success;
                    _dialogService.ShowMessage("Lưu thông tin phòng chiếu thành công!", "Thành công");
                    await PhongChieuMoiAsync();
                }
                else
                {
                    _dialogService.ShowError("Lưu thất bại, vui lòng kiểm tra lại cơ sở dữ liệu.", "Lỗi");
                }
            }
            catch (Exception ex)
            {
                // Display the full stack trace and message for critical save errors
                _dialogService.ShowError($"Đã xảy ra lỗi nghiêm trọng khi lưu thông tin:\n{ex.ToString()}", "Lỗi hệ thống");
            }
        }

        // Validate all user inputs and business rules before saving
        private bool KiemTraThongTin()
        {
            if (string.IsNullOrEmpty(TenPhong))
            {
                _dialogService.ShowWarning("Vui lòng nhập tên phòng chiếu!", "Cảnh báo");
                return false;
            }

            if (LoaiPhongDuocChon == null)
            {
                _dialogService.ShowWarning("Vui lòng chọn loại phòng chiếu!", "Cảnh báo");
                return false;
            }

            if (ChonTinhTrang == null)
            {
                _dialogService.ShowWarning("Vui lòng chọn tình trạng phòng!", "Cảnh báo");
                return false;
            }

            if (DanhSachGhe.Count == 0)
            {
                _dialogService.ShowWarning("Phòng chiếu phải có ít nhất 1 ghế!", "Cảnh báo");
                return false;
            }

            foreach (var ghe in DanhSachGhe)
            {
                if (string.IsNullOrEmpty(ghe.MaSoGhe))
                {
                    _dialogService.ShowWarning("Vui lòng nhập đầy đủ mã số ghế cho tất cả các ghế!", "Cảnh báo");
                    return false;
                }

                if (!Regex.IsMatch(ghe.MaSoGhe, @"^[a-zA-Z0-9]+$"))
                {
                    _dialogService.ShowWarning($"Mã số ghế '{ghe.MaSoGhe}' chứa ký tự không hợp lệ (chỉ chấp nhận chữ và số)!", "Cảnh báo");
                    return false;
                }

                if (string.IsNullOrEmpty(ghe.MaLoaiGhe))
                {
                    _dialogService.ShowWarning($"Ghế '{ghe.MaSoGhe}' chưa được chọn loại ghế!", "Cảnh báo");
                    return false;
                }

                // Verify if the selected seat type is compatible with the room type rules
                bool hopLe = ListLoaiGheKhaDung.Any(lg => lg.MaLoaiGhe == ghe.MaLoaiGhe);

                if (!hopLe)
                {
                    _dialogService.ShowWarning($"Loại ghế của ghế '{ghe.MaSoGhe}' không phù hợp với loại phòng chiếu đã chọn!", "Cảnh báo");
                    return false;
                }
            }

            // Check for duplicate seat numbers within the same room
            var gheTrung = DanhSachGhe.GroupBy(g => g.MaSoGhe).FirstOrDefault(g => g.Count() > 1);

            if (gheTrung != null)
            {
                _dialogService.ShowWarning($"Mã số ghế '{gheTrung.Key}' bị trùng lặp trong danh sách!", "Cảnh báo");
                return false;
            }

            return true; // All validations passed
        }
    }
}