using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Utility.Json
{
    class CardJson
    {
        public string Name { get; set; }
        public int Damage { get; set; }
        public Model.CardTypes Type { get; set; }
        public Model.ElementTypes ElementType { get; set; }
        public string Description { get; set; }
       // public Model.MonsterType MonsterType { get; set; }
    }
}
