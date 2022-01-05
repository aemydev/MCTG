using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Model
{
    enum ElementTypes {None, Water, Fire, Normal}

    public abstract class Card
    {
        public string CardID { get; set; }
        public string Title { get; set; }
        public int Damage {get; init;}
        public string Description { get; set; }

        public Card(string cardID, string cardName, int damage, string description = "")
        {
            CardID = cardID;
            Title = cardName;
            Damage = Damage;
        }
    }
}
