using Cinema_Management_App.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cinema_Management_App.Interfaces
{
    public interface ITheLoaiRepository
    {
        Task<IEnumerable<TheLoai>> GetAllTheLoaiAsync();
        Task<TheLoai?> GetByIdAsync(string maTheLoai);
    }
}