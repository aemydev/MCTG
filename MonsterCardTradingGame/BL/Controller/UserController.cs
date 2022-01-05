using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MonsterCardTradingGame.BL.Controller
{
    class UserController
    {
        static DAL.Repository.IUserRepository userrepos = new DAL.Respository.UserRepository();

        /*
         *  Register a new User
         */
        public static bool Register(Utility.Json.CredentialsJson cred)
        {
            // Hash the password
            cred.Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                cred.Password,
                Encoding.UTF8.GetBytes("js83$0jolsod/"),
                KeyDerivationPrf.HMACSHA512,
                10,
                64)
            );

            // Add to User Table
            try
            {
                userrepos.Create(new Model.User(cred.Username, cred.Password));
                return true;
            } catch (System.Exception e)
            {
                Console.WriteLine($"[{ DateTime.UtcNow}] - {e.Message}");
                return false; // Registration failed
            }
            
            // Add Deck?

        }

        /*
         *  Login User, returns Token if login successful, if not returns empty string
         */
        public static string Login(Utility.Json.CredentialsJson cred)
        {
            string passwordFromDB;
            try
            {
                passwordFromDB = userrepos.GetPwByUsername(cred.Username);
            }
            catch
            {
                return ""; // sth went wrong with the DB, user might not exist
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
                return ""; // Incorrect Password -> no token returned
            }
        }






        private static string generateToken(string username)
        {
            string token = $"Basic {username}-mctgToken";
            return token;
        }
    }
}
