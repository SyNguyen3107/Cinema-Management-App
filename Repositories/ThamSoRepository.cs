using Cinema_Management_App.Interfaces;
using Cinema_Management_App.Services; // Thêm namespace chứa IDatabaseService nếu cần
using MySql.Data.MySqlClient;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Cinema_Management_App.Repositories
{
    public class ThamSoRepository : IThamSoRepository
    {
        private readonly IDatabaseService _dbService;

        public ThamSoRepository(IDatabaseService dbService)
        {
            _dbService = dbService;
        }

        public async Task<bool> LayThamSoDungSaiAsync(string maThamSo)
        {
            string query = "SELECT GiaTri FROM THAMSODUNGSAI WHERE MaThamSo = @ma";
            DbParameter[] parameters = new DbParameter[]
            {
                new MySqlParameter("@ma", maThamSo)
            };

            var result = await _dbService.ExecuteScalarAsync(query, parameters);

            if (result == null || result == DBNull.Value)
            {
                return false;
            }

            return Convert.ToBoolean(result);
        }

        public async Task<double> LayThamSoGiaTriAsync(string maThamSo)
        {
            string query = "SELECT GiaTri FROM THAMSOGIATRI WHERE MaThamSo = @ma";
            DbParameter[] parameters = new DbParameter[]
            {
                new MySqlParameter("@ma", maThamSo)
            };

            var result = await _dbService.ExecuteScalarAsync(query, parameters);

            if (result == null || result == DBNull.Value)
            {
                return 0.0;
            }

            return Convert.ToDouble(result);
        }
    }
}