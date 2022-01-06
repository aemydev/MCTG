using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.BL.Services
{
    class CardService
    {
        static DAL.Repository.ICardRepository Cardrepos = new DAL.Repository.CardRepository();

        /*
         *  Add new Card to DB
         */
        public static bool AddCard(Model.Card card)
        {
            try
            {
                Cardrepos.Create(card);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /*
         *  Show all Cards
         */
        public static void ShowAllCards()
        {
            try
            {
                Cardrepos.GetAll();
            }
            catch
            {
                throw;
            }
        }

        /*
         *  Get Package
         */
        public static void AquirePackage(int user_id)
        {
            
            // Get Array of 5 Cards
            List<string> package = Cardrepos.GetAllCardIdsWithoutOwner();

            foreach(string card_id in package)
            {
                // Update ownership of Card
                // UPDATE card SET owner = 'calue
               // Cardrepos.UpdateOwner();
            }

            // zuerst select, dann update

            // SELECT * FROM card WHERE owner 
            // Update owner of 5 cards to username 

            // return aquired Package
            
        }
    }
}
