using System.Collections.ObjectModel;

namespace Cinema_Management_App.Models
{
    public class PhongChieu
    {
        public string MaPhong { get; set; }
        public string TenPhong { get; set; }
        public string MaLoaiPhong { get; set; }
        public string MaTinhTrang { get; set; }
        public string GhiChu { get; set; }
        public List<Ghe> DanhSachGhe { get; set; }
    }
}
