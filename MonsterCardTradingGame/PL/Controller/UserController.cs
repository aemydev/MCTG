using MonsterCardTradingGame.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.PL.Controller
{
    public static class UserController
    {
        /*
         *  /register, POST
         */
        public static HttpResponse Register(Server.HttpRequest req)
        {
            Console.WriteLine($"[{DateTime.Now}]: Register new User");

            // String -> Json:
            var cred = JsonConvert.DeserializeObject<Utility.Json.CredentialsJson>(req.Content);

            if (BL.Services.UserService.Register(cred))
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

        /*
        *  /login, POST
        */
        public static HttpResponse Login(Server.HttpRequest req)
        {
            HttpResponse res;
            // String -> Json (Credentials)
            var cred = JsonConvert.DeserializeObject<Utility.Json.CredentialsJson>(req.Content);

            try
            {
                string token = BL.Services.UserService.Login(cred);

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


        /*
        *  /deck, GET
        */
        public static HttpResponse ShowDeck(Server.HttpRequest req)
        {
            throw new NotImplementedException();
        }

        /*
         *  /deck, PUT
         */
        public static HttpResponse ChangeDeck(Server.HttpRequest req)
        {
            throw new NotImplementedException();
        }

        /*
         *  /users/username, GET
         */
        public static HttpResponse ShowProfile(Server.HttpRequest req)
        {
            throw new NotImplementedException();
        }

        /*
         *  /users/username, PUT
         */
        public static HttpResponse ChangeProfile(Server.HttpRequest req)
        {
            throw new NotImplementedException();
        }
    }
}
