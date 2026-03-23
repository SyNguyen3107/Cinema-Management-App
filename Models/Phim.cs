using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema_Management_App.Models
{
    public class Phim
    {
        public int MaPhim { get; set; }
        public string TenPhim { get; set; }
        public int ThoiLuong { get; set; }
        public int MaNhanPhim { get; set; }
        public string TenDaoDien { get; set; }
        public string TenDienVienChinh { get; set; }
        public DateTime NgayKhoiChieu { get; set; }
<<<<<<< HEAD

        public int[] MaTheLoai { get; set; } = Array.Empty<int>();
=======
>>>>>>> 523c6e7 (feat: add database models for cinema management)
    }
}