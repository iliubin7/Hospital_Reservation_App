﻿using Hospital_Reservation_App.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Hospital_Reservation_App.Repositories
{
    public class UserRepository : DataBaseRepository, IUserRepository
    {
        public void Add(UserModel userModel)
        {
            using (var connection = GetConnection())
            {
                using (var command = new MySqlCommand())
                {
                    string passHash = BCrypt.Net.BCrypt.HashPassword(SecureStringToString(userModel.Password));
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = "INSERT INTO users(PESEL, firstname, lastname, sex, email, password) VALUES (@pesel, @fname, @lname, @sx, @mail, @pass)";
                    command.Parameters.Add("@pesel", MySqlDbType.VarChar).Value = SecureStringToString(userModel.PESEL);
                    command.Parameters.Add("@fname", MySqlDbType.VarChar).Value = userModel.firstName;
                    command.Parameters.Add("@lname", MySqlDbType.VarChar).Value = userModel.lastName;
                    command.Parameters.Add("@sx", MySqlDbType.VarChar).Value = userModel.sex;
                    command.Parameters.Add("@mail", MySqlDbType.VarChar).Value = userModel.email;
                    command.Parameters.Add("@pass", MySqlDbType.VarChar).Value = passHash;
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool AuthentificateUser(NetworkCredential credential)
        {
            bool validUser;
            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {
                string passUserDB = "";
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT * FROM users WHERE email = @mail";
                command.Parameters.Add("@mail", MySqlDbType.VarChar).Value = credential.UserName;
                MySqlDataReader data = command.ExecuteReader();
                if (data.HasRows)
                {
                    while (data.Read())
                    {
                        passUserDB = Convert.ToString(data.GetValue(6));
                    }
                }
                if (BCrypt.Net.BCrypt.Verify(Convert.ToString(credential.Password), passUserDB))
                {
                    validUser = true;
                }
                else
                {
                    validUser = false;
                }

            }
            return validUser;
        }

        public String SecureStringToString(SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

        public bool checkMail(string email)
        {
            bool validMail;
            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT email FROM users WHERE email = @mail";
                command.Parameters.Add("@mail", MySqlDbType.VarChar).Value = email;
                validMail = command.ExecuteScalar() == null ? false : true;
            }
            return validMail;
        }

        public bool checkPeselLength(SecureString pesel)
        {
            bool validPeselLength;
            if (SecureStringToString(pesel).Length != 11)
                validPeselLength = false;
            else
                validPeselLength = true;
            return validPeselLength;
        }

        public bool checkPeselUser(SecureString pesel)
        {
            bool validPeselUser;
            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT PESEL FROM users WHERE PESEL = @pesel";
                command.Parameters.Add("@pesel", MySqlDbType.VarChar).Value = SecureStringToString(pesel);
                validPeselUser = command.ExecuteScalar() == null ? false : true;
            }
            return validPeselUser;
        }

        public bool checkPassRepeat(SecureString password, SecureString passwordRep)
        {
            if (SecureStringToString(password) == SecureStringToString(passwordRep))
                return true;
            else
                return false;
        }
    }
}
