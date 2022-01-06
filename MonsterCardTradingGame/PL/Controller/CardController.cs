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
    class CardController
    {
        /*
         *  /cards, GET
         *  Show all Cards for User
         */
        public static HttpResponse ShowCards(Server.HttpRequest req)
        {
            HttpResponse res;

            if (!BL.Services.AuthService.Auth(req))
            {
                res = new HttpResponse(HttpStatusCode.Forbidden);
                res.AddContent("application/json", "{\"message\":\"Access denied. Please Login.\"}");
                return res;
            }
            string token = req.Headers["Authorization"]; // no check required, because if token would not exits, if above would have failed
            string username = BL.Services.AuthService.getUserNameFromToken(token);

            BL.Services.CardService.ShowAllCards();

            res = new HttpResponse(HttpStatusCode.Forbidden);
            res.AddContent("application/json", "{\"message\":\"Happy Peppy.\"}");
            return res;
        }

        /*
         *  /packages, POST (only Admin)
         *  Create new Packages
         */
        public static HttpResponse AddPackage(Server.HttpRequest req)
        {
            HttpResponse res;

            if (!BL.Services.AuthService.AuthAdmin(req))
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
                if(card_.Type == "Monster")
                {
                    Model.Card cardTemp = new Model.MonsterCard(card_.Id, card_.Name, card_.Damage, card_.Description);
                    if (!BL.Services.CardService.AddCard(cardTemp))
                    {
                        // if everything worked -> return success message
                        res = new HttpResponse(HttpStatusCode.Conflict);
                        res.AddContent("application/json", "{\"message\":\"Error creating Package.\"}");
                        return res;
                    }
                }
                else
                {
                    Model.Card cardTemp = new Model.SpellCard(card_.Id, card_.Name, card_.Damage, card_.ElementType, card_.Description);
                    if (!BL.Services.CardService.AddCard(cardTemp))
                    {
                        // if everything worked -> return success message
                        res = new HttpResponse(HttpStatusCode.Conflict);
                        res.AddContent("application/json", "{\"message\":\"Error creating Package.\"}");
                        return res;
                    }
                }
            }
            // if everything worked -> return success message
            res = new HttpResponse(HttpStatusCode.Created);
            res.AddContent("application/json", "{\"message\":\"Package successfully created\"}");
            return res;
        }       
        
        /*
         *  /transactions/packages, POST
         *  Aquire new Package (5 Cards)
         */
        public static HttpResponse BuyPackage(Server.HttpRequest req)
        {
            HttpResponse res;

            if (!BL.Services.AuthService.Auth(req))
            {
                res = new HttpResponse(HttpStatusCode.Forbidden);
                res.AddContent("application/json", "{\"message\":\"Access denied.\"}");
                return res;
            }

            // Get Username from Token
            string username = BL.Services.AuthService.getUserNameFromToken(req.Headers["Authorization"]);
            Console.WriteLine($"Username from Token: {username}");
            // Get User_ID from username
            Model.User user = BL.Services.UserService.GetUserByUsername(username);
            
            //BL.Controller.CardController.AquirePackage(username);

            res = new HttpResponse(HttpStatusCode.OK);
            res.AddContent("application/json", "{\"message\":\"Access denied.\"}");
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
