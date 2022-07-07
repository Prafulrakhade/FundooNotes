using BusinessLayer.Interface;
using DatabaseLayer;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Sevices
{
    public class UserBL : IUserBL
    {
         IUserRL userRL;
        public UserBL(IUserRL userRL)
        {
            this.userRL = userRL;
        }
        public void AddUser(UserModel userModel)
        {
            try
            {
                this.userRL.AddUser(userModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<UserResponseModel> GetAllUsers()
        {
            return this.userRL.GetAllUsers();

        }
    }
}
