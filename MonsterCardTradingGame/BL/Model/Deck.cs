using MonsterCardTradingGame.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Model
{
    public class Deck
    {
        public Guid DeckId { get; set; }
        public Guid Owner { get; set; }
        public string Title { get; set; }
        public List<Card> Cards { get; set; } = new();

        public Deck(Guid deckid, Guid owner, string title)
        {
            DeckId = deckid;
            Owner = owner;
            Title = title;
        }
    }
}
