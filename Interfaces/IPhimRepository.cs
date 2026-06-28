using Cinema_Management_App.Models;
using Cinema_Management_App.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cinema_Management_App.Interfaces
{
    public interface IPhimRepository
    {
        Task<IEnumerable<Phim>> GetAllAsync();
        Task<Phim?> GetByIdAsync(string maPhim);
        Task<bool> AddPhimAsync(Phim phimMoi);
        Task<bool> UpdateAsync(Phim phim);
        Task<bool> DeleteAsync(string maPhim);
        Task<bool> ExistsAsync(string maPhim);
        Task<string> GenerateMaPhimAsync();
        Task<IEnumerable<BaoCaoDoanhThuPhimDTO>> GetMoviesRevenueReportsByMonthYear(int month, int year);
    }
}