using System.Configuration;
using MySql.Data.MySqlClient;

namespace Cinema_Management_App.Repositories
{
    public static class Database
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["AivenMySQL"].ConnectionString;

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}