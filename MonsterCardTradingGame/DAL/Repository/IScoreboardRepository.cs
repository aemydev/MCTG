using MonsterCardTradingGame.Model;
using System;
using System.Collections.Generic;

namespace MonsterCardTradingGame.DAL.Repository
{
    interface IScoreboardRepository
    {

        public void Create(ScoreboardEntry entry);

        public void Update(ScoreboardEntry entry);

        public ScoreboardEntry GetEntry(Guid userid);

        public List<ScoreboardEntry> GetAll();
    }
}
