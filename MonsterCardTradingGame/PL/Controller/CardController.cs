using MonsterCardTradingGame.Model;
using MonsterCardTradingGame.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using MonsterCardTradingGame.Exceptions;

namespace MonsterCardTradingGame.PL.Controller
{
    class CardController
    {
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
                cards = BL.Services.CardService.ShowAllCards(username);
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
               cards.Add(new Model.Card(Guid.NewGuid(), jsoncard.Name, jsoncard.Description, jsoncard.Damage, (CardTypes)jsoncard.Type, (ElementTypes)jsoncard.ElementType));        
            }

            if (BL.Services.CardService.AddPackage(cards))
            {
                // if everything worked -> return success message
                res = new HttpResponse(HttpStatusCode.Created);
                res.AddContent("application/json", "{\"message\":\"Package successfully created.\"}");
                return res;
            }
            else
            {  
                res = new HttpResponse(HttpStatusCode.InternalServerError);
                res.AddContent("application/json", "{\"message\":\"Error creating Package. Please try again.\"}");
                return res;
            }
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
               cards =  BL.Services.CardService.AquirePackage(username);
            }
            catch
            {
                res = new HttpResponse(HttpStatusCode.Conflict);
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
