using MonsterCardTradingGame.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Router.Endpoints
{
    class Register : IEndpoint
    {
        public HttpResponse call(string content = null)
        {
            // String -> Json:
            var cred = JsonConvert.DeserializeObject<Utility.Json.CredentialsJson>(content);

            if (Controller.UserController.Register(cred))
            {
                HttpResponse res;
                res = new HttpResponse(HttpStatusCode.OK);
                res.AddContent("application/json", "{\"response\":\"User successfully registerd\"}");
                return res;
            }
            else
            {
                HttpResponse res;
                res = new HttpResponse(HttpStatusCode.Conflict);
                res.AddContent("application/json", "{\"response\":\"User registration failed. Please try again.\"}");
                return res;
            }
        }
    }
}
