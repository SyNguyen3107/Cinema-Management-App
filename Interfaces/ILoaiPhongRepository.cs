using Cinema_Management_App.Models;
using Cinema_Management_App.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cinema_Management_App.Interfaces
{
    public interface ILoaiPhongRepository
    {
        Task<IEnumerable<LoaiPhong>> GetAllLoaiPhongAsync();
        Task<LoaiPhong?> GetByIdAsync(string maLoaiPhong);
        Task<IEnumerable<BaoCaoDoanhThuLoaiPhongLoaiGheDTO>> GetRevenueReportByRoomTypeAndSeatTypeAsync(int month, int year);
    }
}