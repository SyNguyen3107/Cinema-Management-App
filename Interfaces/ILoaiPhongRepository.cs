using Cinema_Management_App.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cinema_Management_App.Interfaces
{
    public interface ILoaiPhongRepository
    {
        Task<IEnumerable<LoaiPhong>> GetAllLoaiPhongAsync();
        Task<LoaiPhong?> GetByIdAsync(string maLoaiPhong);
    }
}