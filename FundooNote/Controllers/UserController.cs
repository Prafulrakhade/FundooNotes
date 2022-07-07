using BusinessLayer.Interface;
using DatabaseLayer;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace FundooNote.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        IUserBL userBL;
        public UserController(IUserBL userBL)
        {
            this.userBL = userBL;
        }
        [HttpPost("Register")]
        public IActionResult AddUser(UserModel userModel)
        {
            try
            {
                this.userBL.AddUser(userModel);
                return this.Ok(new { success = true, Message = "User Registration Sucessfull" });

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            try
            {
                List<UserResponseModel> users = new List<UserResponseModel>();
                users = this.userBL.GetAllUsers();
                return Ok(new { success = true, Message = "All Users fetch successfully", data = users });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
