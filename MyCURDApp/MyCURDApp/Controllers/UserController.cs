using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MyCURDApp.Models;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;

namespace MyCURDApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public UserController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [HttpGet]
        public JsonResult GetAllUsers()
        {
            string query = @"select * from User";
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

        [HttpPost]
        public JsonResult CreateNewUser(User user)
        {
            string query = @"
                            insert into User(FullName, DateOfBirth, Avatar, DepartmentId) value (@FullName, @DateOfBirth, @Avatar, @DepartmentId);
                            SELECT LAST_INSERT_Id();
                           ";
            string connString = _configuration.GetConnectionString("AppDBConnString");

            MySqlConnection conn = new MySqlConnection(connString);
            conn.Open();

            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@FullName", user.FullName);
            cmd.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth);
            cmd.Parameters.AddWithValue("@Avatar", user.Avatar);
            cmd.Parameters.AddWithValue("@DepartmentId", user.DepartmentId);
            object cmdRes = cmd.ExecuteScalar();
            int newId = Convert.ToInt32(cmdRes);
            conn.Close();

            user.DepartmentId = newId;
            return Json(user);
        }

        [HttpPut]
        public JsonResult UpdateUserWithId(User newUser, int oldUId)
        {
            string query = @"
                            update User 
                                set FullName = @FullName, DateOfBirth = @DateOfBirth, Avatar = @Avatar, DepartmentId = @DepartmentId
                                where UserId = @oldUId;
                           ";
            string connString = _configuration.GetConnectionString("AppDBConnString");

            MySqlConnection conn = new MySqlConnection(connString);
            conn.Open();

            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@FullName", newUser.FullName);
            cmd.Parameters.AddWithValue("@DateOfBirth", newUser.DateOfBirth);
            cmd.Parameters.AddWithValue("@Avatar", newUser.Avatar);
            cmd.Parameters.AddWithValue("@DepartmentId", newUser.DepartmentId);
            cmd.Parameters.AddWithValue("@oldUId", oldUId);

            conn.Close();

            newUser.UserId = oldUId;
            return Json(newUser);
        }

        [HttpDelete]
        public JsonResult DeleteUserWithId(int UId, bool isGetOldData = true)
        {
            string connString = _configuration.GetConnectionString("AppDBConnString");
            MySqlConnection conn = new MySqlConnection(connString);
            conn.Open();

            DataTable user = null;
            if (isGetOldData)
            {
                user = new DataTable();
                string dataQuery = @"select * from User where UserId = @UId;";
                MySqlCommand dataCmd = new MySqlCommand(dataQuery, conn);
                dataCmd.Parameters.AddWithValue("@UId", UId);
                MySqlDataReader myReader = dataCmd.ExecuteReader();
                user.Load(myReader);
            }

            string query = @"
                            delete from User where UserId = @UId;
                           ";

            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UId", UId);
            cmd.ExecuteNonQuery();
            conn.Close();

            return Json(isGetOldData ? user : "User deleted!");
        }

        [Route("saveFile")]
        [HttpPost]
        public JsonResult SaveFile(IFormFile file) { 
            try
            {
                if (file != null)
                {
                    string fileName = file.FileName;
                    var physcalFilePath = _env.ContentRootPath + "/Photos/" + fileName;
                    using (var stream = new FileStream(physcalFilePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        return Json(fileName); 
                    }
                }
                throw new Exception("Missing file to upload!");
            } catch (Exception e)
            {
                return Json("Error in saving file: " + e.Message);
            }
                
        }
    }
}
