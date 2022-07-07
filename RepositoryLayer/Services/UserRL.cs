using DatabaseLayer;
using Microsoft.Extensions.Configuration;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace RepositoryLayer.Services
{
    public class UserRL : IUserRL
    {
        private readonly string connectionString;
        public UserRL(IConfiguration configuartion)
        {
            connectionString = configuartion.GetConnectionString("Fundoonotes");
        }
        public void AddUser(UserModel userModel)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                using (connection)
                {
                    connection.Open();
                    //Creating a stored Procedure for adding Users into database
                    SqlCommand com = new SqlCommand("spAddUser", connection);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@FirstName", userModel.FirstName);
                    com.Parameters.AddWithValue("@LastName", userModel.LastName);
                    com.Parameters.AddWithValue("@Email", userModel.Email);
                    com.Parameters.AddWithValue("@Password", userModel.Password);
                    var result = com.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<UserResponseModel> GetAllUsers()
        {

            List<UserResponseModel> users = new List<UserResponseModel>();
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                using (connection)
                {
                    connection.Open();
                    SqlCommand com = new SqlCommand("spGetAllUser", connection);
                    com.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = com.ExecuteReader();
                    while (reader.Read())
                    {
                        UserResponseModel user = new UserResponseModel();
                        user.UserID = reader["UserID"] == DBNull.Value ? default : reader.GetInt32("UserID");
                        user.FirstName = reader["FirstName"] == DBNull.Value ? default : reader.GetString("FirstName");
                        user.LastName = reader["LastName"] == DBNull.Value ? default : reader.GetString("LastName");
                        user.Email = reader["Email"] == DBNull.Value ? default : reader.GetString("Email");
                        user.Password = reader["Password"] == DBNull.Value ? default : reader.GetString("Password");
                        user.CreatedDate = reader["CreatedDate"] == DBNull.Value ? default : reader.GetDateTime("CreatedDate");
                        user.ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? default : reader.GetDateTime("ModifiedDate");
                        users.Add(user);
                    }
                    return users;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
