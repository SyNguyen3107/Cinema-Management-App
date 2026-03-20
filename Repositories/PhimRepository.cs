using Cinema_Management_App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinema_Management_App.Models;

namespace Cinema_Management_App.Repositories
{
    public class PhimRepository
    {
        private readonly MySQLService _dbService;

        // Hệ thống DI tự động truyền MySQLService duy nhất vào đây
        public PhimRepository(MySQLService dbService)
        {
            _dbService = dbService;
        }

        public void AddPhim(Phim newPhim)
        {
            // Sử dụng _dbService để thực thi SQL
            //string query = "INSERT INTO Phim (TenPhim) VALUES (...)";
            //_dbService.ExecuteNonQuery(query);
        }
    }
}
