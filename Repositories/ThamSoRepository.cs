using Cinema_Management_App.Interfaces;
using Cinema_Management_App.Services;
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
        public async Task<bool> CapNhatThamSoDungSaiAsync(string maThamSo, bool giaTri)
        {
            string query = "UPDATE THAMSODUNGSAI SET GiaTri = @giaTri WHERE MaThamSo = @ma";

            int giaTriInt = giaTri ? 1 : 0;

            DbParameter[] parameters = new DbParameter[]
            {
                new MySqlParameter("@giaTri", giaTriInt),
                new MySqlParameter("@ma", maThamSo)
            };

            int rowsAffected = await _dbService.ExecuteNonQueryAsync(query, parameters);

            return rowsAffected > 0;
        }

        public async Task<bool> CapNhatThamSoGiaTriAsync(string maThamSo, double giaTri)
        {
            string query = "UPDATE THAMSOGIATRI SET GiaTri = @giaTri WHERE MaThamSo = @ma";
            DbParameter[] parameters = new DbParameter[]
            {
                new MySqlParameter("@giaTri", giaTri),
                new MySqlParameter("@ma", maThamSo)
            };

            int rowsAffected = await _dbService.ExecuteNonQueryAsync(query, parameters);
            return rowsAffected > 0;
        }
    }
}