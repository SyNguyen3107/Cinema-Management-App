using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema_Management_App.Services
{
    //Đây là class duy nhất có quyền truy cập vào DB, các class khác muốn truy cập DB phải thông qua class này
    
    public class MySQLService
    {
        private readonly string _connectionString;
        public MySQLService()
        {
            // Lấy chuỗi kết nối từ App.config thông qua tên "AivenMySQL"
            _connectionString = ConfigurationManager.ConnectionStrings["AivenMySQL"].ConnectionString;
        }
    }
}
