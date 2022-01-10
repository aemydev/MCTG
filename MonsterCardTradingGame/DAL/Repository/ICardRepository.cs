using MonsterCardTradingGame.Model;
using System;
using System.Collections.Generic;

namespace MonsterCardTradingGame.DAL.Repository
{
    public interface ICardRepository
    {
        // Create
        //void Create(Card card);
        public void CreateMultiple(List<Card> cards);

        // Read
        // IEnumerable<Card> GetAll();
        public List<Card> GetAllByUser(Guid userid);
        //void GetById(Guid id);

        // Update
        //void UpdateOwner(Guid cardid, Guid userid);
        List<Card> GetPackage(Guid userid); // meh

        // Delete
        //void Delete(Card card);
    }
}
