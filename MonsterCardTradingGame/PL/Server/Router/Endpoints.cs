using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Server.Router
{
    public static class Endpoints
    {
        public static HttpResponse Register(Server.HttpRequest req)
        {
            Console.WriteLine($"[{DateTime.Now}]: Register new User");
            
            // String -> Json:
            var cred = JsonConvert.DeserializeObject<Utility.Json.CredentialsJson>(req.Content);

            if (BL.Controller.UserController.Register(cred))
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

        public static HttpResponse Login(Server.HttpRequest req)
        {
            HttpResponse res;
            // String -> Json (Credentials)
            var cred = JsonConvert.DeserializeObject<Utility.Json.CredentialsJson>(req.Content);

            try
            {
                string token = BL.Controller.UserController.Login(cred);

                if (token == "")
                {
                    Console.WriteLine($"[{DateTime.UtcNow}]: Login unsuccessful for {cred.Username}");
                    // return response with Token set
                    res = new HttpResponse(HttpStatusCode.Forbidden);
                    res.AddContent("application/json", "{\"response\":\"Login failed. Please try again.\"}");
                    return res;
                }

                res = new HttpResponse(HttpStatusCode.OK);
                res.addHeader("Authorization", token);
                res.AddContent("application/json", "{\"response\":\"Login successful.\"}");
                return res;
            }
            catch (System.Exception e)
            {
                res = new HttpResponse(HttpStatusCode.Forbidden);
                string errorMsg = "{\"response\":\"" + $"{ e.Message}" + "\"}";
                res.AddContent("application/json", errorMsg);
                return res;
            }
        }

        public static HttpResponse Packages(Server.HttpRequest req)
        {
            HttpResponse res;

            if (!Utility.Helper.AuthAdmin(req))
            {
                res = new HttpResponse(HttpStatusCode.Forbidden);
                res.AddContent("application/json", "{\"error\":\"Access denied. Only admins can create packages.\"}");
                return res;
            }

            // Save Package bzw. Cards in Package to DB
            // Card, Package ID/Package Name
            var package = JsonConvert.DeserializeObject<List<Utility.Json.CardJson>>(req.Content);

            foreach(Utility.Json.CardJson card_ in package)
            {
                Model.Card cardTemp = new Model.Card(card_.Id, card_.Name, card_.Damage);
            }


            // if everything worked -> return success message
            res = new HttpResponse(HttpStatusCode.Created);
            res.AddContent("application/json", "{\"message\":\"Package successfully created\"}");
            return res;
        }       
    }
}
