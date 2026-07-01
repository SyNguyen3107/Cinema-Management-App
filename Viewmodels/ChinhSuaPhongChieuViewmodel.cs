using Cinema_Management_App.DTOs;
using Cinema_Management_App.Interfaces;
using Cinema_Management_App.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema_Management_App.Viewmodels
{
    public partial class ChinhSuaPhongChieuViewmodel : ObservableObject
    {
        private readonly IPhongChieuRepository _phongChieuRepo;
        private readonly ILoaiPhongRepository _loaiPhongRepo;
        private readonly ILoaiGheRepository _loaiGheRepo;
        private readonly IGheRepository _gheRepo;
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
        private string _maPhong = string.Empty;

        [ObservableProperty]
        private string _tenPhong = string.Empty;

        [ObservableProperty]
        private string _ghiChu = string.Empty;

        [ObservableProperty]
        private TinhTrangPhong? _chonTinhTrang;

        [ObservableProperty]
        private ObservableCollection<LoaiGhe> _listLoaiGhe = new();

        [ObservableProperty]
        private ObservableCollection<Ghe> _DanhSachGheBanDau = new();

        private PhongChieu _phongChieuHienTai;

        public Action<bool?>? RequestClose;
        private bool infoChanged = false;

        public ChinhSuaPhongChieuViewmodel(
            IPhongChieuRepository phongChieuRepo,
            ILoaiPhongRepository loaiPhongRepo,
            ILoaiGheRepository loaiGheRepo,
            ITinhTrangPhongRepository tinhTrangPhongRepo,
            IGheRepository gheRepo,
            IDialogService dialogService,
            IWindowService windowService)
        {
            _phongChieuRepo = phongChieuRepo;
            _loaiPhongRepo = loaiPhongRepo;
            _loaiGheRepo = loaiGheRepo;
            _tinhTrangPhongRepo = tinhTrangPhongRepo;
            _gheRepo = gheRepo;
            _dialogService = dialogService;
            _windowService = windowService;
        }

        public void RecieveRoomId(string maPhong)
        {
            MaPhong = maPhong;
            _ = LoadAsync();
        }

        private async Task LoadAsync()
        {
            try
            {
                var dsTinhTrang = await _tinhTrangPhongRepo.GetAllTinhTrangPhongAsync();
                var dsLoaiPhong = await _loaiPhongRepo.GetAllLoaiPhongAsync();
                var dsLoaiGhe = await _loaiGheRepo.GetAllLoaiGheAsync();

                DanhSachLoaiPhong = new ObservableCollection<LoaiPhong>(dsLoaiPhong);
                DanhSachTinhTrangPhong = new ObservableCollection<TinhTrangPhong>(dsTinhTrang);
                ListLoaiGhe = new ObservableCollection<LoaiGhe>(dsLoaiGhe);

                _phongChieuHienTai = await _phongChieuRepo.GetByIdAsync(MaPhong);

                if (_phongChieuHienTai != null)
                {
                    MaPhong = _phongChieuHienTai.MaPhong;
                    TenPhong = _phongChieuHienTai.TenPhong;
                    GhiChu = _phongChieuHienTai.GhiChu;

                    LoaiPhongDuocChon = DanhSachLoaiPhong.FirstOrDefault(x => x.MaLoaiPhong == _phongChieuHienTai.MaLoaiPhong);
                    ChonTinhTrang = DanhSachTinhTrangPhong.FirstOrDefault(x => x.MaTinhTrangPhong == _phongChieuHienTai.MaTinhTrang);

                    var dsGheBanDau = await _gheRepo.GetAllGheByRoomIdAsync(MaPhong);
                    DanhSachGheBanDau = new ObservableCollection<Ghe>(dsGheBanDau);
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi tải dữ liệu từ cơ sở dữ liệu.\nChi tiết lỗi: {ex.Message}", "Lỗi");
            }
        }

        [RelayCommand]
        private async Task CapNhat()
        {
            if (string.IsNullOrWhiteSpace(TenPhong) || LoaiPhongDuocChon == null || ChonTinhTrang == null)
            {
                _dialogService.ShowError("Vui lòng nhập đầy đủ tên phòng, loại phòng và tình trạng.", "Thiếu thông tin");
                return;
            }

            try
            {
                _phongChieuHienTai.TenPhong = TenPhong;
                _phongChieuHienTai.MaLoaiPhong = LoaiPhongDuocChon.MaLoaiPhong;
                _phongChieuHienTai.MaTinhTrang = ChonTinhTrang.MaTinhTrangPhong;
                _phongChieuHienTai.GhiChu = GhiChu;


                var danhSachMaLoaiGheDangDung = DanhSachGheBanDau.Select(g => g.MaLoaiGhe).Distinct().ToList();

                await _loaiGheRepo.KiemTraVaThemQuyDinhLoaiGheAsync(LoaiPhongDuocChon.MaLoaiPhong, danhSachMaLoaiGheDangDung);

                bool result = await _phongChieuRepo.UpdateWithChairListAsync(_phongChieuHienTai, DanhSachGheBanDau);

                if (result)
                {
                    infoChanged = true;
                    RequestClose?.Invoke(infoChanged);
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi lưu dữ liệu:\n{ex.Message}", "Lỗi");
            }
        }

        [RelayCommand]
        private void Thoat()
        {
            RequestClose?.Invoke(infoChanged);
        }

        [RelayCommand]
        private async Task DatLai()
        {
            await LoadAsync();
        }

        [RelayCommand]
        private async Task XoaGhe(Ghe gheCanXoa)
        {
            if (gheCanXoa == null) return;

            bool xacNhan = await _dialogService.ShowConfirmationAsync($"Bạn có chắc muốn xóa ghế {gheCanXoa.MaSoGhe} khỏi danh sách?", "Xác nhận");
            if (xacNhan)
            {
                DanhSachGheBanDau.Remove(gheCanXoa);
            }
        }
        [RelayCommand]
        private void ThemGhe()
        {
            var gheMoi = new Ghe
            {         
                MaGhe = string.Empty,

                MaSoGhe = string.Empty,

                MaLoaiGhe = null
            };
            DanhSachGheBanDau.Add(gheMoi);
        }
    }
}
