using DatabaseLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Interface
{
    public interface IUserBL
    {
        public void AddUser(UserModel userModel);
        public List<UserResponseModel> GetAllUsers();
        public string LoginUser(LoginUserModel loginUser);
        public bool ForgetPasswordUser(string email);
    }
}
