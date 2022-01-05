using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.BL.Controller
{
    class CardController
    {
        static DAL.Repository.ICardRepository Cardrepos = new DAL.Repository.CardRepository();

        /*
         *  Add new Package
         */
        public static void AddCard(Model.Card card)
        {
            try
            {
                Cardrepos.Create(card);

            }
            catch
            {
                throw;
            }


        }
    }
}
