using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MonsterCardTradingGame.Exceptions;

namespace MonsterCardTradingGame.BL.Services
{
    class UserService
    {
        static DAL.Repository.IUserRepository userrepos = new DAL.Respository.UserRepository();

        /*
         *  Register a new User
         */
        public static bool Register(Utility.Json.CredentialsJson cred)
        {
            // Validate username
            // 
            // Hash the Password
            cred.Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                cred.Password,
                Encoding.UTF8.GetBytes("js83$0jolsod/"),
                KeyDerivationPrf.HMACSHA512,
                10,
                64)
            );

            // Add to Player Table
            try
            {
                userrepos.Create(new Model.Credentials(cred.Username, cred.Password));
                return true;
            }
            catch(RepositoryException e) when (e.Message == "User already exists")
            {
                throw new HttpException("409 Conflict");
            }
            catch (System.Exception) {
                throw new HttpException("500 Interal Server Error");
            }           
        }

        /*
         *  Login User, returns Token if login successful, if not returns empty string
         */
        public static string Login(Utility.Json.CredentialsJson cred)
        {
            string passwordFromDB;
            try
            {
                Model.User user = userrepos.GetByName(cred.Username);
                passwordFromDB = user.Password;
            }
            catch(RepositoryException e) when(e.Message == "User does not exist")
            {
                throw new HttpException("404 Not Found"); 
            }catch
            {
                throw new HttpException("500 Internal Server Error");
            }

            // Hash the pw the user entered:
            cred.Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                cred.Password,
                Encoding.UTF8.GetBytes("js83$0jolsod/"), // Salt Value
                KeyDerivationPrf.HMACSHA512,
                10,
                64));

            // Check if pw match up:
            if (passwordFromDB == cred.Password)
            {
                return generateToken(cred.Username);
            }
            else
            {
                throw new HttpException("401 Unauthorized");
            }
        }

        public static Model.User GetUserByUsername(string username)
        {
            try
            {
                return userrepos.GetByName(username);
            }
            catch
            {
                throw;
            }
        }


        /*
         *  Helper
         */
        private static string generateToken(string username)
        {
            string token = $"Basic {username}-mctgToken";
            return token;
        }
    }
}
