using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Model
{
    enum ElementTypes {Normal, Water, Fire}

    public abstract class Card
    {
        public string CardID { get; set; }
        public string Title { get; set; }
        public float Damage {get; init;}
        public string Description { get; set; }
        public int OwnerID { get; set; }

        public Card(string cardID, string cardName, float damage, string description = "")
        {
            CardID = cardID;
            Title = cardName;
            Damage = Damage;
            Description = description;
        }
    }
}
