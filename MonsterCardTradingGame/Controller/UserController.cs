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
        public static bool Register(Model.Credentials cred)
        {
            if (userrepos.Create(new Model.User(cred.Username, cred.Password)) == true){
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
