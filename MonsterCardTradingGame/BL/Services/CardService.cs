using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterCardTradingGame.Exceptions;
using MonsterCardTradingGame.Model;
using MonsterCardTradingGame.DAL;

namespace MonsterCardTradingGame.BL.Services
{ 
    public class CardService
    {
        public static DAL.Repository.ICardRepository cardrepos;
        private UserService UserService = new UserService();

        /*
         *  Constructor, Seam for Unit-Tests -> we are able to plug in our mock repos
         */
        public CardService()
        {
            cardrepos = new DAL.Repository.CardRepository();
        }

        public CardService(DAL.Repository.ICardRepository repos)
        {
            cardrepos = repos;
        }

        /*
        *  Add new Card to DB
        */
        public void AddPackage(List<Card> cards)
        {
            // Validate the Cards to be added
            if(cards.Count > 5)
            {
                throw new ServiceException("too many cards");

            }
            
            if(cards.Count < 5)
            {
                throw new ServiceException("not enough cards");
            }

            // Try adding new Package to Database:
            try
            {
                cardrepos.CreateMultiple(cards);
            }
            catch
            {
                throw new ServiceException();
            }
        }

        /*
         *  Show all Cards
         */
        public List<Card> ShowAllCards(Guid userid)
        {
            List<Card> cards;

            try
            {
                cards = cardrepos.GetAllByUser(userid);
                return cards;
            }
            catch
            {
                throw new ServiceException("db error");
            }
        }

        /*
         *  Get Package
         */
        public IEnumerable<Model.Card> AquirePackage(string username)
        {
            Console.WriteLine($"[{DateTime.UtcNow}]\tAquire new Package for \"{username}\"");

            // Try to get user from db:
            if (!UserService.GetUserByUsername(username, out User user))
            {
                throw new ServiceException("user not found");
            }
            
            // enough coins to buy package?
            if(user.Coins < 5)
            {
                Console.WriteLine($"[{DateTime.UtcNow}]\tError: user \"{username}\" has not enough money");
                throw new ServiceException("no money"); 
            }
     
            // get package
            try
            {
                return cardrepos.GetPackage(user.UserId);
            }
            catch
            {
                throw new ServiceException("db error");
            }
        }
    }
}
