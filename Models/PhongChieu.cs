using System.Collections.ObjectModel;

namespace Cinema_Management_App.Models
{
    public class PhongChieu
    {
        public string MaPhong { get; set; }
        public string TenPhong { get; set; }
        public string TenLoaiPhong { get; set; }
        public string TenTinhTrang { get; set; }
        public string GhiChu { get; set; }
        public decimal TongGiaTri { get; set; }
        public ObservableCollection<Ghe> DanhSachGhe { get; set; } = new ObservableCollection<Ghe>();
    }
}
