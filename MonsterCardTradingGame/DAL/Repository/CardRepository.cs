using MonsterCardTradingGame.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.DAL.Repository
{
    class CardRepository : ICardRepository
    {
        Postgres.DBAccess db = Postgres.DBAccess.Instance;

        /*
         *  Create
         */
        public void Create(Card card)
        {
            try
            {
                db.Insert(card);
            }
            catch(System.Exception e)
            {
                Console.WriteLine(e.Message);
                throw; 
            }
        }

        public void DeleteUser(Card card)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Card> GetAllUser()
        {
            throw new NotImplementedException();
        }

        public Card GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Card GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public void UpdateUser(Card card)
        {
            throw new NotImplementedException();
        }
    }
}
