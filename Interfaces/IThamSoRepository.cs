using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema_Management_App.Interfaces
{
    public interface IThamSoRepository
    {
        Task<bool> LayThamSoDungSaiAsync(string maThamSo);
        Task<double> LayThamSoGiaTriAsync(string maThamSo);

        Task<bool> CapNhatThamSoDungSaiAsync(string maThamSo, bool giaTri);
        Task<bool> CapNhatThamSoGiaTriAsync(string maThamSo, double giaTri);
    }
}
