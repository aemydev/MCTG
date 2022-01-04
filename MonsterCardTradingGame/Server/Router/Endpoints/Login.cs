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
    class Login : IEndpoint
    {
        public HttpResponse call(string content = null)
        {
            // String -> Json (Credentials)
            var cred = JsonConvert.DeserializeObject<Utility.Json.CredentialsJson>(content);
            
            try
            {
                if (Controller.UserController.Login(cred))
                {
                    // return response with Token set
                    HttpResponse res;
                    res = new HttpResponse(HttpStatusCode.OK);
                    res.addHeader("Authorization", "valid");
                    res.AddContent("application/json", "{\"response\":\"Login successful.\"}");
                    return res;
                }
                else
                {
                    // Response ohne Token -> Error
                    HttpResponse res;
                    res = new HttpResponse(HttpStatusCode.Forbidden);
                    res.AddContent("application/json", "{\"response\":\"Login failed. Please try again.\"}");
                    return res;
                }
            }
            catch (System.Exception e)
            {
                HttpResponse res;
                res = new HttpResponse(HttpStatusCode.Forbidden);
                string errorMsg = "{\"response\":\"" + $"{ e.Message}" + "\"}";
                res.AddContent("application/json", errorMsg);
                return res;
            }
        }
    }
}
