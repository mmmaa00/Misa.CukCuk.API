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
    public class CustomersController : ControllerBase
    {
        /// <summary>
        /// Lấy tất cả khách hàng
        /// </summary>
        /// <returns>Trả về danh sách toàn bộ khách hàng</returns>
        [HttpGet]
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

            var sqlCommand = "SELECT * FROM Customer";
            var customers = dbConnection.Query<object>(sqlCommand);

            // 4. Trả về cho Client

            var response = StatusCode(200, customers);
            return response;
        }

        /// <summary>
        /// Lấy khách hàng theo Id
        /// </summary>
        /// <returns>Trả về khách hàng có id theo yêu cầu tìm kiếm</returns>
        //[HttpGet("{customerId}")]
        //public IActionResult GetById(Guid customerId)
        //{
        //    Truy cập vào database

        //     1.Khai báo thông tin kết nối database:

        //    var connectionString = "Host = 47.241.69.179;" +
        //        "Database = MISA.CukCuk_Demo_NVMANH;" +
        //        "User Id = dev;" +
        //        "Password = 12345678";

        //    2.Khởi tạo đối tượng kết nối với Database

        //  IDbConnection dbConnection = new MySqlConnection(connectionString);

        //    3.Lấy dữ liệu

        //    var sqlCommand = $"SELECT * FROM Customer WHERE customerId = '{customerId.ToString()}' ";
        //    var customer = dbConnection.QueryFirstOrDefault<object>(sqlCommand);

        //    4.Trả về cho Client

        //  var response = StatusCode(200, customer);
        //    return response;
        //}

        [HttpGet("{customerId}")]
        public IActionResult GetById(Guid customerId)
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

            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@CustomerIdParam", customerId);
            var sqlCommand = "SELECT * FROM Customer WHERE customerId = @CustomerIdParam";
            var customer = dbConnection.QueryFirstOrDefault<object>(sqlCommand, dynamicParameters);

            // 4. Trả về cho Client

            var response = StatusCode(200, customer);
            return response;
        }

        /// <summary>
        /// Lấy khách hàng theo Code
        /// </summary>
        /// <returns>Trả về khách hàng có code theo yêu cầu tìm kiếm</returns>
        [HttpGet("getByCode")]
        public IActionResult GetByCode(string customerCode)
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

            var sqlCommand = $"SELECT * FROM Customer WHERE customerCode = '{customerCode}' ";
            var customer = dbConnection.QueryFirstOrDefault<object>(sqlCommand);

            // 4. Trả về cho Client

            var response = StatusCode(200, customer);
            return response;
        }

        /// <summary>
        /// Tạo khách hàng mới
        /// </summary>
        /// <returns>1 nếu thành công</returns>
        [HttpPost]
        public IActionResult CreateCustomer([FromBody]Customer customer)
        {
            // Truy cập vào database

            // 1. Khai báo thông tin kết nối database:

            var connectionString = "Host = 47.241.69.179;" +
                "Database = MISA.CukCuk_Demo_NVMANH;" +
                "User Id = dev;" +
                "Password = 12345678";

            // 2. Khởi tạo đối tượng kết nối với Database

            IDbConnection dbConnection = new MySqlConnection(connectionString);

            // 3. Thêm dữ liệu vào database

            var columnsName = string.Empty;
            var columnsParam = string.Empty;
            var dynamicParameters = new DynamicParameters();

            // Sinh customerId
            customer.CustomerId = Guid.NewGuid();
            // Đọc từng property của object
            var properties = customer.GetType().GetProperties();

            // Duyệt từng property
            foreach (var property in properties)
            {
                // Lấy tên của property
                var propName = property.Name;

                // Lấy value của property
                var propValue = property.GetValue(customer);

                // Lấy kiểu của property
                var propType = property.PropertyType;

                // Thêm param tương ứng với mỗi property của object
                dynamicParameters.Add($"@{propName}",propValue);

                columnsName+= $"{propName},";
                columnsParam+= $"@{propName},";
            }

            // Loại bỏ dấu phẩy
            columnsName = columnsName.Remove(columnsName.Length - 1, 1);
            columnsParam = columnsParam.Remove(columnsParam.Length - 1, 1);
     
            var sqlCommand = $"INSERT INTO Customer({columnsName}) VALUES({columnsParam})";
            var rowEffects = dbConnection.Execute(sqlCommand, param: dynamicParameters);
            
            // 4. Trả về cho Client

            var response = StatusCode(200, rowEffects);
            return response;
        }

        /// <summary>
        /// Sửa thông tin khách hàng
        /// </summary>
        /// <returns>1 nếu thành công</returns>
        [HttpPut("{customerId}")]
        public IActionResult UpdateCustomer(Guid customerId, Customer customer)
        {
            // Truy cập vào database

            // 1. Khai báo thông tin kết nối database:

            var connectionString = "Host = 47.241.69.179;" +
                "Database = MISA.CukCuk_Demo_NVMANH;" +
                "User Id = dev;" +
                "Password = 12345678";

            // 2. Khởi tạo đối tượng kết nối với Database

            IDbConnection dbConnection = new MySqlConnection(connectionString);

            // 3. Thêm dữ liệu vào database

            var columnsName = string.Empty;
            var columnsParam = string.Empty;
            var dynamicParameters = new DynamicParameters();

            // Đọc từng property của object
            var properties = customer.GetType().GetProperties();

            // Duyệt từng property
            foreach (var property in properties)
            {
                // Lấy tên của property
                var propName = property.Name;

                // Lấy value của property
                var propValue = property.GetValue(customer);

                // Lấy kiểu của property
                var propType = property.PropertyType;

                // Thêm param tương ứng với mỗi property của object
                dynamicParameters.Add($"@{propName}", propValue);
                columnsName += $"{propName} = @{propName},";
               
            }

            // Loại bỏ dấu phẩy
            columnsName = columnsName.Remove(columnsName.Length - 1, 1);
            dynamicParameters.Add("@CustomerIdParam", customerId);
            var sqlCommand = $"UPDATE Customer SET {columnsName} WHERE customerId = @CustomerIdParam";
            var rowEffects = dbConnection.Execute(sqlCommand, param: dynamicParameters);

            // 4. Trả về cho Client

            var response = StatusCode(200, rowEffects);
            return response;
        }

        /// <summary>
        /// Xóa khách hàng theo id
        /// </summary>
        /// <returns>1 nếu thành công</returns>
        [HttpDelete("{customerId}")]
        public IActionResult DeleteById(Guid customerId)
        {
            // Truy cập vào database

            // 1. Khai báo thông tin kết nối database:

            var connectionString = "Host = 47.241.69.179;" +
                "Database = MISA.CukCuk_Demo_NVMANH;" +
                "User Id = dev;" +
                "Password = 12345678";

            // 2. Khởi tạo đối tượng kết nối với Database

            IDbConnection dbConnection = new MySqlConnection(connectionString);

            // 3. Xóa dữ liệu

            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@CustomerIdParam", customerId);

            var sqlCommand = "DELETE FROM Customer WHERE customerId = @CustomerIdParam";
            var rowEffects = dbConnection.Execute(sqlCommand, param: dynamicParameters);

            // 4. Trả về cho Client

            var response = StatusCode(200, rowEffects);
            return response;
        }
    }
}
