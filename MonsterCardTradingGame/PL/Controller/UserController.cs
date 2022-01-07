using MonsterCardTradingGame.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MonsterCardTradingGame.Exceptions;

namespace MonsterCardTradingGame.PL.Controller
{
    public static class UserController
    {
        /*
         *  /register, POST
         */
        public static HttpResponse Register(Server.HttpRequest req)
        {
            HttpResponse res;

            // String -> Json
            var cred = JsonConvert.DeserializeObject<Utility.Json.CredentialsJson>(req.Content);

            try
            {
                BL.Services.UserService.Register(cred);
            }catch(HttpException e) when (e.Message == "409 Conflict")
            {
                res = new HttpResponse(HttpStatusCode.Conflict);
                res.AddContent("application/json", "{\"response\":\"User already exists.\"}");
                return res;
            }catch(HttpException e) when(e.Message == "500 Interal Server Error")
            {
                res = new HttpResponse(HttpStatusCode.InternalServerError);
                res.AddContent("application/json", "{\"response\":\"Internal Server Error.\"}");
                return res;
            }

            res = new HttpResponse(HttpStatusCode.OK);
            res.AddContent("application/json", "{\"response\":\"User successfully created.\"}");
            return res;
        }

        /*
        *  /login, POST
        */
        public static HttpResponse Login(Server.HttpRequest req)
        {
            HttpResponse res;

            var cred = JsonConvert.DeserializeObject<Utility.Json.CredentialsJson>(req.Content);
            string token;
            try
            {
                token = BL.Services.UserService.Login(cred);
            }
            catch (HttpException e) when (e.Message == "404 Not Found")
            {
                res = new HttpResponse(HttpStatusCode.NotFound);
                res.AddContent("application/json", "{\"message\":\"User not found.\"}");
                return res;
            }catch(HttpException e) when(e.Message =="500 Internal Server Error")
            {
                res = new HttpResponse(HttpStatusCode.InternalServerError);
                res.AddContent("application/json", "{\"message\":\"Something went wrong.\"}");
                return res;
            }catch(HttpException e) when (e.Message == "401 Unauthorized")
            {
                res = new HttpResponse(HttpStatusCode.Unauthorized);
                res.AddContent("application/json", "{\"message\":\"Password incorrect.\"}");
                return res;
            }

            // Login successfull
            res = new HttpResponse(HttpStatusCode.OK);
            res.addHeader("Authorization", token);
            res.AddContent("application/json", "{\"response\":\"Login successful.\"}");
            return res;
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
