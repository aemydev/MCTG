using System.Collections.Generic;

namespace MonsterCardTradingGame.Model
{
    public class BattleResult
    {
        public string Winner { get; set; }
        public List<string> BattleLog { get; set; }

        public BattleResult(string winner, List<string> battleLog)
        {
            Winner = winner;
            BattleLog = battleLog;
        }
    }
}
