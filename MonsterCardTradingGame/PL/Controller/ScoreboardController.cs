using MonsterCardTradingGame.Model;
using MonsterCardTradingGame.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace MonsterCardTradingGame.PL.Controller
{
    class ScoreboardController
    {
        public static HttpResponse ShowScoreboard(HttpRequest req)
        {
            HttpResponse res;
            List<ScoreboardEntry> scoreboard;

            if (!BL.Services.AuthService.AuthToken(req.Headers, out string username, out Guid userid))
            {
                res = new HttpResponse(HttpStatusCode.Forbidden);
                res.AddContent("application/json", "{\"message\":\"Access denied. Invalid token.\"}");
                return res;
            }

            Console.WriteLine($"[{DateTime.UtcNow}]\tGet Scorboard");

            try
            {
                BL.Services.ScoreboardService.GetScoreboard(userid, out List<ScoreboardEntry> scoreboarddb);
                scoreboard = scoreboarddb;
            }
            catch
            {
                res = new HttpResponse(HttpStatusCode.NotFound);
                res.AddContent("application/json", "{\"message\":\"Something went wrong\"}");
                return res;
            }

            string jsonString = JsonConvert.SerializeObject(scoreboard);

            res = new HttpResponse(HttpStatusCode.OK);
            res.AddContent("application/json", $"{{\"message\":\"Send Scoreboard\", \"content\":{jsonString}}}");
            return res;
        }

        public static HttpResponse ShowStats(HttpRequest req)
        {
            HttpResponse res;
            ScoreboardEntry score;

            if (!BL.Services.AuthService.AuthToken(req.Headers, out string username, out Guid userid))
            {
                res = new HttpResponse(HttpStatusCode.Forbidden);
                res.AddContent("application/json", "{\"message\":\"Access denied. Invalid token.\"}");
                return res;
            }

            Console.WriteLine($"[{DateTime.UtcNow}]\tGet Stats of \"{username}\"");

            try
            {
                BL.Services.ScoreboardService.GetStats(userid, out ScoreboardEntry userscore);
                score = userscore;
            }
            catch
            {
                res = new HttpResponse(HttpStatusCode.NotFound);
                res.AddContent("application/json", "{\"message\":\"Something went wrong\"}");
                return res;
            }

            string jsonString = JsonConvert.SerializeObject(score);

            res = new HttpResponse(HttpStatusCode.OK);
            res.AddContent("application/json", $"{{\"message\":\"Send stats for {username}\", \"content\":{jsonString}}}");
            return res;
        }
    }
}
