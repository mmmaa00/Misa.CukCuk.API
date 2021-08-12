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
    public class DepartmentsController : ControllerBase
    {
        /// <summary>
        /// Hàm lấy thông tin toàn bộ phòng ban
        /// </summary>
        /// <returns>Mảng thông tin toàn bộ phòng ban</returns>
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
                var sqlCommand = "SELECT * FROM Department";
                var departments = dbConnection.Query<object>(sqlCommand);

                // 4. Trả về cho Client
                if (departments.Count() > 0)
                {
                    var response = StatusCode(200, departments);
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
        /// Hàm lấy thông tin của phòng ban theo Id
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns>Thông tin của phòng ban</returns>
        [HttpGet("{departmentId}")]
        public IActionResult GetById(Guid departmentId)
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
                dynamicParameters.Add("@DepartmentIdParam", departmentId);

                var sqlCommand = "SELECT * FROM Department WHERE departmentId = @DepartmentIdParam";
                var department = dbConnection.QueryFirstOrDefault<object>(sqlCommand, dynamicParameters);

                // 4. Trả về cho Client
                if (department == null)
                {
                    return StatusCode(204, Properties.ResourcesVN.EmptyData_VN);
                }
                else
                {
                    return StatusCode(200, department);
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
        /// Hàm tạo phòng ban
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateDepartment([FromBody] Department department)
        {
            try
            {
                // Kiểm tra thông tin của phòng ban đã hợp lệ hay chưa?

                // 1. Tên phòng ban bắt buộc phải có
                if (department.DepartmentName == "" || department.DepartmentName == null)
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

                // 2. Mã phòng ban không được để trống
                if (department.DepartmentCode == "" || department.DepartmentCode == null)
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

                // Sinh departmentId
                department.DepartmentId = Guid.NewGuid();

                // Đọc từng property của object
                var properties = department.GetType().GetProperties();

                // Duyệt từng property
                foreach (var property in properties)
                {
                    // Lấy tên của property
                    var propName = property.Name;

                    // Lấy value của property
                    var propValue = property.GetValue(department);

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

                var sqlCommand = $"INSERT INTO Department({columnsName}) VALUES({columnsParam})";
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
        /// Hàm sửa thông tin phòng ban
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="department"></param>
        /// <returns></returns>
        [HttpPut("{departmentId}")]
        public IActionResult UpdateDepartment([FromRoute] Guid departmentId, [FromBody] Department department)
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
                var properties = department.GetType().GetProperties();

                // Duyệt từng property
                foreach (var property in properties)
                {
                    // Lấy tên của property
                    var propName = property.Name;

                    // Lấy value của property
                    var propValue = property.GetValue(department);

                    // Lấy kiểu của property
                    var propType = property.PropertyType;

                    // Thêm param tương ứng với mỗi property của object
                    dynamicParameters.Add($"@{propName}", propValue);
                    columnsName += $"{propName} = @{propName},";
                }

                // Loại bỏ dấu phẩy
                columnsName = columnsName.Remove(columnsName.Length - 1, 1);
                dynamicParameters.Add("@DepartmentIdParam", departmentId);
                var sqlCommand = $"UPDATE Department SET {columnsName} WHERE departmentId = @departmentIdParam";
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
        /// Hàm xóa phòng ban theo Id
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        [HttpDelete("{departmentId}")]
        public IActionResult DeleteById(Guid departmentId)
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
                dynamicParameters.Add("@DepartmentIdParam", departmentId);

                var sqlCommand = "DELETE FROM Department WHERE departmentId = @DepartmentIdParam";
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
