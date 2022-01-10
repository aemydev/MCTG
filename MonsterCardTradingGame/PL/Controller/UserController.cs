using MonsterCardTradingGame.BL.Services;
using MonsterCardTradingGame.Exceptions;
using MonsterCardTradingGame.Model;
using MonsterCardTradingGame.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace MonsterCardTradingGame.PL.Controller
{
    public static class UserController
    {
        private static UserService UserService = new UserService();

        #region userbasics
        /*
         *  /register, POST
         */
        public static HttpResponse Register(HttpRequest req)
        {
            HttpResponse res;
            Utility.Json.CredentialsJson cred;
            
            // String -> Json
            try
            {
                cred = JsonConvert.DeserializeObject<Utility.Json.CredentialsJson>(req.Content);
            }
            catch
            {
                res = new HttpResponse(HttpStatusCode.BadRequest);
                res.AddContent("application/json", "{\"message\":\"Could not parse content\"}");
                return res;
            }

            Console.WriteLine($"[{DateTime.UtcNow}]\tRegister new user \"{cred.Username}\"");

            try
            {
                UserService.Register(cred);

            }
            catch (ServiceException e) when (e.Message == "user already exists")
            {
                res = new HttpResponse(HttpStatusCode.Conflict);
                res.AddContent("application/json", "{\"response\":\"User already exists.\"}");
                return res;

            }
            catch (ServiceException e) when (e.Message == "db error")
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
        public static HttpResponse Login(HttpRequest req)
        {
            HttpResponse res;
            string token;
            Utility.Json.CredentialsJson cred;

            try
            {
                cred = JsonConvert.DeserializeObject<Utility.Json.CredentialsJson>(req.Content);
            }
            catch
            {
                res = new HttpResponse(HttpStatusCode.BadRequest);
                res.AddContent("application/json", "{\"message\":\"Could not parse content\"}");
                return res;
            }

            Console.WriteLine($"[{DateTime.UtcNow}]\t Login \"{cred.Username}\"");

            try
            {
                token = UserService.Login(cred);
            }
            catch (ServiceException e) when (e.Message == "not found")
            {
                res = new HttpResponse(HttpStatusCode.NotFound);
                res.AddContent("application/json", "{\"message\":\"User not found.\"}");
                return res;
            }
            catch (ServiceException e) when (e.Message == "db error")
            {
                res = new HttpResponse(HttpStatusCode.InternalServerError);
                res.AddContent("application/json", "{\"message\":\"Something went wrong.\"}");
                return res;
            }
            catch (ServiceException e) when (e.Message == "login failed")
            {
                res = new HttpResponse(HttpStatusCode.Unauthorized);
                res.AddContent("application/json", "{\"message\":\"Password incorrect.\"}");
                return res;
            }

            // Login successfull
            res = new HttpResponse(HttpStatusCode.OK);
            res.addHeader("Authorization", token);
            res.AddContent("application/json", "{\"message\":\"Login successful.\"}");
            return res;
        }

        #endregion
        #region deck

        /*
        *  /deck, GET
        */
        public static HttpResponse ShowActiveDeck(HttpRequest req)
        {
            HttpResponse res;

            // Valid token?
            if (!AuthService.AuthToken(req.Headers, out string username, out Guid userid))
            {
                // no -> send error response
                res = new HttpResponse(HttpStatusCode.Forbidden);
                res.AddContent("application/json", "{\"message\":\"Access denied. Please Login.\"}");
                return res;
            }

            Console.WriteLine($"[{DateTime.UtcNow}]\tShow active deck \"username\"");

            // Get user
            if (!UserService.GetUserByUsername(username, out User user))
            {
                res = new HttpResponse(HttpStatusCode.NotFound);
                res.AddContent("application/json", "{\"message\":\"User not found\"}");
                return res;
            }

            // if active_deck == null
            if (user.ActiveDeckId == null)
            {
                // no active deck -> send response
                res = new HttpResponse(HttpStatusCode.OK);
                res.AddContent("application/json", "{\"message\":\"No active deck set.\"}");
                return res;
            }

            // otherwise get deck with deck_id and send it inkl titel etc.
            if (!UserService.GetActiveDeck((Guid)user.ActiveDeckId, out Deck deck))
            {
                res = new HttpResponse(HttpStatusCode.InternalServerError);
                res.AddContent("application/json", "{\"response\":\"Something went wrong\"}");
                return res;
            }

            // Send active deck
            string jsonString = JsonConvert.SerializeObject(deck);
            res = new HttpResponse(HttpStatusCode.OK);
            res.AddContent("application/json", $"{{\"response\":\"Returning active deck.\", \"content\":{jsonString}}}");
            return res;
        }

        /*
        *  /deck/all, GET
        */
        public static HttpResponse ShowAllDecks(HttpRequest req)
        {
            HttpResponse res;

            // Valid token?
            if (!BL.Services.AuthService.AuthToken(req.Headers, out string username, out Guid userid))
            {
                // no -> send error response
                res = new HttpResponse(HttpStatusCode.Forbidden);
                res.AddContent("application/json", "{\"message\":\"Access denied. Please Login.\"}");
                return res;
            }

            Console.WriteLine($"[{DateTime.Now}]\t Show all cards from \"{username}\"");

            // Get all decks where owner == user_id
            List<Deck> decks;
            try
            {
                decks = UserService.GetAllDecks(userid);
            }
            catch
            {
                // sth went wrong
                // no decks found
                res = new HttpResponse(HttpStatusCode.InternalServerError);
                res.AddContent("application/json", "{\"response\":\"Something went wrong\"}");
                return res;
            }

            // Send decks
            string jsonString = JsonConvert.SerializeObject(decks);
            res = new HttpResponse(HttpStatusCode.OK);
            res.AddContent("application/json", $"{{\"response\":\"Content: all decks owned by {username}\", \"content\":{jsonString}}}");
            return res;
        }

        /*
         *  /deck/new, POST
         */
        public static HttpResponse AddNewDeck(HttpRequest req)
        {
            HttpResponse res;
            
            // Valid token?
            if (!AuthService.AuthToken(req.Headers, out string username, out Guid userid))
            {
                // no -> send error response
                res = new HttpResponse(HttpStatusCode.Forbidden);
                res.AddContent("application/json", "{\"message\":\"Access denied. Please Login.\"}");
                return res;
            }

            Console.WriteLine($"[{DateTime.UtcNow}]\t Add new Deck for \"{username}\"");
            
            if (!UserService.GetUserByUsername(username, out User user))
            {
                res = new HttpResponse(HttpStatusCode.InternalServerError);
                res.AddContent("application/json", "{\"message\":\"User not found\"}");
                return res;
            }

            Utility.Json.DeckJson deckJson;
            try
            {
                deckJson = JsonConvert.DeserializeObject<Utility.Json.DeckJson>(req.Content);
            }
            catch
            {
                res = new HttpResponse(HttpStatusCode.InternalServerError);
                res.AddContent("application/json", "{\"message\":\"Could not parse content\"}");
                return res;
            }

            try
            {
                UserService.AddNewDeck(deckJson, user.UserId);
            }
            catch
            {
                res = new HttpResponse(HttpStatusCode.InternalServerError);
                res.AddContent("application/json", "{\"response\":\"Something went wrong.\"}");
                return res;
            }

            res = new HttpResponse(HttpStatusCode.OK);
            res.AddContent("application/json", "{\"response\":\"New deck created successfully\"}");
            return res;
        }

        /*
         *  /deck, PUT
         */
        public static HttpResponse SetActiveDeck(HttpRequest req)
        {
            HttpResponse res;

            // Valid token?
            if (!BL.Services.AuthService.AuthToken(req.Headers, out string username, out Guid userid))
            {
                // no -> send error response
                res = new HttpResponse(HttpStatusCode.Forbidden);
                res.AddContent("application/json", "{\"message\":\"Access denied. Please Login.\"}");
                return res;
            }

            Utility.Json.DeckJson deckJson;
            try
            {
                deckJson = JsonConvert.DeserializeObject<Utility.Json.DeckJson>(req.Content);
            }
            catch
            {
                res = new HttpResponse(HttpStatusCode.BadRequest);
                res.AddContent("application/json", "{\"message\":\"Could not parse content\"}");
                return res;
            }

            // Add Deck
            try
            {
                UserService.SetActiveDeck(userid, Guid.Parse(deckJson.Id));
            }
            catch
            {
                res = new HttpResponse(HttpStatusCode.InternalServerError);
                res.AddContent("application/json", "{\"message\":\"Something went wrong\"}");
                return res;
            }

            res = new HttpResponse(HttpStatusCode.OK);
            res.AddContent("application/json", "{\"message\":\"Active Deck has been set.\"}");
            return res;
        }

        #endregion
    }
}
