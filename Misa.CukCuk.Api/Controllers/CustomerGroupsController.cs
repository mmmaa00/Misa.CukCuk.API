using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Misa.CukCuk.Api.Model;
using MySqlConnector;
using System.Data;
using Dapper;

namespace Misa.CukCuk.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerGroupsController : ControllerBase
    {
        /// <summary>
        /// Lấy tất cả nhóm khách hàng
        /// </summary>
        /// <returns>Trả về danh sách toàn bộ nhóm khách hàng</returns>
        [HttpGet("getAll")]
        public IActionResult GetAll()
        {
            // Truy cập vào database

            // 1. Khai báo thông tin kết nối database:

            var connectionString = "Host = 47.241.69.179;" +
                "Database = MISA.CukCuk_Demo_NVMANH;" +
                "User Id = dev;" +
                "Password = 12345678";

            // 2. Khởi tạo đối tượng kết nối với Database

            IDbConnection dbConnection = new MySqlConnection(connectionString);

            // 3. Lấy dữ liệu

            var sqlCommand = "SELECT * FROM CustomerGroup";
            var cusGroups = dbConnection.Query<object>(sqlCommand);

            // 4. Trả về cho Client

            var response = StatusCode(200, cusGroups);
            return response;
        }

        /// <summary>
        /// Lấy nhóm khách hàng theo Id
        /// </summary>
        /// <returns>Trả về nhóm khách hàng có id theo yêu cầu tìm kiếm</returns>
        [HttpGet("getById")]
        public IActionResult GetById(Guid customerGroupId)
        {
            // Truy cập vào database

            // 1. Khai báo thông tin kết nối database:

            var connectionString = "Host = 47.241.69.179;" +
                "Database = MISA.CukCuk_Demo_NVMANH;" +
                "User Id = dev;" +
                "Password = 12345678";

            // 2. Khởi tạo đối tượng kết nối với Database

            IDbConnection dbConnection = new MySqlConnection(connectionString);

            // 3. Lấy dữ liệu

            var sqlCommand = $"SELECT * FROM CustomerGroup WHERE customerGroupId = '{customerGroupId.ToString()}'";
            var cusGroup = dbConnection.QueryFirstOrDefault<object>(sqlCommand);

            // 4. Trả về cho Client

            var response = StatusCode(200, cusGroup);
            return response;
        }
    }
}
