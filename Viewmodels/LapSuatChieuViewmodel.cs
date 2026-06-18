using Cinema_Management_App.Interfaces;
using Cinema_Management_App.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Cinema_Management_App.Viewmodels
{
    public partial class LapSuatChieuViewmodel : ObservableObject
    {
        private readonly ISuatChieuRepository _suatChieuRepo;
        private readonly IPhongChieuRepository _phongChieuRepo;
        private readonly IPhimRepository _phimRepo;
        private readonly IThamSoRepository _thamSoRepo;
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        private string _maSuatChieu = string.Empty;

        [ObservableProperty]
        private TraCuuPhongChieuDTO? _phongDuocChon;

        [ObservableProperty]
        private string _loaiPhongChieu = string.Empty;

        [ObservableProperty]
        private string _tinhTrangPhong = string.Empty;

        [ObservableProperty]
        private string _soLuongGhe = string.Empty;

        [ObservableProperty]
        private ObservableCollection<Phim> _danhSachPhim = new();

        [ObservableProperty]
        private Phim? _phimDuocChon;

        [ObservableProperty]
        private string? _thoiLuong;

        [ObservableProperty]
        private ObservableCollection<TraCuuPhongChieuDTO> _danhSachPhongChieu = new();

        [ObservableProperty]
        private DateTime? _ngayChieu = DateTime.Now.Date; 

        [ObservableProperty]
        private DateTime? _gioBatDau;

        [ObservableProperty]
        private DateTime? _gioKetThuc;

        public Action? RequestClose;

        public LapSuatChieuViewmodel(ISuatChieuRepository suatChieuRepo,
            IPhongChieuRepository phongChieuRepo,
            IPhimRepository phimRepo,
             IThamSoRepository thamSoRepo,
             IDialogService dialogService)
        {
            _suatChieuRepo = suatChieuRepo;
            _phongChieuRepo = phongChieuRepo;
            _dialogService = dialogService;
            _phimRepo = phimRepo;
            _thamSoRepo = thamSoRepo;

            _ = LoadAsync();
        }

        private async Task LoadAsync()
        {
            try
            {
                MaSuatChieu = await _suatChieuRepo.GenerateMaSuatChieu();
                var dsPhongChieu = await _phongChieuRepo.TraCuuPhongChieuAsync(null, null, null, null, null, null, null, null, null);
                DanhSachPhongChieu = new ObservableCollection<TraCuuPhongChieuDTO>(dsPhongChieu);

                var dsPhim = await _phimRepo.GetAllAsync();
                DanhSachPhim = new ObservableCollection<Phim>(dsPhim);
            }
            catch
            {
                _dialogService.ShowError("Lỗi khi tải dữ liệu từ cơ sở dữ liệu. Vui lòng kiểm tra kết nối.", "Lỗi");
            }
        }

        partial void OnPhongDuocChonChanged(TraCuuPhongChieuDTO? value)
        {
            if (value != null)
            {
                LoaiPhongChieu = value.LoaiPhong;
                TinhTrangPhong = value.TinhTrangPhong;
                SoLuongGhe = value.SoLuongGhe.ToString();
            }
            else
            {
                LoaiPhongChieu = string.Empty;
                TinhTrangPhong = string.Empty;
                SoLuongGhe = string.Empty;
            }
        }
        partial void OnPhimDuocChonChanged(Phim? value)
        {
            if (value != null)
            {
                ThoiLuong = value.ThoiLuong.ToString();
                TinhGioKetThuc();
            }
            else
            {
                ThoiLuong = null;
            }
        }
        partial void OnGioBatDauChanged(DateTime? value)
        {
            if (GioKetThuc == null)
                TinhGioKetThuc();
        }

        private void TinhGioKetThuc()
        {
            if (GioBatDau.HasValue && PhimDuocChon != null && PhimDuocChon.ThoiLuong > 0)
            {
                GioKetThuc = GioBatDau.Value.AddMinutes(PhimDuocChon.ThoiLuong);
            }
        }
        private async Task<bool> KiemTraSuatChieuHopLeAsync()
        {
            if (PhimDuocChon == null)
            {
                _dialogService.ShowWarning("Vui lòng chọn phim!", "Cảnh báo");
                return false;
            }
            if (PhongDuocChon == null)
            {
                _dialogService.ShowWarning("Vui lòng chọn phòng chiếu!", "Cảnh báo");
                return false;
            }
            if (!NgayChieu.HasValue || !GioBatDau.HasValue || !GioKetThuc.HasValue)
            {
                _dialogService.ShowWarning("Vui lòng nhập đầy đủ ngày và giờ chiếu!", "Cảnh báo");
                return false;
            }

            DateTime hienTaiBatDau = NgayChieu.Value.Date + GioBatDau.Value.TimeOfDay;
            DateTime hienTaiKetThuc = NgayChieu.Value.Date + GioKetThuc.Value.TimeOfDay;

            if (GioKetThuc.Value.TimeOfDay < GioBatDau.Value.TimeOfDay)
            {
                hienTaiKetThuc = hienTaiKetThuc.AddDays(1);
            }

            var dsSuatChieuCungPhong = await _suatChieuRepo.GetByRoomIdAsync(PhongDuocChon.MaPhong);

            bool quyDinhTrungGio = await _thamSoRepo.LayThamSoDungSaiAsync("TS_TRUNG");
            bool quyDinhCachNhau = await _thamSoRepo.LayThamSoDungSaiAsync("TS_CACH"); 
            double thoiGianNghi = 0;

            if (quyDinhCachNhau)
            {
                thoiGianNghi = await _thamSoRepo.LayThamSoGiaTriAsync("TS_NGHI"); 
            }

            foreach (var scCu in dsSuatChieuCungPhong)
            {
                DateTime cuBatDau = scCu.NgayChieu.Date + scCu.GioBatDau;
                DateTime cuKetThuc = scCu.NgayChieu.Date + scCu.GioKetThuc;
                if (scCu.GioKetThuc < scCu.GioBatDau)
                {
                    cuKetThuc = cuKetThuc.AddDays(1);
                }

                if (quyDinhTrungGio)
                {
                    if (hienTaiBatDau < cuKetThuc && cuBatDau < hienTaiKetThuc)
                    {
                        _dialogService.ShowError($"Suất chiếu bị trùng giờ với một suất chiếu khác cùng phòng ({scCu.GioBatDau:hh\\:mm} - {scCu.GioKetThuc:hh\\:mm})!", "Lỗi trùng lịch");
                        return false;
                    }
                }

                if (quyDinhCachNhau)
                {
                    if (cuKetThuc <= hienTaiBatDau)
                    {
                        TimeSpan khoangCach = hienTaiBatDau - cuKetThuc;
                        if (khoangCach.TotalMinutes < thoiGianNghi)
                        {
                            _dialogService.ShowWarning($"Giờ bắt đầu phải cách giờ kết thúc của suất trước ít nhất {thoiGianNghi} phút! (Suất trước kết thúc lúc {cuKetThuc:dd/MM HH:mm})", "Không đủ thời gian nghỉ");
                            return false;
                        }
                    }

                    if (hienTaiKetThuc <= cuBatDau)
                    {
                        TimeSpan khoangCach = cuBatDau - hienTaiKetThuc;
                        if (khoangCach.TotalMinutes < thoiGianNghi)
                        {
                            _dialogService.ShowWarning($"Giờ kết thúc phải cách giờ bắt đầu của suất sau ít nhất {thoiGianNghi} phút! (Suất sau bắt đầu lúc {cuBatDau:dd/MM HH:mm})", "Không đủ thời gian nghỉ");
                            return false;
                        }
                    }
                }
            }

            return true; 
        }

        [RelayCommand]
        private async Task LapSuatChieuAsync()
        {
            if (!await KiemTraSuatChieuHopLeAsync())
                return;

            try
            {
                var suatChieuMoi = new SuatChieu
                {
                    MaSuatChieu = this.MaSuatChieu,
                    NgayChieu = this.NgayChieu!.Value,
                    GioBatDau = this.GioBatDau!.Value.TimeOfDay,
                    GioKetThuc = this.GioKetThuc!.Value.TimeOfDay,
                    MaPhim = this.PhimDuocChon!.MaPhim,
                    MaPhong = this.PhongDuocChon!.MaPhong
                };
                bool ketQua = await _suatChieuRepo.AddSuatChieuAsync(suatChieuMoi);

                if (ketQua)
                {
                    _dialogService.ShowMessage("Lập suất chiếu phim thành công!", "Thành công");

                    MaSuatChieu = await _suatChieuRepo.GenerateMaSuatChieu();
                    await ResetFormAsync();
                }
                else
                {
                    _dialogService.ShowError("Không thể lưu suất chiếu. Vui lòng kiểm tra lại dữ liệu.", "Lỗi hệ thống");
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Đã xảy ra lỗi khi lưu suất chiếu: {ex.Message}", "Lỗi");
            }
        }
        private async Task ResetFormAsync()
        {
            MaSuatChieu = await _suatChieuRepo.GenerateMaSuatChieu();

            PhimDuocChon = null;
            PhongDuocChon = null;

            NgayChieu = DateTime.Now.Date;
            GioBatDau = null;
            GioKetThuc = null;
        }
        [RelayCommand]
        private void Thoat()
        {
            RequestClose?.Invoke();
        }
    }
}