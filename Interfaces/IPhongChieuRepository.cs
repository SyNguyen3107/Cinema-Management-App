using Cinema_Management_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema_Management_App.Interfaces
{
    public interface IPhongChieuRepository
    {
        Task<bool> AddPhongChieuAsync(
            PhongChieu phong,
            IEnumerable<Ghe> dsGhe);

        Task<IEnumerable<PhongChieu>> GetAllAsync();

        Task<PhongChieu?> GetByIdAsync(string maPhong);

        Task<bool> UpdateAsync(PhongChieu phong);

        Task<bool> DeleteAsync(string maPhong);

        Task<bool> ExistsAsync(string maPhong);
         Task<string> GenerateMaPhong();
    }
}
