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
                List<UserResponseModel> userModel = new List<UserResponseModel>();
                userModel = this.userBL.GetAllUsers();
                return Ok(new { success = true, Message = "All Users fetch successfully", data = userModel });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost("Login")]
        public IActionResult CheckUser(LoginUserModel user)
        {
            try
            {
                string result = this.userBL.LoginUser(user);
                return Ok(new { success = true, Message = "Token Generated successfully", data = result });

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}