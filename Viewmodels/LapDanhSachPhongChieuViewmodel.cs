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
        private readonly ITinhTrangPhongRepository _tinhTrangPhongRepo;
        private readonly IDialogService _dialogService; // Added for MVVM compliant dialogs

        // Action for the View (code-behind) to subscribe to for closing the window
        public Action? RequestClose;

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

        public LapDanhSachPhongChieuViewmodel(
            IPhongChieuRepository phongChieuRepo,
            ILoaiPhongRepository loaiPhongRepo,
            ILoaiGheRepository loaiGheRepo,
            ITinhTrangPhongRepository tinhTrangPhongRepo,
            IDialogService dialogService) // Inject the Dialog Service
        {
            _phongChieuRepo = phongChieuRepo;
            _loaiPhongRepo = loaiPhongRepo;
            _loaiGheRepo = loaiGheRepo;
            _tinhTrangPhongRepo = tinhTrangPhongRepo;
            _dialogService = dialogService;

            _ = LoadAsync();
            if (_phongChieuRepo != null)
            {
                MaPhong = _phongChieuRepo.GenerateMaPhong();
            }
        }

        private async Task LoadAsync()
        {
            try
            {
                var dsTinhTrang = await _tinhTrangPhongRepo.GetAllTinhTrangPhongAsync();
                var dsLoaiPhong = await _loaiPhongRepo.GetAllLoaiPhongAsync();

                DanhSachLoaiPhong = new ObservableCollection<LoaiPhong>(dsLoaiPhong);
                DanhSachTinhTrangPhong = new ObservableCollection<TinhTrangPhong>(dsTinhTrang);
            }
            catch
            {
                _dialogService.ShowError("Error loading data from the database. Please check your connection and try again.", "Error");
            }
        }

        async partial void OnLoaiPhongDuocChonChanged(LoaiPhong? value)
        {
            ListLoaiGheKhaDung.Clear();
            if (value != null)
            {
                try
                {
                    var dsLoaiGhe = await _loaiGheRepo.GetAllLoaiGheByMaLoaiPhongAsync(value.MaLoaiPhong);
                    foreach (var lg in dsLoaiGhe)
                    {
                        ListLoaiGheKhaDung.Add(lg);
                    }
                }
                catch (Exception ex)
                {
                    _dialogService.ShowError($"Error fetching seat types: {ex.Message}", "Error");
                }
            }
        }

        [RelayCommand]
        private void ThemGhe()
        {
            DanhSachGhe.Add(new Ghe
            {
                MaPhong = this.MaPhong
            });
        }

        [RelayCommand]
        private void XoaGhe(Ghe? ghe)
        {
            if (ghe != null)
            {
                DanhSachGhe.Remove(ghe);
            }
        }

        [RelayCommand]
        private void PhongChieuMoi()
        {
            DanhSachGhe.Clear();
            LoaiPhongDuocChon = null;
            ListLoaiGheKhaDung.Clear();
            TenPhong = string.Empty;
            GhiChu = string.Empty;
            ChonTinhTrang = null;
            if (_phongChieuRepo != null)
            {
                MaPhong = _phongChieuRepo.GenerateMaPhong();
            }
        }

        [RelayCommand]
        private void Thoat()
        {
            // Trigger the action so the View knows it should close without the ViewModel directly manipulating the UI
            RequestClose?.Invoke();
        }

        [RelayCommand]
        private async Task LuuThongTinAsync() // Changed to async Task for RelayCommand
        {
            if (!KiemTraThongTin())
                return;

            try
            {
                var newPhong = new PhongChieu
                {
                    MaPhong = this.MaPhong,
                    TenPhong = this.TenPhong.Trim(),
                    MaLoaiPhong = LoaiPhongDuocChon!.MaLoaiPhong,
                    MaTinhTrang = ChonTinhTrang!.MaTinhTrangPhong, // Ensure this matches your TinhTrangPhong model property
                    GhiChu = this.GhiChu?.Trim() ?? string.Empty
                };

                foreach (var ghe in DanhSachGhe)
                {
                    ghe.MaGhe = $"{MaPhong}_{ghe.MaSoGhe}";
                }

                if (await _phongChieuRepo.AddPhongChieuAsync(newPhong, DanhSachGhe))
                {
                    _dialogService.ShowMessage("Room information saved successfully!", "Success");
                    PhongChieuMoi();
                }
                else
                {
                    _dialogService.ShowError("Failed to save, please check the database.", "Error");
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Error saving information: {ex.Message}", "Error");
            }
        }

        private bool KiemTraThongTin()
        {
            if (string.IsNullOrEmpty(TenPhong))
            {
                _dialogService.ShowWarning("Please enter the room name!", "Warning");
                return false;
            }

            if (LoaiPhongDuocChon == null)
            {
                _dialogService.ShowWarning("Please select a room type!", "Warning");
                return false;
            }

            if (ChonTinhTrang == null)
            {
                _dialogService.ShowWarning("Please select the room status!", "Warning");
                return false;
            }

            if (DanhSachGhe.Count == 0)
            {
                _dialogService.ShowWarning("The room must have at least 1 seat!", "Warning");
                return false;
            }

            foreach (var ghe in DanhSachGhe)
            {
                if (string.IsNullOrEmpty(ghe.MaSoGhe))
                {
                    _dialogService.ShowWarning("Please enter all seat numbers completely!", "Warning");
                    return false;
                }

                if (!Regex.IsMatch(ghe.MaSoGhe, @"^[a-zA-Z0-9]+$"))
                {
                    _dialogService.ShowWarning($"Seat number '{ghe.MaSoGhe}' contains invalid characters!", "Warning");
                    return false;
                }

                if (string.IsNullOrEmpty(ghe.MaLoaiGhe))
                {
                    _dialogService.ShowWarning($"Seat '{ghe.MaSoGhe}' does not have a selected type!", "Warning");
                    return false;
                }

                bool hopLe = ListLoaiGheKhaDung.Any(lg => lg.MaLoaiGhe == ghe.MaLoaiGhe);

                if (!hopLe)
                {
                    _dialogService.ShowWarning($"Seat '{ghe.MaSoGhe}' type is incompatible with the room type!", "Warning");
                    return false;
                }
            }

            var gheTrung = DanhSachGhe.GroupBy(g => g.MaSoGhe).FirstOrDefault(g => g.Count() > 1);

            if (gheTrung != null)
            {
                _dialogService.ShowWarning($"Seat number '{gheTrung.Key}' is duplicated!", "Warning");
                return false;
            }

            return true;
        }
    }
}