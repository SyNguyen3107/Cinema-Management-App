using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Cinema_Management_App.Models;
using Cinema_Management_App.Interfaces;
using Cinema_Management_App.Extensions;



namespace Cinema_Management_App.Viewmodels
{
    public partial class TraCuuPhongChieuViewmodel : ObservableObject
    {
        private readonly IPhongChieuRepository _phongChieuRepo;
        private readonly ILoaiPhongRepository _loaiPhongRepo;
        private readonly ITinhTrangPhongRepository _tinhTrangPhongRepo;
        private readonly IDialogService _dialogService;
        
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
        private ObservableCollection<TraCuuPhongChieuDTO> _danhSachPhongChieuHopLe= new();

        public Action? RequestClose;

        public TraCuuPhongChieuViewmodel(IPhongChieuRepository phongChieuRepo,
            ILoaiPhongRepository loaiPhongRepo,
            ITinhTrangPhongRepository tinhTrangPhongRepo,
            IDialogService dialogService)
        {
            _phongChieuRepo = phongChieuRepo;
            _loaiPhongRepo = loaiPhongRepo;
            _tinhTrangPhongRepo = tinhTrangPhongRepo;
            _dialogService = dialogService;

            _ = LoadAsync();
        }
        private async Task LoadAsync()
        {
            try
            {
                var dsTinhTrangDangCo = await _tinhTrangPhongRepo.GetAllTinhTrangPhongAsync();
                var dsLoaiPhongDangCo = await _loaiPhongRepo.GetAllLoaiPhongAsync();
                
                var dsLoaiPhong = new List<LoaiPhong> { new LoaiPhong { MaLoaiPhong = "", TenLoaiPhong = "Tất cả" } };
                dsLoaiPhong.AddRange(dsLoaiPhongDangCo);
                DanhSachLoaiPhong = new ObservableCollection<LoaiPhong>(dsLoaiPhong);
                LoaiPhongDuocChon = dsLoaiPhong[0];

                var dsTinhTrangPhong = new List<TinhTrangPhong> { new TinhTrangPhong { MaTinhTrangPhong = "", TenTinhTrangPhong = "Tất cả" } };
                dsTinhTrangPhong.AddRange(dsTinhTrangDangCo);
                DanhSachTinhTrangPhong = new ObservableCollection<TinhTrangPhong>(dsTinhTrangPhong);
                TinhTrangDuocChon = dsTinhTrangPhong[0];
            }
            catch
            {
                _dialogService.ShowError("Error loading data from the database. Please check your connection and try again.", "Error");
            }
        }
        [RelayCommand]
        private async Task TraCuuPhongChieuAsync()
        {
            try
            {
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
                _dialogService.ShowError($"Error occurred: {ex.Message}", "Error");
            }
        }
        [RelayCommand]
        private void Thoat()
        {
            RequestClose?.Invoke();
        }
    }
}
