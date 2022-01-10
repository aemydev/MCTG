using MonsterCardTradingGame.Model;
using MonsterCardTradingGame.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using MonsterCardTradingGame.Exceptions;

namespace MonsterCardTradingGame.PL.Controller
{
    public class CardController
    {
        public static BL.Services.CardService cardService = new BL.Services.CardService();
        
        /*
         *  /cards, GET
         *  Show all Cards for User
         */
        public static HttpResponse ShowCards(HttpRequest req)
        {
            HttpResponse res;
            
            // Valid token?
            if (!BL.Services.AuthService.AuthToken(req.Headers, out string username, out Guid userid))
            {
                // no -> send error response
                res = new HttpResponse(HttpStatusCode.Forbidden);
                res.AddContent("application/json", "{\"message\":\"Access denied. Token invalid.\"}");
                return res;
            }

            List<Card> cards = new();

            try
            {
                cards = cardService.ShowAllCards(userid);
            }
            catch(HttpException e) when (e.Message == "no cards")
            {
                // User has no cards
                res = new HttpResponse(HttpStatusCode.NotFound);
                res.AddContent("application/json", "{\"message\":\"User does not own any cards.\"}");
                return res;
            }
            catch
            {
                res = new HttpResponse(HttpStatusCode.InternalServerError);
                res.AddContent("application/json", "{\"message\":\"Something went wrong. Please try again.\"}");
                return res;
            }

            // Success -> send cards
            string jsonString = JsonConvert.SerializeObject(cards);
            res = new HttpResponse(HttpStatusCode.OK);
            res.AddContent("application/json", $"{{\"message\":\"Success!\", \"content\":{jsonString}}}");
            return res;
        }

        /*
         *  /packages, POST (only Admin)
         *  Create new Packages
         */
        public static HttpResponse AddPackage(HttpRequest req)
        {
            HttpResponse res;
            
            // Valid token? username == admin? -> only "admin" can create new packages
            if (!BL.Services.AuthService.AuthAdmin(req))
            {
                // Send Error Response
                res = new HttpResponse(HttpStatusCode.Forbidden);
                res.AddContent("application/json", "{\"error\":\"Access denied. Only admins can create packages.\"}");
                return res;
            }

            List<Card> cards = new();

            // String -> Json (Content)
            var package = JsonConvert.DeserializeObject<List<Utility.Json.CardJson>>(req.Content);

            // CardJson -> Json
            foreach(Utility.Json.CardJson jsoncard in package)
            {       
               cards.Add(new Card(Guid.NewGuid(), jsoncard.Name, jsoncard.Description, jsoncard.Damage, (CardTypes)jsoncard.Type, (ElementTypes)jsoncard.ElementType));        
            }

            try
            {
                cardService.AddPackage(cards);
                
            }catch (ServiceException e) when (e.Message == "too many cards")
            {
                res = new HttpResponse(HttpStatusCode.BadRequest);
                res.AddContent("application/json", "{\"message\":\"Too many cards. New Packages must contain 5 Cards.\"}");
                return res;

            }catch(ServiceException e) when (e.Message == "not enough cards")
            {
                res = new HttpResponse(HttpStatusCode.Created);
                res.AddContent("application/json", "{\"message\":\"Package successfully created.\"}");
                return res;
            }
            catch
            {
                res = new HttpResponse(HttpStatusCode.InternalServerError);
                res.AddContent("application/json", "{\"message\":\"Something went wrong.\"}");
                return res;
            }

            res = new HttpResponse(HttpStatusCode.Created);
            res.AddContent("application/json", "{\"message\":\"Package successfully created.\"}");
            return res;
        }

        /*
         *  /transactions/packages, POST
         *  Aquire new Package (5 Cards)
         */
        public static HttpResponse BuyPackage(Server.HttpRequest req)
        {
            HttpResponse res;
            IEnumerable<Card> cards;

            if (!BL.Services.AuthService.AuthToken(req.Headers, out string username, out Guid userid))
            {
                res = new HttpResponse(HttpStatusCode.Forbidden);
                res.AddContent("application/json", "{\"message\":\"Access denied.\"}");
                return res;
            }

            try{
               cards = cardService.AquirePackage(username);
            }
            catch (ServiceException e) when (e.Message == "error getting user")
            {
                res = new HttpResponse(HttpStatusCode.NotFound);
                res.AddContent("application/json", "{\"message\":\"Could not get user.\"}");
                return res;
            }catch(ServiceException e) when (e.Message == "no money")
            {
                res = new HttpResponse(HttpStatusCode.Conflict);
                res.AddContent("application/json", "{\"message\":\"Not enough money\"}");
                return res;
            }
            catch
            {
                res = new HttpResponse(HttpStatusCode.InternalServerError);
                res.AddContent("application/json", "{\"message\":\"Something went wrong.\"}");
                return res;
            }

            string jsonString = JsonConvert.SerializeObject(cards);    
            res = new HttpResponse(HttpStatusCode.OK);
            res.AddContent("application/json", $"{{\"message\":\"Package successfully aquired\",\"content\":{jsonString}}}");
            return res;
        }

        /*
         *  /tradings, GET
         *  Show Trade-Deals
         */
        public static HttpResponse ShowTradeDeals(Server.HttpRequest req)
        {
            throw new NotImplementedException();
        }


        /*
         *  /tradings/card_id, POST
         *  Trade Cards
         */
        public static HttpResponse TradeCards(Server.HttpRequest req)
        {
            throw new NotImplementedException();
        }
    }
}
