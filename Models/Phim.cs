using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema_Management_App.Models
{
    public class Phim
    {
        public required string MaPhim { get; set; }
        public string? TenPhim { get; set; }
        public int ThoiLuong { get; set; }
        public required string MaNhanPhim { get; set; }
        public required string MaTheLoai { get; set; }
        public string? TenDaoDien { get; set; }
        public string? TenDienVienChinh { get; set; }
        public DateTime NgayKhoiChieu { get; set; }
    }
}
