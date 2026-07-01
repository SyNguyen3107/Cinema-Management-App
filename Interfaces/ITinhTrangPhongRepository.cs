using Cinema_Management_App.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cinema_Management_App.Interfaces
{
    public interface ITinhTrangPhongRepository
    {
        Task<IEnumerable<TinhTrangPhong>> GetAllTinhTrangPhongAsync();
        Task<TinhTrangPhong?> GetByIdAsync(string maTinhTrangPhong);
    }
}