using MonsterCardTradingGame.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Server.Router
{
    class Router
    {
        public delegate Server.HttpResponse Endpoint(Server.HttpRequest req);
        public Dictionary<string, Endpoint> PostRoutes { get; set; } = new();
        public Dictionary<string, Endpoint> GetRoutes { get; set; } = new();
        public Dictionary<string, Endpoint> PutRoutes { get; set; } = new();
    }
}
