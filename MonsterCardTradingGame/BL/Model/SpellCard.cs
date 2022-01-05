using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Model
{
    class SpellCard : Card
    {
        public ElementTypes ElementType { get; set; }

        SpellCard(string cardID, string title, int damage, ElementTypes elementType, string description = "")
            : base(cardID, title, damage, description)
        {
            ElementType = elementType;
        }
    }
}
