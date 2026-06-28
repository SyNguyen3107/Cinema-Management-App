namespace Cinema_Management_App.DTOs
{
    public class TraCuuPhongChieuDTO
    {
        public int STT { get; set; }
        public string MaPhong { get; set; } = string.Empty;
        public string TenPhong { get; set; } = string.Empty;
        public string LoaiPhong { get; set; } = string.Empty;
        public string TinhTrangPhong { get; set; } = string.Empty;
        public int SoLuongGhe { get; set; }
        public decimal TongThanhTien { get; set; }
        public string GhiChu { get; set; } = string.Empty;
    }
}