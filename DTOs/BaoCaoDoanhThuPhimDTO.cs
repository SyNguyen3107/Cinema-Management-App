using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema_Management_App.DTOs
{
    public class BaoCaoDoanhThuPhimDTO
    {
        public int STT { get; set; }

        public string TenPhim { get; set; } = string.Empty;

        public decimal DoanhThu { get; set; }

        public double TyLeLapDayGhe { get; set; }
    }
}
