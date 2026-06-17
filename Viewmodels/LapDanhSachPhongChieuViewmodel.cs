using Cinema_Management_App.Models;
using Cinema_Management_App.Repositories;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;

namespace Cinema_Management_App.Viewmodels
{
    public partial class LapDanhSachPhongChieuViewmodel : ObservableObject
    {
        private readonly PhongChieuRepository _phongChieuRepo;
        private readonly LoaiPhongRepository _loaiPhongRepo;
        private readonly LoaiGheRepository _loaiGheRepo;
        private readonly TinhTrangPhongRepository _tinhTrangPhongRepo;

        [ObservableProperty]
        private ObservableCollection<LoaiPhong> _danhSachLoaiPhong;
        [ObservableProperty]
        private ObservableCollection<TinhTrangPhong> _danhSachTinhTrangPhong;

        [ObservableProperty]
        private LoaiPhong _loaiPhongDuocChon;

        [ObservableProperty]
        private string _maPhong;

        [ObservableProperty]
        private string _tenPhong;

        [ObservableProperty]
        private string _ghiChu;

        [ObservableProperty]
        private TinhTrangPhong _chonTinhTrang;


        [ObservableProperty]
        private ObservableCollection<LoaiGhe> _listLoaiGheKhaDung = new ObservableCollection<LoaiGhe>();

        [ObservableProperty]
        private ObservableCollection<Ghe> _danhSachGhe = new();

        public LapDanhSachPhongChieuViewmodel(
            PhongChieuRepository phongChieuRepo,
            LoaiPhongRepository loaiPhongRepo,
            LoaiGheRepository loaiGheRepo,
            TinhTrangPhongRepository tinhTrangPhongRepo)
        {
            _phongChieuRepo = phongChieuRepo;
            _loaiPhongRepo = loaiPhongRepo;
            _loaiGheRepo = loaiGheRepo;
            _tinhTrangPhongRepo = tinhTrangPhongRepo;

            Load();
            if (_phongChieuRepo != null)
            {
                MaPhong = _phongChieuRepo.GetMaPhongChieuMoi();
            }
        }

        private void Load()
        {
            try
            {
                var dsTinhTrang = _tinhTrangPhongRepo.GetAllTinhTrangPhong();
                var dsLoaiPhong = _loaiPhongRepo.GetAllLoaiPhong();

                DanhSachLoaiPhong = new ObservableCollection<LoaiPhong>(dsLoaiPhong);
                DanhSachTinhTrangPhong = new ObservableCollection<TinhTrangPhong>(dsTinhTrang);

            }
            catch
            {
                MessageBox.Show("Lỗi khi tải dữ liệu từ CSDL. Vui lòng kiểm tra kết nối và thử lại.", "Lỗi");
            }
        }


        partial void OnLoaiPhongDuocChonChanged(LoaiPhong value)
        {
            ListLoaiGheKhaDung.Clear();
            if (value != null)
            {
                var dsLoaiGhe = _loaiGheRepo.GetAllLoaiGheByMaLoaiPhong(value.MaLoaiPhong);
                foreach (var lg in dsLoaiGhe)
                {
                    ListLoaiGheKhaDung.Add(lg);
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
        private void XoaGhe(Ghe ghe)
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
                MaPhong = _phongChieuRepo.GetMaPhongChieuMoi(); // Tự động lấy mã phòng chiếu mới từ repository
            }
        }

        [RelayCommand]
        private void Thoat(Window window)
        {
            if (window != null)
            {
                window.Close();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        [RelayCommand]
        private void LuuThongTin()
        {
            if (!KiemTraThongTin())
                return;

            try
            {
                var newPhong = new PhongChieu
                {
                    MaPhong = this.MaPhong,
                    TenPhong = this.TenPhong,
                    MaLoaiPhong = LoaiPhongDuocChon.MaLoaiPhong,
                    MaTinhTrang = ChonTinhTrang.MaTinhTrangPhong,
                    GhiChu = this.GhiChu
                };
                foreach (var ghe in DanhSachGhe)
                {
                    ghe.MaGhe = $"{MaPhong}_{ghe.MaSoGhe}";
                }
                if (_phongChieuRepo.AddPhongChieu(newPhong, DanhSachGhe))
                {
                    MessageBox.Show("Lưu thông tin phòng chiếu thành công!");
                    PhongChieuMoi();
                }
                else
                {
                    MessageBox.Show("Lưu thất bại, vui lòng kiểm tra lại CSDL.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu thông tin: {ex.Message}");
            }
        }
        private bool KiemTraThongTin()
        {
            if (string.IsNullOrEmpty(TenPhong))
            {
                MessageBox.Show("Vui lòng nhập tên phòng!", "Cảnh báo");
                return false;
            }

            if (LoaiPhongDuocChon == null)
            {
                MessageBox.Show("Vui lòng chọn loại phòng!", "Cảnh báo");
                return false;
            }
            if (ChonTinhTrang == null)
            {
                MessageBox.Show("Vui lòng chọn tình trạng phòng!", "Cảnh báo");
                return false;
            }
            if (DanhSachGhe.Count == 0)
            {
                MessageBox.Show("Phòng chiếu phải có ít nhất 1 ghế!", "Cảnh báo");
                return false;
            }

            foreach (var ghe in DanhSachGhe)
            {
                if (string.IsNullOrEmpty(ghe.MaSoGhe))
                {
                    MessageBox.Show("Nhập đầy đủ số ghế!", "Cảnh báo");
                    return false;
                }
                if (!Regex.IsMatch(ghe.MaSoGhe, @"^[a-zA-Z0-9]+$"))
                {
                    MessageBox.Show(
                        $"Mã số ghế '{ghe.MaSoGhe}' chứa ký tự không hợp lệ!",
                        "Cảnh báo"
                    );

                    return false;
                }
                if (string.IsNullOrEmpty(ghe.MaLoaiGhe))
                {
                    MessageBox.Show(
                        $"Ghế {ghe.MaSoGhe} chưa chọn loại ghế!",
                        "Cảnh báo"
                    );

                    return false;
                }

                bool hopLe = ListLoaiGheKhaDung
                    .Any(lg => lg.MaLoaiGhe == ghe.MaLoaiGhe);

                if (!hopLe)
                {
                    MessageBox.Show(
                        $"Ghế {ghe.MaSoGhe} không phù hợp với loại phòng!",
                        "Cảnh báo"
                    );

                    return false;
                }

                
            }
            var gheTrung = DanhSachGhe
    .GroupBy(g => g.MaSoGhe)
    .FirstOrDefault(g => g.Count() > 1);

            if (gheTrung != null)
            {
                MessageBox.Show(
                    $"Ghế mã số {gheTrung.Key} bị trùng!",
                    "Cảnh báo"
                );

                return false;
            }
            return true;
        }
    }
}