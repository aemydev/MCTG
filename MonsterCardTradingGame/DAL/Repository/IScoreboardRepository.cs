using MonsterCardTradingGame.Exceptions;
using MonsterCardTradingGame.Model;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.DAL.Repository
{
    interface IScoreboardRepository
    {

        public void Create(Model.ScoreboardEntry entry);


        public void Update(Model.ScoreboardEntry entry);

        public ScoreboardEntry GetEntry(Guid userid);
     }
}
