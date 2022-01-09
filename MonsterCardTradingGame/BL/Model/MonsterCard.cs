using MonsterCardTradingGame.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Model
{
    public enum MonsterType { Goblin, Dragon, Wizard, Orc, Knight, Kraken, FireElve }

    class MonsterCard : Card
    {
        public MonsterType MonsterType { get; set; }

        public MonsterCard(Guid cardid, string cardName, string description, int damage, CardTypes cardType, ElementTypes elementType, MonsterType monsterType, Guid? ownerid = null)
        : base(cardid, cardName, description, damage, cardType, elementType, ownerid)
        {
            MonsterType = monsterType;
        }
    }
}
