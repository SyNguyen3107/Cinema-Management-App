using Cinema_Management_App.Models;
using Cinema_Management_App.DTOs;
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

        Task<bool> ExistsAsync(string maGhe);

        Task<Ghe?> GetByIdAsync(string maGhe);

        Task<string> GenerateMaGhe();

        Task<IEnumerable<GheDTO>> GetAvailableByRoomId(string maPhong, string maSuatChieu);

        Task<IEnumerable<GheDTO>> GetAllGheDTOByRoomIdAsync(string maPhong);
    }
}
