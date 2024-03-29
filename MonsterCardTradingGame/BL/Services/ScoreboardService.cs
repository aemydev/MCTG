﻿using MonsterCardTradingGame.Model;
using System;
using System.Collections.Generic;

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

        public static bool GetStats(Guid userid, out ScoreboardEntry userscore)
        {
            try
            {
                userscore = scorerepos.GetEntry(userid);
                return true;
            }
            catch
            {
                userscore = null;
                return false;
            }
        }

        public static bool GetScoreboard(Guid userid, out List<ScoreboardEntry> scoreboard)
        {
            try
            {
                scoreboard = scorerepos.GetAll();
                return true;
            }
            catch
            {
                scoreboard = null;
                return false;
            }
        }
    }
}
