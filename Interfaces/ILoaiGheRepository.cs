using Cinema_Management_App.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cinema_Management_App.Interfaces
{
    public interface ILoaiGheRepository
    {
        Task<IEnumerable<LoaiGhe>> GetAllLoaiGheAsync();
        Task<IEnumerable<LoaiGhe>> GetAllLoaiGheByMaLoaiPhongAsync(string maLoaiPhong);
    }
}