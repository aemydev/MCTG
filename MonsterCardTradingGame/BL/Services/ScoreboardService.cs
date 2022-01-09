using MonsterCardTradingGame.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.BL.Services
{
    class ScoreboardService
    {
        static DAL.Repository.IScoreboardRepository scorerepos = new DAL.Repository.ScoreboardRepository();

        public static void AddEntry(ScoreboardEntry newentry)
        {
            // Does user already have entry?
            ScoreboardEntry oldentry;
            bool entryExists = true;

            try
            {
                oldentry = scorerepos.GetEntry(newentry.UserId);
            }
            catch
            {
                entryExists = false;
            }

            if (entryExists)
            {
                try
                {
                    scorerepos.Update(newentry);
                }
                catch
                {
                    throw;
                }

            }
            else
            {
                try
                {
                    scorerepos.Create(newentry);
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}
