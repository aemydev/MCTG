using MonsterCardTradingGame.Model;
using System;
using System.Collections.Generic;

namespace MonsterCardTradingGame.DAL.Repository
{
    public interface IDeckRepository
    {
        public Deck GetDeckById(Guid id);
        public List<Deck> GetAll(Guid userid);

        public void AddDeck(Utility.Json.DeckJson deck, Guid owner);
    }
}
