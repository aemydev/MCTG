using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterCardTradingGame.Exceptions;
using MonsterCardTradingGame.Model;

namespace MonsterCardTradingGame.BL.Services
{ 
    class CardService
    {
        public static DAL.Repository.ICardRepository cardrepos = new DAL.Repository.CardRepository();

        

        /*
        *  Add new Card to DB
        */
        public static bool AddPackage(List<Card> cards)
        {
            // Auth user

            try
            {
                cardrepos.CreateMultiple(cards);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /*
         *  Show all Cards
         */
        public static List<Model.Card> ShowAllCards(string username)
        {
            Model.User user = BL.Services.UserService.GetUserByUsername(username);
            List<Model.Card> cards = new();

            try
            {
                cards = cardrepos.GetAllByUser(user.UserId);
                return cards;
            }
            catch
            {
                throw;
            }
        }

        /*
         *  Get Package
         */
        public static IEnumerable<Model.Card> AquirePackage(string username)
        {
            Console.WriteLine($"[{DateTime.UtcNow}]\tAquire new Package for \"{username}\"");

            // Get user
            Model.User user;
            try
            {
               user = UserService.GetUserByUsername(username);
            }
            catch
            {
                // Error Handling
                throw new HttpException("404 Not found");
            }

            // Does user have enough coins to buy a package?
            if(user.Coins < 5)
            {
                Console.WriteLine($"[{DateTime.UtcNow}]\tError: user \"{username}\" has not enough money");
                throw new HttpException("No Money"); // Not enough coins
            }

            try
            {
                return cardrepos.GetPackage(user.UserId);
            }
            catch
            {
                throw new HttpException("500 Internal Server Error");
            }
        }
    }
}
