using MonsterCardTradingGame.Exceptions;
using MonsterCardTradingGame.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.PL.Controller
{
    public static class GameController
    {
        /*
         * /battle/new, POST
         */
        public static HttpResponse NewBattle(HttpRequest req)
        {
            HttpResponse res;
            String winner;

            // Valid token?
            if (!BL.Services.AuthService.AuthToken(req.Headers, out string username, out Guid userid))
            {
                res = new HttpResponse(HttpStatusCode.Forbidden);
                res.AddContent("application/json", "{\"message\":\"Access denied. Invalid token.\"}");
                return res;
            }

            BL.Services.GameService game = new();

            try
            {
                winner = game.StartBattle(username);

            }catch(GameException e) when (e.Message =="no deck set")
            {
                res = new HttpResponse(HttpStatusCode.NotFound);
                res.AddContent("application/json", "{\"message\":\"Something went wrong. Please try again.\"}");
                return res;
            }
            catch(GameException e) when (e.Message=="no game found")
            {
                res = new HttpResponse(HttpStatusCode.NotFound);
                res.AddContent("application/json", "{\"message\":\"Currently are not battles availiable.\"}");
                return res;
            }
            catch (SystemException e)
            {
                Console.WriteLine(e.Message);
                res = new HttpResponse(HttpStatusCode.InternalServerError);
                res.AddContent("application/json", "{\"message\":\"Something went wrong. Please try again.\"}");
                return res;
            }

            res = new HttpResponse(HttpStatusCode.OK);
            res.AddContent("application/json", $"{{\"message\":\"Battle Ended. {winner} won the Battle\"}}");
            return res;
        }

        /*
         * /battle/join, POST
         */
        public static HttpResponse JoinBattle(HttpRequest req)
        {
            HttpResponse res;

            // Valid token?
            if (!BL.Services.AuthService.AuthToken(req.Headers, out string username, out Guid userid))
            {
                res = new HttpResponse(HttpStatusCode.Forbidden);
                res.AddContent("application/json", "{\"message\":\"Access denied. Invalid token.\"}");
                return res;
            }

            string winner;
            BL.Services.GameService game = new();

            try
            {
                winner = game.JoinBattle(username);
            }
            catch (GameException e) when (e.Message == "no deck set")
            {
                res = new HttpResponse(HttpStatusCode.NotFound);
                res.AddContent("application/json", "{\"message\":\"Something went wrong. Please try again.\"}");
                return res;
            }
            catch (GameException e) when (e.Message == "no game found")
            {
                res = new HttpResponse(HttpStatusCode.NotFound);
                res.AddContent("application/json", "{\"message\":\"Currently are not battles availiable.\"}");
                return res;
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                res = new HttpResponse(HttpStatusCode.InternalServerError);
                res.AddContent("application/json", "{\"message\":\"Join Battle\"}");
                return res;
            }

            res = new HttpResponse(HttpStatusCode.OK);
            res.AddContent("application/json",$"{{\"message\":\"Battle Ended. {winner} won the Battle \"}}");
            return res;
        }
    }
}
