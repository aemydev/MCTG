using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MonsterCardTradingGame.Controller
{
    class UserController
    {
        static DB.Repository.IUserRepository userrepos = new DB.Respository.UserRepository();

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

            try
            {
                userrepos.Create(new Model.User(cred.Username, cred.Password));
                return true;
            }catch(System.Exception e)
            {
                Console.WriteLine($"[{ DateTime.UtcNow}] - {e.Message}");
                return false; // Registration failed
            }    
        }

        /*
         *  Login User
         */
        public static bool Login(Utility.Json.CredentialsJson cred)
        {
            string passwordFromDB;
            try
            {
                passwordFromDB = userrepos.GetPwByUsername(cred.Username);
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"[{ DateTime.UtcNow}] - {e.Message}");
                throw; // sth went wrong with the DB, user might not exist
            }

            // Hash the pw the user entered:
            cred.Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                cred.Password,
                Encoding.UTF8.GetBytes("js83$0jolsod/"),
                KeyDerivationPrf.HMACSHA512,
                10,
                64));

            // Check if pw match up:
            if (passwordFromDB == cred.Password)
            {
                // yes -> return true
                return true;
            }
            else
            {
                // no -> return false, incorrect pw
                return false;
            }
        }



    }
}
