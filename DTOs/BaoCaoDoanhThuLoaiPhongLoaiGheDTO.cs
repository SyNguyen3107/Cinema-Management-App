using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema_Management_App.DTOs
{
    public class BaoCaoDoanhThuLoaiPhongLoaiGheDTO
    {
        public int STT { get; set; }

        public string TenLoaiPhong { get; set; } = string.Empty;

        public string TenLoaiGhe { get; set; } = string.Empty;

        public decimal DoanhThu { get; set; }

        public double TyLeDongGopDoanhThu { get; set; }
    }
}
