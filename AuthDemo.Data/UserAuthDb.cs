using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthDemo.Data
{
    public class UserAuthDb
    {
        private string _connectionString;

        public UserAuthDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddUser(User user, string password)
        {
            string salt = PasswordHelper.GenerateSalt();
            string passwordHash = PasswordHelper.HashPassword(password, salt);
            user.PasswordSalt = salt;
            user.PasswordHash = passwordHash;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Users (FirstName, LastName, Email, PasswordHash, Salt)" +
                                      " VALUES (@firstName, @lastName, @email, @hash, @salt)";
                command.Parameters.AddWithValue("@firstName", user.FirstName);
                command.Parameters.AddWithValue("@lastName", user.LastName);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@hash", user.PasswordHash);
                command.Parameters.AddWithValue("@salt", user.PasswordSalt);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public User Login(string email, string password)
        {
            User user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }
            bool isCorrectPassword = PasswordHelper.PasswordMatch(password, user.PasswordSalt, user.PasswordHash);
            if (!isCorrectPassword)
            {
                return null;
            }

            return user;
        }

        public User GetByEmail(string email)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Users WHERE Email = @email";
                command.Parameters.AddWithValue("@email", email);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }

                return GetUserFromReader(reader);
            }
        }

        private User GetUserFromReader(SqlDataReader reader)
        {
            User user = new User
            {
                Id = (int)reader["Id"],
                FirstName = (string)reader["FirstName"],
                LastName = (string)reader["LastName"],
                Email = (string)reader["Email"],
                PasswordHash = (string)reader["PasswordHash"],
                PasswordSalt = (string)reader["Salt"]
            };
            return user;
        }
    }
}
