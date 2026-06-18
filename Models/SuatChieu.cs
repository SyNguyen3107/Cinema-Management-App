using System;

namespace Cinema_Management_App.Models
{
    public class SuatChieu
    {
        public string MaSuatChieu { get; set; } = string.Empty;

        public DateTime NgayChieu { get; set; } = DateTime.Now.Date;

        public TimeSpan GioBatDau { get; set; }

        public TimeSpan GioKetThuc { get; set; }
        public string MaPhim { get; set; } = string.Empty;
        public string MaPhong { get; set; } = string.Empty;
    }
}