using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Utility.Json
{
    class CardJson
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public float Damage { get; set; }
        public string Type { get; set; }
        public Model.ElementTypes ElementType { get; set; }
        public string Description { get; set; }

    }
}
