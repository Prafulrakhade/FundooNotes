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
        public string LoginUser(LoginUserModel loginUser)
        {
            try
            {
                return this.userRL.LoginUser(loginUser);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool ForgetPasswordUser(string email)
        {
            try
            {
                return this.userRL.ForgetPasswordUser(email);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ResetPassoword(string email, PasswordModel modelPassword)
        {
           try
            {
                return this.userRL.ResetPassoword(email, modelPassword);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
