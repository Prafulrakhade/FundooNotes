using BusinessLayer.Interface;
using DatabaseLayer;
using Microsoft.AspNetCore.Mvc;
using System;

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
    }
}
