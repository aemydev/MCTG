using MonsterCardTradingGame.Exceptions;
using MonsterCardTradingGame.Model;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MonsterCardTradingGame.BL.Services
{
    class GameService
    {
        private static UserService UserService = new UserService();

        private static bool gameEnded;
        private static bool accepted;
        private static string Winner;
        private static List<string> BattleLog;

        public GameService()
        {
            gameEnded = false;
            accepted = false;
            Winner = "";
        }

        public BattleResult StartBattle(string username)
        {
            // Active deck set for user?
            if (!UserService.GetUserByUsername(username, out User user))
            {
                throw new ServiceException("user not found");
            }

            if (user.ActiveDeckId.ToString() == "")
            {
                Console.WriteLine($"[{DateTime.UtcNow}]\tError: No active deck set for \"{username}\"");
                throw new GameException("deck not set");
            }

            // Create new Battle-Request
            BattleRequest battleReq = new BattleRequest(user);
            Program.BattleRequest.Enqueue(battleReq);

            Console.WriteLine($"[{DateTime.UtcNow}\t New Battle Request created. Waiting for other players to join ...)");

            // Subscribe to event
            battleReq.OnAccepted += StartBattle_BattleReq_OnAccepted;

            while (!accepted)
            {
                Thread.Sleep(1000);
            }

            while (!gameEnded)
            {
                Thread.Sleep(1000);
            }

            return new BattleResult(Winner, BattleLog);
        }

        /*
         *  When user accepts Battle Request
         */
        private static void StartBattle_BattleReq_OnAccepted(object sender, BattleRequest.OnAcceptedArgs e)
        {
            Console.WriteLine($"[{DateTime.UtcNow}\tBattleRequest accepted");
            accepted = true;
            // Wait for battle to end
            // Subscribe to event
            Battle battle = Program.battles[e.battleId];
            battle.OnBattleEnd += Start_Battle_Battle_OnBattleEnd;
            battle.Start();
        }

        private static void Start_Battle_Battle_OnBattleEnd(object sender, Battle.OnBattleEndArgs e)
        {
            Winner = e.winner;
            BattleLog = e.battleLog;
            gameEnded = true;
        }

        /*
         *  Search for and join Battle
         */
        public BattleResult JoinBattle(string username)
        {
            // Active deck set for user?
            if (!UserService.GetUserByUsername(username, out User user))
            {
                throw new Exception("user not found");
            }

            if (user.ActiveDeckId.ToString() == "")
            {
                throw new HttpException("deck not set");
            }

            // Check if there are open battle-requests (1min)
            DateTime endTime = DateTime.Now.AddMinutes(1);
            bool stopSearch = false;
            Guid battleId;

            Console.WriteLine($"[{DateTime.UtcNow}\t \"{username}\" is searching for a battle...");

            while (!stopSearch)
            {
                if (DateTime.Compare(DateTime.Now, endTime) >= 0)
                {
                    stopSearch = true;
                    throw new GameException("no game found"); // Time is up, no battles found
                }
                else if (Program.BattleRequest.TryDequeue(out BattleRequest battleReq))
                {
                    stopSearch = true;
                    battleId = battleReq.Accept(user);
                    Program.battles[battleId].OnBattleEnd += Join_Battle_Battle_OnBattleEnd;
                }
            }

            while (!gameEnded)
            {
                Thread.Sleep(1000);
            }

            return new BattleResult(Winner,BattleLog);
        }

        private static void Join_Battle_Battle_OnBattleEnd(object sender, Battle.OnBattleEndArgs e)
        {
            Console.WriteLine($"[{DateTime.Now}]\tOnBattleEnd-Event invoked. Set gameEnded = true");
            Winner = e.winner;
            BattleLog = e.battleLog;
            gameEnded = true;

            //Program.battles[e.battleid].Status = BattleStates.Done;
        }
    }
}
