using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Model
{
    public enum ElementTypes {Normal, Water, Fire}
    public enum CardTypes { Spell, Monster }

    public class Card
    {
        public Guid CardID { get; set; }
        public string Title { get; set; }
        public int Damage { get; set; }
        public string Description { get; set; }
        public CardTypes Type { get; set; }
        public ElementTypes ElementType { get; set; }
        public Guid? OwnerId { get; set; } // nullable, because card can have no owner

        public Card(Guid cardid, string cardName, string description, int damage, CardTypes cardType, ElementTypes elementType, Guid? ownerid=null)
        {
            CardID = cardid;
            Title = cardName;
            Damage = damage;
            Description = description;
            Type = cardType;
            ElementType = elementType;
            OwnerId = ownerid;
        }
    }
}
