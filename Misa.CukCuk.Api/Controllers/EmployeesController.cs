﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Misa.CukCuk.Api.Model;
using MySqlConnector;
using System.Data;
using Dapper;
using System.Text.RegularExpressions;

namespace Misa.CukCuk.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        /// <summary>
        /// Hàm lấy thông tin toàn bộ nhân viên
        /// </summary>
        /// <returns>Mảng thông tin toàn bộ nhân viên</returns>
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
                var sqlCommand = "SELECT * FROM Employee";
                var employees = dbConnection.Query<object>(sqlCommand);

                // 4. Trả về cho Client
                if (employees.Count() > 0)
                {
                    var response = StatusCode(200, employees);
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
        /// Hàm lấy thông tin của nhân viên theo Id
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns>Thông tin của nhân viên</returns>
        [HttpGet("{employeeId}")]
        public IActionResult GetById(Guid employeeId)
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
                dynamicParameters.Add("@EmployeeIdParam", employeeId);

                var sqlCommand = "SELECT * FROM Employee WHERE employeeId = @EmployeeIdParam";
                var employee = dbConnection.QueryFirstOrDefault<object>(sqlCommand, dynamicParameters);

                // 4. Trả về cho Client
                if (employee == null)
                {
                    return StatusCode(204, Properties.ResourcesVN.EmptyData_VN);
                }
                else
                {
                    return StatusCode(200, employee);
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
        /// Hàm tạo nhân viên mới
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateEmployee([FromBody] Employee employee)
        {
            try
            {
                // Kiểm tra thông tin của nhân viên đã hợp lệ hay chưa?

                // 1. Mã nhân viên bắt buộc phải có
                if (employee.EmployeeCode == "" || employee.EmployeeCode == null)
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

                // 2. Email phải đúng định dạng
                var emailFormat = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
                var isMatch = Regex.IsMatch(employee.Email, emailFormat, RegexOptions.IgnoreCase);
                if (isMatch == false)
                {
                    var errorObj = new
                    {
                        userMsg = Properties.ResourcesVN.Error_Input_VN,
                        errorCode = "misa-002",
                        moreInfo = "https://openapi.misa.com.vn/errorcode/misa-001",
                        traceId = "ba9587fd-1a79-4ac5-a0ca-2c9f74dfd3fb"
                    };
                    return BadRequest(errorObj);
                }

                // 3. Các trường bắt buộc phải có: Tên, số điện thoại
                bool TestForNullOrEmpty(string s)
                {
                    bool result;
                    result = s == null || s == string.Empty;
                    return result;
                }
                if (TestForNullOrEmpty(employee.FullName) || TestForNullOrEmpty(employee.PhoneNumber))
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

                // Sinh customerId
                employee.EmployeeId = Guid.NewGuid();

                // Đọc từng property của object
                var properties = employee.GetType().GetProperties();

                // Duyệt từng property
                foreach (var property in properties)
                {
                    // Lấy tên của property
                    var propName = property.Name;

                    // Lấy value của property
                    var propValue = property.GetValue(employee);

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

                var sqlCommand = $"INSERT INTO Employee({columnsName}) VALUES({columnsParam})";
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
        /// Hàm sửa thông tin nhân viên
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPut("{employeeId}")]
        public IActionResult UpdateEmployee([FromRoute] Guid employeeId, [FromBody] Employee employee)
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
                var properties = employee.GetType().GetProperties();

                // Duyệt từng property
                foreach (var property in properties)
                {
                    // Lấy tên của property
                    var propName = property.Name;

                    // Lấy value của property
                    var propValue = property.GetValue(employee);

                    // Lấy kiểu của property
                    var propType = property.PropertyType;

                    // Thêm param tương ứng với mỗi property của object
                    dynamicParameters.Add($"@{propName}", propValue);
                    columnsName += $"{propName} = @{propName},";
                }

                // Loại bỏ dấu phẩy
                columnsName = columnsName.Remove(columnsName.Length - 1, 1);
                dynamicParameters.Add("@EmployeeIdParam", employeeId);
                var sqlCommand = $"UPDATE Employee SET {columnsName} WHERE employeeId = @EmployeeIdParam";
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
        /// Hàm xóa nhân viên theo Id
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [HttpDelete("{employeeId}")]
        public IActionResult DeleteById(Guid employeeId)
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
                dynamicParameters.Add("@EmployeeIdParam", employeeId);

                var sqlCommand = "DELETE FROM Employee WHERE employeeId = @EmployeeIdParam";
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
