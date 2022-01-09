using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Model
{
    class ScoreboardEntry
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int GamesPlayed { get; set; }
        public int Elo { get; set; }

        public ScoreboardEntry(Guid userid, int elo)
        {
            UserId = userid;
            Elo = elo;
        }

        public ScoreboardEntry(Guid id, Guid userid, int elo, int games_played)
        {
            Id = id;
            UserId = userid;
            Elo = elo;
            GamesPlayed = games_played;
        }
    }
}
