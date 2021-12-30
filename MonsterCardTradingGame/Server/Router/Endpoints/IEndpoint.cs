using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Router.Endpoints
{
    interface IEndpoint
    {
        public bool call(string content = null);
    }
}
