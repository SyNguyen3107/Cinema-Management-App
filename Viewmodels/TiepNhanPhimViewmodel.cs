using Cinema_Management_App.Models;
using Cinema_Management_App.Repositories;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace Cinema_Management_App.Viewmodels
{
    public partial class TiepNhanPhimViewmodel : ObservableObject
    {
        private readonly PhimRepository _phimRepository;
        private readonly NhanPhimRepository _nhanPhimRepository;
        private readonly TheLoaiRepository _theLoaiRepository;

        [ObservableProperty] private string _maPhim = default!;
        [ObservableProperty] private string _tenPhim;
        [ObservableProperty] private string _tenDaoDien;
        [ObservableProperty] private string _tenDienVienChinh;
        [ObservableProperty] private int _thoiLuong;
        [ObservableProperty] private DateTime _ngayKhoiChieu = DateTime.Now;
        [ObservableProperty] private NhanPhim _nhanPhimDuocChon;
        [ObservableProperty] private TheLoai _theLoaiDuocChon;

        [ObservableProperty] private ObservableCollection<NhanPhim> _danhSachNhanPhim;
        [ObservableProperty] private ObservableCollection<TheLoai> _danhSachTheLoai;

        public TiepNhanPhimViewmodel(
            PhimRepository phimRepository,
            NhanPhimRepository nhanPhimRepository,
            TheLoaiRepository theLoaiRepository)
        {
            _phimRepository = phimRepository;
            _nhanPhimRepository = nhanPhimRepository;
            _theLoaiRepository = theLoaiRepository;

            if (_phimRepository != null)
            {
                MaPhim = _phimRepository.GetNewMaPhim();
            }
            LoadDuLieuTuDB();
        }

        [RelayCommand]
        private void TiepNhan()
        {
            if (!KiemTraThongTinPhim())
            {
                return;
            }

            Phim phimMoi = new Phim
            {
                MaPhim = MaPhim,
                TenPhim = this.TenPhim,
                TenDaoDien = this.TenDaoDien,
                TenDienVienChinh = this.TenDienVienChinh,
                ThoiLuong = this.ThoiLuong,
                NgayKhoiChieu = this.NgayKhoiChieu,
                MaNhanPhim = NhanPhimDuocChon?.MaNhanPhim ?? 0,
                MaTheLoai = TheLoaiDuocChon?.MaTheLoai ?? string.Empty
            };

            if (_phimRepository.AddPhim(phimMoi))
            {
                MessageBox.Show($"Đã tiếp nhận phim '{TenPhim}' thành công vào hệ thống!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                DatLai();
            }
            else
            {
                MessageBox.Show("Lưu phim thất bại! Hãy kiểm tra lại kết nối CSDL hoặc định dạng dữ liệu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void DatLai()
        {
            MaPhim = string.Empty;
            TenPhim = string.Empty;
            TenDaoDien = string.Empty;
            TheLoaiDuocChon = null;
            TenDienVienChinh = string.Empty;
            ThoiLuong = 0;
            NgayKhoiChieu = DateTime.Now;
            NhanPhimDuocChon = null;
        }

        [RelayCommand]
        private void XoaPhim()
        {
            MessageBox.Show("Chức năng xóa phim đang được phát triển.", "Thông báo");
        }

        [RelayCommand]
        private void CapnhatPhim()
        {
            MessageBox.Show("Chức năng cập nhật phim đang được phát triển.", "Thông báo");
        }

        [RelayCommand]
        private void TimPhim()
        {
            MessageBox.Show("Chức năng tìm kiếm phim đang được phát triển.", "Thông báo");
        }

        [RelayCommand]
        private void Thoat()
        {
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            window?.Close();
        }

        private void LoadDuLieuTuDB()
        {
            try
            {
                var listNhanPhim = _nhanPhimRepository.GetAllNhanPhim();
                var listTheLoai = _theLoaiRepository.GetAllTheLoai();

                DanhSachNhanPhim = new ObservableCollection<NhanPhim>(listNhanPhim);
                DanhSachTheLoai = new ObservableCollection<TheLoai>(listTheLoai);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu từ CSDL: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool KiemTraThongTinPhim()
        {
            if (string.IsNullOrWhiteSpace(TenPhim))
            {
                MessageBox.Show("Vui lòng nhập tên phim!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (NhanPhimDuocChon == null)
            {
                MessageBox.Show("Vui lòng chọn nhãn phim!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (TheLoaiDuocChon == null)
            {
                MessageBox.Show("Vui lòng chọn thể loại!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (ThoiLuong <= 0)
            {
                MessageBox.Show("Thời lượng phải lớn hơn 0!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (String.IsNullOrEmpty(TenDaoDien))
            {
                MessageBox.Show("Vui lòng nhập tên đạo diễn!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (String.IsNullOrEmpty(TenDienVienChinh))
            {
                MessageBox.Show("Vui lòng nhập tên diễn viên chính!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
    }
}