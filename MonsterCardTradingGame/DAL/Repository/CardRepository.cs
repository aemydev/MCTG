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
        private const string TABLE_NAME = "card";

        /*
         *  Create
         */
        public void Create(Card card)
        {
            try
            {
                Dictionary<string, object> keyValue = new();
                keyValue.Add("card_id", card.CardID);
                keyValue.Add("title", card.Title);
                keyValue.Add("description", card.Description);
                keyValue.Add("damage", card.Damage);
                db.Insert(TABLE_NAME, keyValue);
            }
            catch(System.Exception e)
            {
                Console.WriteLine(e.Message);
                throw; 
            }
        }

        public void Delete(Card card)
        {
            throw new NotImplementedException();
        }

        public /*IEnumerable<Card>*/ void GetAll()
        {
            List<string> keys = new() { "*" };
            try
            {
                List<object> result = db.Select(TABLE_NAME, keys);

                foreach (var item in result)
                {
                    Console.WriteLine(item.ToString());
                }
            }
            catch
            {
                throw;
            }
        }

      public List<string> GetAllCardIdsWithoutOwner()
        {
            throw new NotImplementedException();

            /*
             List<string> keys = new (){ "card_id" };
             string where = "WHERE owner is null";

             try
             {
                 return db.Select(TABLE_NAME, keys, where, "LIMIT 5");

             }
             catch
             {
                 throw;
             }
            */
        }

        public Card GetById(int id)
        {
            throw new NotImplementedException();
        }

        /*
         *  Update
         */
        public void Update(Card card)
        {
           
        }

        public void UpdateOWner(int user_id, string card_id)
        {
            Dictionary<string, string> items = new();
            items.Add("owner", "user_id");
            try
            {
                db.Update(TABLE_NAME, items, "WHERE card_id == card_id");

            }
            catch
            {
                Console.WriteLine("Oh oh error");
            }
        }
    }
}
