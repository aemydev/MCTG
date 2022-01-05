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
            gameServer.router.PostRoutes.Add("/register", Server.Router.Endpoints.Register);
            gameServer.router.PostRoutes.Add("/login", Server.Router.Endpoints.Login);
            gameServer.router.PostRoutes.Add("/packages", Server.Router.Endpoints.Packages);

            gameServer.Run();
        }
    }
}
