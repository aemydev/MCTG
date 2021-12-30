using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Router.Endpoints
{
    class Register : IEndpoint
    {
        public bool call(string content = null)
        {
            // String -> Json:
            var cred = JsonConvert.DeserializeObject<Utility.Json.CredentialsJson>(content);

            if (Controller.UserController.Register(cred))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
