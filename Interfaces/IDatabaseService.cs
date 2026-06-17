using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Cinema_Management_App.Services
{
    public interface IDatabaseService
    {
        DbConnection CreateConnection();

        // Synchronous methods
        DataTable ExecuteQuery(string query,DbParameter[] parameters = null);
        int ExecuteNonQuery(string query, DbParameter[] parameters = null);
        object ExecuteScalar(string query, DbParameter[] parameters = null);

        // Asynchronous methods to prevent UI freezing
        Task<DataTable> ExecuteQueryAsync(string query, DbParameter[] parameters = null);
        Task<int> ExecuteNonQueryAsync(string query, DbParameter[] parameters = null);
        Task<object> ExecuteScalarAsync(string query, DbParameter[] parameters = null);
    }
}