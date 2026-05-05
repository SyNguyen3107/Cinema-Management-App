using Cinema_Management_App.Models;
using Cinema_Management_App.Repositories;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Cinema_Management_App.Viewmodels
{
    public partial class LapDanhSachPhongChieuViewmodel : ObservableObject
    {
        [ObservableProperty]
        private string _maPhong = "PC" + DateTime.Now.ToString("ddMMyyHHmm");

        [ObservableProperty]
        private string _tenPhong;

        [ObservableProperty]
        private string _chonLoaiPhong;
        partial void OnChonLoaiPhongChanged(string value)
        {
            CapNhatLoaiGheKhaDung();
            DanhSachGhe.Clear();
            TinhTongGiaTri();
        }

        [ObservableProperty]
        private string _chonTinhTrang;

        [ObservableProperty]
        private string _ghiChu;

        [ObservableProperty]
        private decimal _tongGiaTri;

        [ObservableProperty]
        private ObservableCollection<string> _listLoaiGheKhaDung = new ObservableCollection<string>();

        public ObservableCollection<Ghe> DanhSachGhe { get; set; } = new ObservableCollection<Ghe>();

        [RelayCommand]
        private void ThemGhe()
        {
            // Dùng biến private _chonLoaiPhong để tránh lỗi context nếu VS chưa build xong
            if (string.IsNullOrEmpty(_chonLoaiPhong))
            {
                MessageBox.Show("Vui lòng chọn loại phòng trước!");
                return;
            }

            var newGhe = new Ghe { STT = DanhSachGhe.Count + 1 };

            // Đăng ký sự kiện để khi chọn loại ghế trong DataGrid, tổng tiền tự nhảy
            newGhe.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(Ghe.DonGia))
                {
                    TinhTongGiaTri();
                }
            };

            DanhSachGhe.Add(newGhe);
        }

        [RelayCommand]
        private void DeleteGhe(Ghe p)
        {
            if (p != null)
            {
                DanhSachGhe.Remove(p);
                for (int i = 0; i < DanhSachGhe.Count; i++)
                    DanhSachGhe[i].STT = i + 1;
                TinhTongGiaTri();
            }
        }
        private readonly PhongChieuRepository _repo = new PhongChieuRepository();
        [RelayCommand]
        private void LuuThongTin()
        {
            try
            {
                // Chuyển đổi tên loại phòng sang ID (Ví dụ: Phòng thường = 1, IMAX = 2)
                int maLoai = ChonLoaiPhong == "Phòng thường" ? 1 : 2;
                int maTT = ChonTinhTrang == "Hoạt động" ? 1 : 2;

                var newPhong = new PhongChieu
                {
                    TenPhong = TenPhong,
                    MaLoaiPhong = maLoai,
                    MaTinhTrang = maTT,
                    GhiChu = GhiChu,
                    TongGiaTri = TongGiaTri
                };

                if (_repo.LuuPhongChieu(newPhong, DanhSachGhe))
                {
                    MessageBox.Show("Lưu dữ liệu vào MySQL thành công!");
                }
                else
                {
                    MessageBox.Show("Lưu thất bại. Vui lòng kiểm tra lại kết nối.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }

        [RelayCommand]
        private void Thoat(Window p) => p?.Close();

        private void CapNhatLoaiGheKhaDung()
        {
            _listLoaiGheKhaDung.Clear();
            if (_chonLoaiPhong == "Phòng thường")
            {
                _listLoaiGheKhaDung.Add("A");
                _listLoaiGheKhaDung.Add("B");
                _listLoaiGheKhaDung.Add("C");
            }
            else if (_chonLoaiPhong == "Phòng IMAX")
            {
                _listLoaiGheKhaDung.Add("D");
                _listLoaiGheKhaDung.Add("E");
            }
        }

        private void TinhTongGiaTri()
        {
            decimal tempTong = DanhSachGhe.Sum(x => x.DonGia);
            TongGiaTri = tempTong;
        }
    }
}