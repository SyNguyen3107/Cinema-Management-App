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
        // Khởi tạo Repository một lần duy nhất
        private readonly PhongChieuRepository _repo = new PhongChieuRepository();

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
            if (string.IsNullOrEmpty(ChonLoaiPhong))
            {
                MessageBox.Show("Vui lòng chọn loại phòng trước!");
                return;
            }

            var newGhe = new Ghe { STT = DanhSachGhe.Count + 1 };

            // Đăng ký sự kiện cập nhật tổng tiền
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
        private void LuuThongTin()
        {
            // 1. Validation (Kiểm tra dữ liệu)
            if (string.IsNullOrEmpty(TenPhong))
            {
                MessageBox.Show("Vui lòng nhập tên phòng chiếu!");
                return;
            }
            if (string.IsNullOrEmpty(ChonLoaiPhong))
            {
                MessageBox.Show("Vui lòng chọn loại phòng!");
                return;
            }
            if (DanhSachGhe.Count == 0)
            {
                MessageBox.Show("Phòng chiếu phải có ít nhất một ghế!");
                return;
            }

            try
            {
                // 2. Chuyển đổi dữ liệu hiển thị sang ID database
                // Lưu ý: ID này phải khớp với bảng LOAIPHONG và TINHTRANGPHONG trong MySQL
                int maLoai = (ChonLoaiPhong == "Phòng thường") ? 1 : 2;
                int maTT = (ChonTinhTrang == "Hoạt động") ? 1 : 2;

                var newPhong = new PhongChieu
                {
                    TenPhong = TenPhong,
                    MaLoaiPhong = maLoai,
                    MaTinhTrang = maTT,
                    GhiChu = GhiChu,
                    TongGiaTri = TongGiaTri
                };

                // 3. Thực hiện lưu thông qua Repository
                if (_repo.LuuPhongChieu(newPhong, DanhSachGhe))
                {
                    MessageBox.Show("Lưu thông tin phòng chiếu vào MySQL thành công!");
                    ResetForm(); // Xóa sạch form sau khi lưu
                }
                else
                {
                    MessageBox.Show("Lỗi: Không thể kết nối hoặc lưu vào cơ sở dữ liệu!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hệ thống: {ex.Message}");
            }
        }

        private void ResetForm()
        {
            TenPhong = string.Empty;
            GhiChu = string.Empty;
            ChonLoaiPhong = null;
            ChonTinhTrang = null;
            DanhSachGhe.Clear();
            MaPhong = "PC" + DateTime.Now.ToString("ddMMyyHHmm");
            TinhTongGiaTri();
        }

        [RelayCommand]
        private void Thoat(Window p) => p?.Close();

        [RelayCommand]
        private void TraCuuPhongChieu()
        {
            MessageBox.Show("Đang mở chức năng tra cứu...");
        }

        private void CapNhatLoaiGheKhaDung()
        {
            _listLoaiGheKhaDung.Clear();
            if (ChonLoaiPhong == "Phòng thường")
            {
                _listLoaiGheKhaDung.Add("A");
                _listLoaiGheKhaDung.Add("B");
                _listLoaiGheKhaDung.Add("C");
            }
            else if (ChonLoaiPhong == "Phòng IMAX")
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