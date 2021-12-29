using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MonsterCardTradingGame.Controller
{
    class UserController
    {
        static DB.Repository.IUserRepository userrepos = new DB.Respository.UserRepository();

        /*
         *  Register a new User
         */
        public static bool Register(Json.Credentials cred)
        {
            try
            {
                userrepos.Create(new Model.User(cred.Username, cred.Password));
                return true;
            }catch
            {
                return false;
            }    
        }
    }
}
