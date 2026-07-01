using Cinema_Management_App.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cinema_Management_App.Interfaces
{
    public interface INhanPhimRepository
    {
        Task<IEnumerable<NhanPhim>> GetAllNhanPhimAsync();
        Task<NhanPhim?> GetByIdAsync(int maNhanPhim);
    }
}