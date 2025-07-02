using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using MyCURDApp.Models;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace MyCURDApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : Controller
    {
        private readonly IConfiguration _configuration;
        public DepartmentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult GetAllDepartments()
        {
            string query = @"select DepartmentId, DepartmentName, DepartmentSize from Department";
            DataTable dataTable = new DataTable();
            string connString = _configuration.GetConnectionString("AppDBConnString");

            MySqlDataReader myReader;

            MySqlConnection conn = new MySqlConnection(connString);
            conn.Open();

            MySqlCommand cmd = new MySqlCommand(query, conn);
            myReader = cmd.ExecuteReader();

            dataTable.Load(myReader);

            myReader.Close();
            conn.Close();

            return Json(dataTable);
        }

        [HttpGet]
        [Route("withId")]
        public JsonResult GetDepartmentById(int departmentId)
        {
            string query = @"select * from Department where DepartmentId = @dID";
            string connString = _configuration.GetConnectionString("AppDBConnString");

            MySqlDataReader myReader;

            MySqlConnection conn = new MySqlConnection(connString);
            conn.Open();

            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@dID", departmentId);
            myReader = cmd.ExecuteReader();

            myReader.Read();
            Department res = new Department
            (
            Convert.ToInt32(myReader["DepartmentId"]),
            myReader["DepartmentName"] as string,
            Convert.ToInt32(myReader["DepartmentSize"])
            );
                
            myReader.Close();
            conn.Close();

            return Json(res);
        }

        [HttpPost]
        public JsonResult CreateNewDepartment(Department department)
        {
            string query = @"
                            insert into Department(DepartmentName, DepartmentSize) value (@DepartmentName, @DepartmentSize);
                            SELECT LAST_INSERT_ID();
                           ";
            string connString = _configuration.GetConnectionString("AppDBConnString");

            MySqlConnection conn = new MySqlConnection(connString);
            conn.Open();

            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@DepartmentName", department.DepartmentName);
            cmd.Parameters.AddWithValue("@DepartmentSize", department.DepartmentSize);
            object cmdRes = cmd.ExecuteScalar();
            int newId = Convert.ToInt32(cmdRes);
            conn.Close();

            department.DepartmentId = newId;
            return Json(department);
        }

        [HttpPut]
        public JsonResult UpdateDepartmentWithId(Department newDepartment, int oldDepartmentId)
        {
            string query = @"
                            update Department set DepartmentName = @DepartmentName, DepartmentSize = @DepartmentSize where DepartmentId = @OldDepartmentId;
                           ";
            string connString = _configuration.GetConnectionString("AppDBConnString");

            MySqlConnection conn = new MySqlConnection(connString);
            conn.Open();

            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@DepartmentName", newDepartment.DepartmentName);
            cmd.Parameters.AddWithValue("@DepartmentSize", newDepartment.DepartmentSize);
            cmd.Parameters.AddWithValue("@OldDepartmentId", oldDepartmentId);
            cmd.ExecuteNonQuery();
            conn.Close();

            newDepartment.DepartmentId = oldDepartmentId;
            return Json(newDepartment);
        }

        [HttpDelete]
        public JsonResult DeleteDepartmentWithId(int departmentId, bool isGetOldData = true)
        {
            string connString = _configuration.GetConnectionString("AppDBConnString");
            MySqlConnection conn = new MySqlConnection(connString);
            conn.Open();

            DataTable department = null;
            if (isGetOldData)
            {
                department = new DataTable();
                string dataQuery = @"select * from Department where DepartmentId = @DepartmentId;";
                MySqlCommand dataCmd = new MySqlCommand(dataQuery, conn);
                dataCmd.Parameters.AddWithValue("@DepartmentId", departmentId);
                MySqlDataReader myReader = dataCmd.ExecuteReader();
                department.Load(myReader);
            }

            string query = @"
                            delete from Department where DepartmentId = @OldDepartmentId;
                           ";

            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@OldDepartmentId", departmentId);
            cmd.ExecuteNonQuery();
            conn.Close();

            return Json(isGetOldData ? department : "Department deleted!");
        }
    }
}
