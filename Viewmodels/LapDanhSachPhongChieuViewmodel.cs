using Cinema_Management_App.Models;
using Cinema_Management_App.Repositories;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Cinema_Management_App.Viewmodels
{
    public partial class LapDanhSachPhongChieuViewmodel : ObservableObject
    {
        private readonly PhongChieuRepository _phongChieuRepo;
        private readonly LoaiPhongRepository _loaiPhongRepo;
        private readonly LoaiGheRepository _loaiGheRepo;

        public LapDanhSachPhongChieuViewmodel(
            PhongChieuRepository phongChieuRepo,
            LoaiPhongRepository loaiPhongRepo,
            LoaiGheRepository loaiGheRepo)
        {
            _phongChieuRepo = phongChieuRepo;
            _loaiPhongRepo = loaiPhongRepo;
            _loaiGheRepo = loaiGheRepo;

            LoadDuLieuBanDau();
            TaoMaPhongMoi();
        }

        // --- PROPERTIES ---
        [ObservableProperty]
        private ObservableCollection<LoaiPhong> _danhSachLoaiPhong = new ObservableCollection<LoaiPhong>();

        [ObservableProperty]
        private LoaiPhong _loaiPhongDuocChon;

        [ObservableProperty]
        private string _maPhong;

        [ObservableProperty]
        private string _tenPhong;

        [ObservableProperty]
        private string _ghiChu; // Đã sửa lỗi thiếu dấu chấm phẩy

        [ObservableProperty]
        private string _chonTinhTrang = "Hoạt động"; // Khởi tạo giá trị mặc định


        [ObservableProperty]
        private ObservableCollection<LoaiGhe> _listLoaiGheKhaDung = new ObservableCollection<LoaiGhe>();

        [ObservableProperty]
        private ObservableCollection<Ghe> _danhSachGhe = new();

        // --- METHODS ---
        private void LoadDuLieuBanDau()
        {
            var dsPhong = _loaiPhongRepo.GetAllLoaiPhong();
            DanhSachLoaiPhong = new ObservableCollection<LoaiPhong>(dsPhong);
        }

        private void TaoMaPhongMoi()
        {
            // Tự động generate Mã Phòng (Bạn có thể sửa logic này theo quy tắc của CSDL)
            MaPhong = "PC" + DateTime.Now.ToString("HHmmss");
        }

        // Tự động bắt sự kiện khi thuộc tính LoaiPhongDuocChon thay đổi nhờ CommunityToolkit
        partial void OnLoaiPhongDuocChonChanged(LoaiPhong value)
        {
            ListLoaiGheKhaDung.Clear();
            if (value != null)
            {
                // Lấy danh sách Loại ghế dựa trên Quy định loại ghế của Loại phòng đó
                var dsLoaiGhe = _loaiGheRepo.GetAllLoaiGheByMaLoaiPhong(value.MaLoaiPhong);
                foreach (var lg in dsLoaiGhe)
                {
                    ListLoaiGheKhaDung.Add(lg);
                }
            }
        }

        // --- COMMANDS ---
        [RelayCommand]
        private void ThemGhe()
        {
            DanhSachGhe.Add(new Ghe
            {
                MaPhong = this.MaPhong
            });
        }

        [RelayCommand]
        private void DeleteGhe(Ghe ghe)
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
            ChonTinhTrang = "Hoạt động";
            TaoMaPhongMoi();
        }

        [RelayCommand]
        private void Thoat(Window window)
        {
            // Đóng cửa sổ hiện tại nếu có truyền CommandParameter[cite: 3, 21], ngược lại Shutdown app
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
            if (string.IsNullOrEmpty(TenPhong))
            {
                MessageBox.Show("Vui lòng nhập tên phòng!", "Cảnh báo");
                return;
            }
            if (LoaiPhongDuocChon == null)
            {
                MessageBox.Show("Vui lòng chọn loại phòng!", "Cảnh báo");
                return;
            }
            if (DanhSachGhe.Count == 0)
            {
                MessageBox.Show("Phòng chiếu phải có ít nhất 1 ghế!", "Cảnh báo");
                return;
            }

            try
            {
                int maTT = (ChonTinhTrang == "Hoạt động") ? 1 : 2;

                var newPhong = new PhongChieu
                {
                    MaPhong = this.MaPhong,
                    TenPhong = this.TenPhong,
                    // Đã bỏ int.Parse vì LoaiPhongRepository lấy lên chuỗi string
                    MaLoaiPhong = LoaiPhongDuocChon.MaLoaiPhong,
                    MaTinhTrang = maTT.ToString(),
                    GhiChu = this.GhiChu
                };

                // AddPhongChieu sẽ nhận dữ liệu phòng và mảng ghế để insert
                if (_phongChieuRepo.AddPhongChieu(newPhong, DanhSachGhe))
                {
                    MessageBox.Show("Lưu thông tin phòng chiếu thành công!");
                    PhongChieuMoi(); // Thay thế cho ResetForm()
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
    }
}