using Cinema_Management_App.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cinema_Management_App.Interfaces
{
    public interface IVeRepository
    {
        // Add a new ticket along with its selected seats inside a secure transaction
        Task<bool> AddVeAsync(Ve ve, IEnumerable<string> danhSachMaGhe);

        // Generate a unique ticket ID
        Task<string> GenerateMaVeAsync();

        // Check if a ticket ID already exists
        Task<bool> ExistsAsync(string maVe);

        // Retrieve all tickets
        Task<IEnumerable<Ve>> GetAllAsync();

        // Retrieve a specific ticket by ID
        Task<Ve?> GetByIdAsync(string maVe);

        Task<IEnumerable<string>> GetDanhSachGheDaBanAsync(string maSuatChieu);
    }
}