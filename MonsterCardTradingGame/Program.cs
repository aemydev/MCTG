using System;

namespace MonsterCardTradingGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(" === Monster Card Trading Game === ");
            Server.HttpServer gameServer = new Server.HttpServer(8080);
            gameServer.Run();
        }
    }
}
