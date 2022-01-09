using MonsterCardTradingGame.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MonsterCardTradingGame.Exceptions;
using MonsterCardTradingGame.Model;

namespace MonsterCardTradingGame.PL.Controller
{
    public static class UserController
    {
        #region userbasics
        /*
         *  /register, POST
         */
        public static HttpResponse Register(HttpRequest req)
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
        public static HttpResponse Login(HttpRequest req)
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

        #endregion
        #region deck

        /*
        *  /deck, GET
        */
        public static HttpResponse ShowActiveDeck(HttpRequest req)
        {
            HttpResponse res;

            // Valid token?
            if (!BL.Services.AuthService.Auth(req))
            {
                // no -> send error response
                res = new HttpResponse(HttpStatusCode.Forbidden);
                res.AddContent("application/json", "{\"message\":\"Access denied. Please Login.\"}");
                return res;
            }

            string username = BL.Services.AuthService.getUserNameFromToken(req.Headers["Authorization"]);
            Model.User user;
            
            Console.WriteLine($"[{DateTime.UtcNow}]\tShow active deck \"username\"");

            // Get user
            try
            {
                user = BL.Services.UserService.GetUserByUsername(username);
            }
            catch(HttpException e) when (e.Message == "404 not found")
            {
                res = new HttpResponse(HttpStatusCode.InternalServerError);
                res.AddContent("application/json", "{\"message\":\"User not found\"}");
                return res;
            }
            catch
            {
                res = new HttpResponse(HttpStatusCode.InternalServerError);
                res.AddContent("application/json", "{\"message\":\"Something went wrong\"}");
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
            Deck deck;
            try
            {
                Console.WriteLine(user.ActiveDeckId);
                deck = BL.Services.UserService.GetActiveDeck((Guid)user.ActiveDeckId);
            }
            catch
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
            if (!BL.Services.AuthService.Auth(req))
            {
                // no -> send error response
                res = new HttpResponse(HttpStatusCode.Forbidden);
                res.AddContent("application/json", "{\"message\":\"Access denied. Please Login.\"}");
                return res;
            }

            string token = req.Headers["Authorization"]; // no if required, we already checked that token is valid
            string username = BL.Services.AuthService.getUserNameFromToken(token);
            Guid userid;
            // Get user
            try
            {
                userid = BL.Services.UserService.GetIdByUsername(username);
            }
            catch (HttpException e) when (e.Message == "404 not found")
            {
                res = new HttpResponse(HttpStatusCode.InternalServerError);
                res.AddContent("application/json", "{\"message\":\"User not found\"}");
                return res;
            }
            catch
            {
                res = new HttpResponse(HttpStatusCode.InternalServerError);
                res.AddContent("application/json", "{\"message\":\"Something went wrong\"}");
                return res;
            }

            // Get all decks where owner == user_id
            List<Deck> decks;
            try
            {
                decks = BL.Services.UserService.GetAllDecks(userid);
            }
            catch
            {
                // sth went wrong
                // no decks found
                res = new HttpResponse(HttpStatusCode.InternalServerError);
                res.AddContent("application/json", "{\"response\":\"Sth went wrong\"}");
                return res;
            }

            // Send decks
            string jsonString = JsonConvert.SerializeObject(decks);
            res = new HttpResponse(HttpStatusCode.OK);
            res.AddContent("application/json", $"{{\"response\":\"Returning as Content: all decks owned by {username}\", \"content\":{jsonString}}}");
            return res;
        }

        /*
         *  /deck/new, POST
         */
        public static HttpResponse AddNewDeck(HttpRequest req)
        {
            HttpResponse res;

            // Valid token?
            if (!BL.Services.AuthService.Auth(req))
            {
                // no -> send error response
                res = new HttpResponse(HttpStatusCode.Forbidden);
                res.AddContent("application/json", "{\"message\":\"Access denied. Please Login.\"}");
                return res;
            }

            string token = req.Headers["Authorization"]; // no if required, we already checked that token is valid
            string username = BL.Services.AuthService.getUserNameFromToken(token);
            Model.User user;

            // Get user
            try
            {
                user = BL.Services.UserService.GetUserByUsername(username);
            }
            catch (HttpException e) when (e.Message == "404 not found")
            {
                res = new HttpResponse(HttpStatusCode.InternalServerError);
                res.AddContent("application/json", "{\"message\":\"User not found\"}");
                return res;
            }
            catch
            {
                res = new HttpResponse(HttpStatusCode.InternalServerError);
                res.AddContent("application/json", "{\"message\":\"Something went wrong\"}");
                return res;
            }

            var deckJson = JsonConvert.DeserializeObject<Utility.Json.DeckJson>(req.Content);
            try
            {
                BL.Services.UserService.AddNewDeck(deckJson,user.UserId);
            }
            catch
            {
                res = new HttpResponse(HttpStatusCode.InternalServerError);
                res.AddContent("application/json", "{\"response\":\"Create new Deck\"}");
                return res;
            }

            res = new HttpResponse(HttpStatusCode.OK);
            res.AddContent("application/json", "{\"response\":\"New deck created successfully\"}");
            return res;
        }

        /*
         *  /deck, PUT
         */
        public static HttpResponse SetActiveDeck(Server.HttpRequest req)
        {
            HttpResponse res;

            // Valid token?
            if (!BL.Services.AuthService.Auth(req))
            {
                // no -> send error response
                res = new HttpResponse(HttpStatusCode.Forbidden);
                res.AddContent("application/json", "{\"message\":\"Access denied. Please Login.\"}");
                return res;
            }

            string token = req.Headers["Authorization"];
            string username = BL.Services.AuthService.getUserNameFromToken(token);
            Guid userid;
            // Get user, if active_deck == null
            try
            {
                userid = BL.Services.UserService.GetIdByUsername(username);
            }
            catch
            {
                res = new HttpResponse(HttpStatusCode.InternalServerError);
                res.AddContent("application/json", "{\"message\":\"Something went wrong\"}");
                return res;
            }


            var deckJson = JsonConvert.DeserializeObject<Utility.Json.DeckJson>(req.Content);
            Console.WriteLine(deckJson.Id);

            // Add Deck
            try
            {
                BL.Services.UserService.SetActiveDeck(userid, Guid.Parse(deckJson.Id));
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
        #region userprofile

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
        
        #endregion
    }
}
