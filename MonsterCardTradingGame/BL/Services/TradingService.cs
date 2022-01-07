using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.BL.Services
{
    class TradingService
    {
        static DAL.Repository.IUserRepository userrepos = new DAL.Respository.UserRepository();
        static DAL.Repository.ICardRepository cardrepos = new DAL.Repository.CardRepository();

        public static bool AquirePackage(string username)
        {
            // Transction??

            // Get user from DB
            Model.User user = userrepos.GetByName(username);

            if(user.Coins < 5)
            {
                // Insufficient Balance
                return false;
            }

            // Get 5 Cards without owner
           
            return true;
        }
    }
}
