using MonsterCardTradingGame.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Router
{
    class Router
    {
        public Dictionary<string, Endpoints.IEndpoint> PostRoutes { get; set; }
        public Dictionary<string, Endpoints.IEndpoint> GetRoutes { get; set; }
        // put
        // delete

        public Router()
        {
            PostRoutes = new();
            GetRoutes = new();
        }

        public void addGetRoute(string route, Endpoints.IEndpoint calls)
        {
            GetRoutes.Add(route, calls);
        }

        public void addPostRoute(string route, Endpoints.IEndpoint calls)
        {
            PostRoutes.Add(route, calls);
        }


        public HttpResponse invoke(string method, string path, string reqContent = null) //returns status Code
        {
            HttpResponse res; 

            if (method == "POST")
            {
                if (PostRoutes[path].call(reqContent))
                {
                    res = new HttpResponse(HttpStatusCode.OK);
                    res.AddContent("application/json", "{\"response\":\"Success\"}");
                }
                else
                {
                    res = new HttpResponse(HttpStatusCode.Conflict);
                    res.AddContent("application/json", "{\"response\":\"Error asdfasdf - not found\"}");
                }
            }
            else
            {
                GetRoutes[path].call();
                res = new HttpResponse(HttpStatusCode.Conflict);
                res.AddContent("application/json", "{\"response\":\"Error asdfasdf - not found\"}");
            }

            return res; // Returns res
        }
    }
}
