using System;
using System.Net;

namespace MonsterCardTradingGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(" === Monster Card Trading Game === ");
            Server.HttpServer gameServer = new Server.HttpServer(10001);

            // Add the Routes:
            gameServer.router.addPostRoute("/register", new Router.Endpoints.Register());
            gameServer.router.addPostRoute("/login", new Router.Endpoints.Login());

            gameServer.Run();
        }
    }
}
