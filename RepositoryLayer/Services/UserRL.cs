using DatabaseLayer;
using Experimental.System.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
        public string LoginUser(LoginUserModel loginUser)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                using (connection)
                {
                    connection.Open();
                    SqlCommand com = new SqlCommand("spLoginUser", connection);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@Email", loginUser.Email);
                    com.Parameters.AddWithValue("@password", loginUser.Password);
                    var result = com.ExecuteNonQuery();
                    SqlDataReader rd = com.ExecuteReader();
                    UserResponseModel response = new UserResponseModel();
                    if (rd.Read())
                    {
                        response.UserID = rd["UserId"] == DBNull.Value ? default : rd.GetInt32("UserId");
                        response.Email = rd["Email"] == DBNull.Value ? default : rd.GetString("Email");
                        response.Password = rd["password"] == DBNull.Value ? default : rd.GetString("password");

                    }
                    return GenerateJWTToken(response.Email, response.UserID);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string GenerateJWTToken(string email, int userId)
        {
            try
            {
                // generate token
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.ASCII.GetBytes("THIS_IS_MY_KEY_TO_GENERATE_TOKEN");
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim("email", email),
                    new Claim("userId",userId.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddHours(2),

                    SigningCredentials =
                new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool ForgetPasswordUser(string email)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                using (connection)
                {
                    connection.Open();
                    SqlCommand com = new SqlCommand("spForgetPasswordUser", connection);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@Email", email);
                    var result = com.ExecuteNonQuery();
                    SqlDataReader rd = com.ExecuteReader();
                    UserResponseModel response = new UserResponseModel();
                    if (rd.Read())
                    {
                        response.UserID = rd["UserId"] == DBNull.Value ? default : rd.GetInt32("UserId");
                        response.Email = rd["Email"] == DBNull.Value ? default : rd.GetString("Email");
                    }
                    MessageQueue messageQueue;
                    //ADD MESSAGE TO QUEUE
                    if (MessageQueue.Exists(@".\Private$\FundooQueue"))
                    {
                        messageQueue = new MessageQueue(@".\Private$\FundooQueue");
                    }
                    else
                    {
                        messageQueue = MessageQueue.Create(@".\Private$\FundooQueue");
                    }
                    Message MyMessage = new Message();
                    MyMessage.Formatter = new BinaryMessageFormatter();
                    MyMessage.Body = GenerateJWTToken(email, response.UserID);
                    MyMessage.Label = "Forget Password Email";
                    messageQueue.Send(MyMessage);
                    Message msg = messageQueue.Receive();
                    msg.Formatter = new BinaryMessageFormatter();
                    EmailService.SendEmail(email, msg.Body.ToString());
                    messageQueue.ReceiveCompleted += new ReceiveCompletedEventHandler(msmqQueue_ReceiveCompleted);

                    messageQueue.BeginReceive();
                    messageQueue.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void msmqQueue_ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            try
            {
                MessageQueue queue = (MessageQueue)sender;
                Message msg = queue.EndReceive(e.AsyncResult);
                EmailService.SendEmail(e.Message.ToString(), GenerateToken(e.Message.ToString()));
                queue.BeginReceive();
            }
            catch (MessageQueueException ex)
            {
                if (ex.MessageQueueErrorCode ==
                    MessageQueueErrorCode.AccessDenied)
                {
                    Console.WriteLine("Access is denied. " +
                        "Queue might be a system queue.");
                }
            }
        }
        private string GenerateToken(string email)
        {
            try
            {
                // generate token
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.ASCII.GetBytes("THIS_IS_MY_KEY_TO_GENERATE_TOKEN");
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim("email", email)

                    }),
                    Expires = DateTime.UtcNow.AddHours(2),

                    SigningCredentials =
                new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
