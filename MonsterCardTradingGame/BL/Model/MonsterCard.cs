using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Model
{
    class MonsterCard : Card
    {
        public MonsterCard(string cardID, string title, float damage, string description) : base(cardID, title, damage, description)
        {
        }
    }
}
