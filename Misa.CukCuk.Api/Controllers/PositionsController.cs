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
    public class PositionsController : ControllerBase
    {
        /// <summary>
        /// Hàm lấy thông tin toàn bộ chức vụ
        /// </summary>
        /// <returns>Mảng thông tin toàn bộ chức vụ</returns>
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
                var sqlCommand = "SELECT * FROM Position";
                var positions = dbConnection.Query<object>(sqlCommand);

                // 4. Trả về cho Client
                if (positions.Count() > 0)
                {
                    var response = StatusCode(200, positions);
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
        /// Hàm lấy thông tin của chức vụ theo Id
        /// </summary>
        /// <param name="positionId"></param>
        /// <returns>Thông tin của chức vụ</returns>
        [HttpGet("{positionId}")]
        public IActionResult GetById(Guid positionId)
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
                dynamicParameters.Add("@PositionIdParam", positionId);

                var sqlCommand = "SELECT * FROM Position WHERE positionId = @PositionIdParam";
                var position = dbConnection.QueryFirstOrDefault<object>(sqlCommand, dynamicParameters);

                // 4. Trả về cho Client
                if (position == null)
                {
                    return StatusCode(204, Properties.ResourcesVN.EmptyData_VN);
                }
                else
                {
                    return StatusCode(200, position);
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
        /// Hàm tạo chức vụ
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreatePosition([FromBody] Position position)
        {
            try
            {
                // Kiểm tra thông tin của chức vụ đã hợp lệ hay chưa?

                // 1. Tên chức vụ bắt buộc phải có
                if (position.PositionName == "" || position.PositionName == null)
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

                // 2. Mã chức vụ không được để trống
                if (position.PositionCode == "" || position.PositionCode == null)
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

                // Sinh PositionId
                position.PositionId = Guid.NewGuid();

                // Đọc từng property của object
                var properties = position.GetType().GetProperties();

                // Duyệt từng property
                foreach (var property in properties)
                {
                    // Lấy tên của property
                    var propName = property.Name;

                    // Lấy value của property
                    var propValue = property.GetValue(position);

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

                var sqlCommand = $"INSERT INTO Position({columnsName}) VALUES({columnsParam})";
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
        /// Hàm sửa thông tin chức vụ
        /// </summary>
        /// <param name="positionId"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        [HttpPut("{positionId}")]
        public IActionResult UpdatePosition([FromRoute] Guid positionId, [FromBody] Position position)
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
                var properties = position.GetType().GetProperties();

                // Duyệt từng property
                foreach (var property in properties)
                {
                    // Lấy tên của property
                    var propName = property.Name;

                    // Lấy value của property
                    var propValue = property.GetValue(position);

                    // Lấy kiểu của property
                    var propType = property.PropertyType;

                    // Thêm param tương ứng với mỗi property của object
                    dynamicParameters.Add($"@{propName}", propValue);
                    columnsName += $"{propName} = @{propName},";
                }

                // Loại bỏ dấu phẩy
                columnsName = columnsName.Remove(columnsName.Length - 1, 1);
                dynamicParameters.Add("@PositionIdParam", positionId);
                var sqlCommand = $"UPDATE Position SET {columnsName} WHERE positionId = @PositionIdParam";
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
        /// Hàm xóa chức vụ theo Id
        /// </summary>
        /// <param name="positionId"></param>
        /// <returns></returns>
        [HttpDelete("{positionId}")]
        public IActionResult DeleteById(Guid positionId)
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
                dynamicParameters.Add("@PositionIdParam", positionId);

                var sqlCommand = "DELETE FROM Position WHERE positionId = @PositionIdParam";
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
