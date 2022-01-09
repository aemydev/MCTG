using MonsterCardTradingGame.Model;
using MonsterCardTradingGame.Exceptions;
using System;
using System.Threading;

namespace MonsterCardTradingGame.BL.Services
{
    class GameService
    {
        private static bool gameEnded;
        private static bool accepted;
        private static string Winner;

        public GameService()
        {
            gameEnded = false;
            accepted = false;
            Winner = "";
        }

        public string StartBattle(string username)
        {
            Console.WriteLine($"[{DateTime.UtcNow}\t StartBattle({username})");

            // Active deck set for user?
            User user;
            try
            {
                user = UserService.GetUserByUsername(username);
            }
            catch
            {
                throw;
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
                Thread.Sleep(500);
            }

            while (!gameEnded)
            {
                Thread.Sleep(500);
            }

            return Winner;
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
            Console.WriteLine($"[{DateTime.Now}]\t OnBattleEnd-Event invoked. Set gameEnded = true");
            Winner = e.winner;
            gameEnded = true;   
        }

        /*
         *  Search for and join Battle
         */
        public string JoinBattle(string username)
        {
            Console.WriteLine($"[{DateTime.UtcNow}\t JoinBattle({username})");
            
            // Active deck set for user?
            User user;
            try
            {
                user = UserService.GetUserByUsername(username);
            }
            catch
            {
                throw;
            }

            if (user.ActiveDeckId.ToString() == "")
            {
                throw new HttpException("deck not set");
            }

            // Check if there are open battle-requests (1min)
            // Current Time
            var startTime = DateTime.Now;
            var endTime = DateTime.Now.AddMinutes(1);
            bool stopSearch = false;
            Guid battleId;

            Console.WriteLine($"[{DateTime.UtcNow}\t ({username}) is searching for a battle...");

            while (!stopSearch)
            {
                if (DateTime.Compare(DateTime.Now, endTime) >= 0)
                {
                    stopSearch = true;
                    throw new GameException("no game found"); // Time is up, no battles found
                }
                else if (Program.BattleRequest.TryDequeue(out BattleRequest battleReq)){
                    stopSearch = true;
                    battleId = battleReq.Accept(user);
                    Program.battles[battleId].OnBattleEnd += Join_Battle_Battle_OnBattleEnd;
                }
            }

            while (!gameEnded)
            {
                Thread.Sleep(500);
            }

            return Winner;
        }

        private static void Join_Battle_Battle_OnBattleEnd(object sender, Battle.OnBattleEndArgs e)
        {
            Console.WriteLine($"[{DateTime.Now}]\tOnBattleEnd-Event invoked. Set gameEnded = true");
            Winner = e.winner;
            gameEnded = true;

            //Program.battles[e.battleid].Status = BattleStates.Done;
        }
    }
}
