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
            gameServer.router.PostRoutes.Add("/register", PL.Controller.UserController.Register);
            gameServer.router.PostRoutes.Add("/login", PL.Controller.UserController.Login);
            
            gameServer.router.PostRoutes.Add("/packages", PL.Controller.CardController.AddPackage);
            gameServer.router.PostRoutes.Add("/transactions/packages", PL.Controller.CardController.BuyPackage);
            
            gameServer.router.GetRoutes.Add("/cards", PL.Controller.CardController.ShowCards);

            gameServer.Run();
        }
    }
}
