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
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CustomerGroupsController : ControllerBase
    {
        /// <summary>
        /// Hàm lấy thông tin toàn bộ nhóm khách hàng
        /// </summary>
        /// <returns>Mảng thông tin toàn bộ nhóm khách hàng</returns>
        [HttpGet]
        public IActionResult GetAll()
        {

            try
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
                var customers = dbConnection.Query<object>(sqlCommand);

                // 4. Trả về cho Client
                if (customers.Count() > 0)
                {
                    var response = StatusCode(200, customers);
                    return response;
                }
                else
                {
                    return StatusCode(204, Properties.ResourcesVN.EmptyData_VN);
                }
            }
            catch (Exception ex)
            {
                var errorObj = new
                {
                    devMsg = ex.Message,
                    userMsg = Properties.ResourcesVN.Exception_ErrorMsg_VN,
                    errorCode = "misa-001",
                    moreInfo = "https://openapi.misa.com.vn/errorcode/misa-001",
                    traceId = "ba9587fd-1a79-4ac5-a0ca-2c9f74dfd3fb"
                };
                return StatusCode(500, errorObj);
            }
        }

        /// <summary>
        /// Hàm lấy thông tin của nhóm khách hàng theo Id
        /// </summary>
        /// <param name="customerGroupId"></param>
        /// <returns>Thông tin của nhóm khách hàng</returns>
        [HttpGet("{customerGroupId}")]
        public IActionResult GetById(Guid customerGroupId)
        {
            try
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
                dynamicParameters.Add("@CustomerGroupIdParam", customerGroupId);

                var sqlCommand = "SELECT * FROM CustomerGroup WHERE customerGroupId = @CustomerGroupIdParam";
                var customer = dbConnection.QueryFirstOrDefault<object>(sqlCommand, dynamicParameters);

                // 4. Trả về cho Client
                if (customer == null)
                {
                    return StatusCode(204, Properties.ResourcesVN.EmptyData_VN);
                }
                else
                {
                    return StatusCode(200, customer);
                }
            }
            catch (Exception ex)
            {
                var errorObj = new
                {
                    devMsg = ex.Message,
                    userMsg = Properties.ResourcesVN.Exception_ErrorMsg_VN,
                    errorCode = "misa-001",
                    moreInfo = "https://openapi.misa.com.vn/errorcode/misa-001",
                    traceId = "ba9587fd-1a79-4ac5-a0ca-2c9f74dfd3fb"
                };
                return StatusCode(500, errorObj);
            }
        }

        /// <summary>
        /// Hàm tạo nhóm khách hàng mới
        /// </summary>
        /// <param name="customerGroup"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateCustomer([FromBody] CustomerGroup customerGroup)
        {
            try
            {
                // Kiểm tra thông tin của nhóm khách hàng đã hợp lệ hay chưa?

                // 1. Tên nhóm khách hàng bắt buộc phải có
                if (customerGroup.CustomerGroupName == "" || customerGroup.CustomerGroupName == null)
                {
                    var errorObj = new
                    {
                        userMsg = Properties.ResourcesVN.Error_EmptyInput_VN,
                        errorCode = "misa-002",
                        moreInfo = "https://openapi.misa.com.vn/errorcode/misa-001",
                        traceId = "ba9587fd-1a79-4ac5-a0ca-2c9f74dfd3fb"
                    };
                    return BadRequest(errorObj);
                }

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

                // Sinh customerGroupId
                customerGroup.CustomerGroupId = Guid.NewGuid();

                // Đọc từng property của object
                var properties = customerGroup.GetType().GetProperties();

                // Duyệt từng property
                foreach (var property in properties)
                {
                    // Lấy tên của property
                    var propName = property.Name;

                    // Lấy value của property
                    var propValue = property.GetValue(customerGroup);

                    // Lấy kiểu của property
                    var propType = property.PropertyType;

                    // Thêm param tương ứng với mỗi property của object
                    dynamicParameters.Add($"@{propName}", propValue);

                    columnsName += $"{propName},";
                    columnsParam += $"@{propName},";
                }

                // Loại bỏ dấu phẩy
                columnsName = columnsName.Remove(columnsName.Length - 1, 1);
                columnsParam = columnsParam.Remove(columnsParam.Length - 1, 1);

                var sqlCommand = $"INSERT INTO CustomerGroup({columnsName}) VALUES({columnsParam})";
                var rowEffects = dbConnection.Execute(sqlCommand, param: dynamicParameters);

                // 4. Trả về cho Client
                if (rowEffects > 0)
                {
                    return StatusCode(201, Properties.ResourcesVN.Created_VN);
                }
                else
                {
                    return BadRequest(Properties.ResourcesVN.Duplicate_VN);
                }
            }
            catch (Exception ex)
            {
                var errorObj = new
                {
                    devMsg = ex.Message,
                    userMsg = Properties.ResourcesVN.Exception_ErrorMsg_VN,
                    errorCode = "misa-001",
                    moreInfo = "https://openapi.misa.com.vn/errorcode/misa-001",
                    traceId = "ba9587fd-1a79-4ac5-a0ca-2c9f74dfd3fb"
                };
                return StatusCode(500, errorObj);
            }
        }

        /// <summary>
        /// Hàm sửa thông tin nhóm khách hàng
        /// </summary>
        /// <param name="customerGroupId"></param>
        /// <param name="customerGroup"></param>
        /// <returns></returns>
        [HttpPut("{customerGroupId}")]
        public IActionResult UpdateCustomer([FromRoute] Guid customerGroupId, [FromBody] CustomerGroup customerGroup)
        {
            try
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
                var properties = customerGroup.GetType().GetProperties();

                // Duyệt từng property
                foreach (var property in properties)
                {
                    // Lấy tên của property
                    var propName = property.Name;

                    // Lấy value của property
                    var propValue = property.GetValue(customerGroup);

                    // Lấy kiểu của property
                    var propType = property.PropertyType;

                    // Thêm param tương ứng với mỗi property của object
                    dynamicParameters.Add($"@{propName}", propValue);
                    columnsName += $"{propName} = @{propName},";
                }

                // Loại bỏ dấu phẩy
                columnsName = columnsName.Remove(columnsName.Length - 1, 1);
                dynamicParameters.Add("@CustomerGroupIdParam", customerGroupId);
                var sqlCommand = $"UPDATE CustomerGroup SET {columnsName} WHERE customerGroupId = @CustomerGroupIdParam";
                var rowEffects = dbConnection.Execute(sqlCommand, param: dynamicParameters);

                // 4. Trả về cho Client
                if (rowEffects > 0)
                {
                    return StatusCode(200, Properties.ResourcesVN.Updated_VN);
                }
                else
                {
                    return StatusCode(204, Properties.ResourcesVN.EmptyData_VN);
                }
            }
            catch (Exception ex)
            {
                var errorObj = new
                {
                    devMsg = ex.Message,
                    userMsg = Properties.ResourcesVN.Exception_ErrorMsg_VN,
                    errorCode = "misa-001",
                    moreInfo = "https://openapi.misa.com.vn/errorcode/misa-001",
                    traceId = "ba9587fd-1a79-4ac5-a0ca-2c9f74dfd3fb"
                };
                return StatusCode(500, errorObj);
            }
        }

        /// <summary>
        /// Hàm xóa nhóm khách hàng theo Id
        /// </summary>
        /// <param name="customerGroupId"></param>
        /// <returns></returns>
        [HttpDelete("{customerGroupId}")]
        public IActionResult DeleteById(Guid customerGroupId)
        {
            try
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
                dynamicParameters.Add("@CustomerGroupIdParam", customerGroupId);

                var sqlCommand = "DELETE FROM CustomerGroup WHERE customerGroupId = @CustomerGroupIdParam";
                var rowEffects = dbConnection.Execute(sqlCommand, param: dynamicParameters);

                // 4. Trả về cho Client
                if (rowEffects > 0)
                {
                    return StatusCode(200, Properties.ResourcesVN.Deleted_VN);
                }
                else
                {
                    return StatusCode(204, Properties.ResourcesVN.EmptyData_VN);
                }
            }
            catch (Exception ex)
            {
                var errorObj = new
                {
                    devMsg = ex.Message,
                    userMsg = Properties.ResourcesVN.Exception_ErrorMsg_VN,
                    errorCode = "misa-001",
                    moreInfo = "https://openapi.misa.com.vn/errorcode/misa-001",
                    traceId = "ba9587fd-1a79-4ac5-a0ca-2c9f74dfd3fb"
                };
                return StatusCode(500, errorObj);
            }
        }
    }
}
