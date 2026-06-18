using Cinema_Management_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema_Management_App.Interfaces
{
    public interface ISuatChieuRepository
    {
        Task<bool> AddSuatChieuAsync(SuatChieu suatChieu);
        Task<IEnumerable<SuatChieu>> GetAllAsync();
        Task<SuatChieu?> GetByIdAsync(string maSuatchieu);
        Task<bool> ExistsAsync(string maSuatchieu);
        Task<string> GenerateMaSuatChieu();

        Task<IEnumerable<SuatChieu>> GetByRoomIdAsync(string maPhong);

    }
}
