using Cinema_Management_App.DTOs;
using Cinema_Management_App.Extensions;
using Cinema_Management_App.Interfaces;
using Cinema_Management_App.Models;
using Cinema_Management_App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema_Management_App.Viewmodels
{
    public partial class TraCuuPhongChieuViewmodel : ObservableObject
    {
        private readonly IPhongChieuRepository _phongChieuRepo;
        private readonly ILoaiPhongRepository _loaiPhongRepo;
        private readonly ITinhTrangPhongRepository _tinhTrangPhongRepo;
        private readonly IDialogService _dialogService;
        private readonly IWindowService _windowService;

        [ObservableProperty]
        private ObservableCollection<LoaiPhong> _danhSachLoaiPhong = new();

        [ObservableProperty]
        private ObservableCollection<TinhTrangPhong> _danhSachTinhTrangPhong = new();

        [ObservableProperty]
        private LoaiPhong? _loaiPhongDuocChon;
        [ObservableProperty]
        private TinhTrangPhong? _tinhTrangDuocChon;

        [ObservableProperty]
        private string _maPhongContain = string.Empty;

        [ObservableProperty]
        private string _tenPhongContain = string.Empty;

        [ObservableProperty]
        private string _ghiChuContain = string.Empty;

        [ObservableProperty]
        private int? _soLuongGheTu;

        [ObservableProperty]
        private int? _soLuongGheDen;

        [ObservableProperty]
        private decimal? _tongTienTu;
        [ObservableProperty]
        private decimal? _tongTienDen;

        [ObservableProperty]
        private ObservableCollection<TraCuuPhongChieuDTO> _danhSachPhongChieuHopLe = new();

        public Action<bool?>? RequestClose;
        private bool infoChanged = false;

        // Inject repositories and services via constructor
        public TraCuuPhongChieuViewmodel(IPhongChieuRepository phongChieuRepo,
            ILoaiPhongRepository loaiPhongRepo,
            ITinhTrangPhongRepository tinhTrangPhongRepo,
            IDialogService dialogService,
            IWindowService windowService)
        {
            _phongChieuRepo = phongChieuRepo;
            _loaiPhongRepo = loaiPhongRepo;
            _tinhTrangPhongRepo = tinhTrangPhongRepo;
            _dialogService = dialogService;
            _windowService = windowService;

            _ = LoadAsync();
        }

        // Asynchronously load initial data for dropdowns, including "All" option
        private async Task LoadAsync()
        {
            try
            {
                var dsTinhTrangDangCo = await _tinhTrangPhongRepo.GetAllTinhTrangPhongAsync();
                var dsLoaiPhongDangCo = await _loaiPhongRepo.GetAllLoaiPhongAsync();

                // Add dummy "All" item for Room Types
                var dsLoaiPhong = new List<LoaiPhong> { new LoaiPhong { MaLoaiPhong = "", TenLoaiPhong = "Tất cả" } };
                dsLoaiPhong.AddRange(dsLoaiPhongDangCo);
                DanhSachLoaiPhong = new ObservableCollection<LoaiPhong>(dsLoaiPhong);
                LoaiPhongDuocChon = dsLoaiPhong[0];

                // Add dummy "All" item for Room Statuses
                var dsTinhTrangPhong = new List<TinhTrangPhong> { new TinhTrangPhong { MaTinhTrangPhong = "", TenTinhTrangPhong = "Tất cả" } };
                dsTinhTrangPhong.AddRange(dsTinhTrangDangCo);
                DanhSachTinhTrangPhong = new ObservableCollection<TinhTrangPhong>(dsTinhTrangPhong);
                TinhTrangDuocChon = dsTinhTrangPhong[0];

                // Optional: Auto-trigger a search on first load to display all data
                await TraCuuPhongChieuAsync();
            }
            catch (Exception ex)
            {
                // Capture and display full exception details for debugging
                _dialogService.ShowError($"Lỗi tải dữ liệu từ cơ sở dữ liệu. Vui lòng kiểm tra kết nối.\nChi tiết lỗi: {ex.Message}", "Lỗi");
            }
        }

        // Search for screening rooms based on provided filters
        [RelayCommand]
        private async Task TraCuuPhongChieuAsync()
        {
            try
            {
                // Map the dummy "All" options to null so the repository ignores these filters
                string? loaiPhongFilter = (LoaiPhongDuocChon == null || LoaiPhongDuocChon.TenLoaiPhong == "Tất cả") ? null : LoaiPhongDuocChon.TenLoaiPhong;
                string? tinhTrangFilter = (TinhTrangDuocChon == null || TinhTrangDuocChon.TenTinhTrangPhong == "Tất cả") ? null : TinhTrangDuocChon.TenTinhTrangPhong;

                var ketQua = await _phongChieuRepo.TraCuuPhongChieuAsync(
                    MaPhongContain, TenPhongContain, loaiPhongFilter, tinhTrangFilter, GhiChuContain,
                    SoLuongGheTu, SoLuongGheDen, TongTienTu, TongTienDen
                );

                DanhSachPhongChieuHopLe.Clear();
                foreach (var item in ketQua)
                {
                    DanhSachPhongChieuHopLe.Add(item);
                }
            }
            catch (Exception ex)
            {
                // Display the full stack trace and message for search errors
                _dialogService.ShowError($"Đã xảy ra lỗi trong quá trình tra cứu:\n{ex.ToString()}", "Lỗi hệ thống");
            }
        }
        [RelayCommand]
        private async Task ChinhSua(TraCuuPhongChieuDTO phongDuocChon)
        {
            if (phongDuocChon == null) return;

            bool? ketQuaLuu = _windowService.ShowDialog<ChinhSuaPhongChieuViewmodel>(vm =>
            {
                vm.RecieveRoomId(phongDuocChon.MaPhong);
            });
            if (ketQuaLuu == true)
            {
                infoChanged = true;
                await TraCuuPhongChieuAsync();
            }
        }
        [RelayCommand]
        private void Thoat()
        {
            RequestClose?.Invoke(infoChanged);
        }
    }
}