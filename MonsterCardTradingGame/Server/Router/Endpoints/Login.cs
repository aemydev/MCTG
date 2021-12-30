using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Router.Endpoints
{
    class Login : IEndpoint
    {
        public bool call(string content = null)
        {
            // String -> Json (Credentials)
            var cred = JsonConvert.DeserializeObject<Utility.Json.CredentialsJson>(content);

            if (Controller.UserController.Login(cred))
            {
                return true; // Res direkt senden?
            }
            else
            {
                return false;
            }
        }
    }
}
