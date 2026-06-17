using Cinema_Management_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema_Management_App.Interfaces
{
    public interface IGheRepository
    {
        Task<bool> AddGheAsync(
            Ghe ghe,
            IEnumerable<Ghe> dsGhe);

        Task<IEnumerable<Ghe>> GetAllAsync();

        Task<Ghe?> GetByIdAsync(string maGhe);

        Task<bool> UpdateAsync(Ghe ghe);

        Task<bool> DeleteAsync(string maGhe);

        Task<bool> ExistsAsync(string maGhe);
        string GenerateMaGhe();
    }
}
