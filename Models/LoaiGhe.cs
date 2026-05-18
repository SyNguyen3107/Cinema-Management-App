using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema_Management_App.Models
{
    public partial class LoaiGhe
    {
        public string MaLoaiGhe { get; set; } = string.Empty;
        public string TenLoaiGhe { get; set; } = string.Empty;
        public decimal DonGia { get; set; }
    }
}
