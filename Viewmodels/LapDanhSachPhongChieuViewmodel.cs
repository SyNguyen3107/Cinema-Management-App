using Cinema_Management_App.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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

        // Sửa tham số value thành kiểu string? để an toàn hơn
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

        [RelayCommand]
        private void LuuThongTin() => MessageBox.Show("Đã lưu thành công!");

        [RelayCommand]
        private void Thoat(Window p) => p?.Close();

        private void CapNhatLoaiGheKhaDung()
        {
            // Luôn dùng biến có dấu gạch dưới _ khi đang viết trong ViewModel này
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
            // Tính toán dựa trên danh sách ghế
            decimal tempTong = DanhSachGhe.Sum(x => x.DonGia);

            // Gán cho Property để UI cập nhật (Thư viện sinh ra TongGiaTri từ _tongGiaTri)
            TongGiaTri = tempTong;
        }
    }
}