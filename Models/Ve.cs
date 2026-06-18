using System;

namespace Cinema_Management_App.Models
{
    public class Ve
    {
        public string MaVe { get; set; } = string.Empty;
        public DateTime NgayBan { get; set; } = DateTime.Now;
        public string TenNhanVienBan { get; set; } = string.Empty;
        public string HinhThucThanhToan { get; set; } = string.Empty;
        public string MaSuatChieu { get; set; } = string.Empty;
        public decimal TongTien { get; set; }
    }
}