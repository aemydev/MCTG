using MonsterCardTradingGame.Model;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;

namespace MonsterCardTradingGame
{
    class Program
    {
        public static ConcurrentDictionary<Guid, Model.Battle> battles = new();
        public static ConcurrentQueue<BattleRequest> BattleRequest = new();

        static void Main(string[] args)
        {
                   
            Console.WriteLine("MMMMMMMM               MMMMMMMM         CCCCCCCCCCCCC TTTTTTTTTTTTTTTTTTTTTTT        GGGGGGGGGGGGG");
            Console.WriteLine("M:::::::M             M:::::::M      CCC::::::::::::C T:::::::::::::::::::::T     GGG::::::::::::G");
            Console.WriteLine("M::::::::M           M::::::::M    CC:::::::::::::::C T:::::::::::::::::::::T   GG:::::::::::::::G");
            Console.WriteLine("M:::::::::M         M:::::::::M   C:::::CCCCCCCC::::C T:::::TT:::::::TT:::::T  G:::::GGGGGGGG::::G");
            Console.WriteLine("M::::::::::M       M::::::::::M  C:::::C       CCCCCC TTTTTT  T:::::T  TTTTTT G:::::G       GGGGGG");
            Console.WriteLine("M:::::::::::M     M:::::::::::M C:::::C                       T:::::T        G:::::G");
            Console.WriteLine("M:::::::M::::M   M::::M:::::::M C:::::C                       T:::::T        G:::::G");
            Console.WriteLine("M::::::M M::::M M::::M M::::::M C:::::C                       T:::::T        G:::::G    GGGGGGGGGG");
            Console.WriteLine("M::::::M  M::::M::::M  M::::::M C:::::C                       T:::::T        G:::::G    G::::::::G");
            Console.WriteLine("M::::::M   M:::::::M   M::::::M C:::::C                       T:::::T        G:::::G    GGGGG::::G");
            Console.WriteLine("M::::::M    M:::::M    M::::::M C:::::C                       T:::::T        G:::::G        G::::G");
            Console.WriteLine("M::::::M     MMMMM     M::::::M  C:::::C       CCCCCC         T:::::T         G:::::G       G::::G");
            Console.WriteLine("M::::::M               M::::::M   C:::::CCCCCCCC::::C       TT:::::::TT        G:::::GGGGGGGG::::G");
            Console.WriteLine("M::::::M               M::::::M    CC:::::::::::::::C       T:::::::::T         GG:::::::::::::::G");
            Console.WriteLine("M::::::M               M::::::M      CCC::::::::::::C       T:::::::::T           GGG::::::GGG:::G");
            Console.WriteLine("MMMMMMMM               MMMMMMMM         CCCCCCCCCCCCC       TTTTTTTTTTT              GGGGGGGGGGG");
            Console.WriteLine("");
            Console.WriteLine("");


            Server.HttpServer gameServer = new Server.HttpServer(10001);

            // Add the Routes:
            gameServer.router.PostRoutes.Add("/register", PL.Controller.UserController.Register);
            gameServer.router.PostRoutes.Add("/login", PL.Controller.UserController.Login);
            
            gameServer.router.PostRoutes.Add("/packages", PL.Controller.CardController.AddPackage);
            gameServer.router.PostRoutes.Add("/transactions/packages", PL.Controller.CardController.BuyPackage);
            
            gameServer.router.GetRoutes.Add("/cards", PL.Controller.CardController.ShowCards);

            // Show active deck:
            gameServer.router.GetRoutes.Add("/deck", PL.Controller.UserController.ShowActiveDeck);
            
            // Show all decks created by user:
            gameServer.router.GetRoutes.Add("/deck/all", PL.Controller.UserController.ShowAllDecks);
           
            // Add new deck, Content: Card_ids of Cards to add to deck
            gameServer.router.PostRoutes.Add("/deck/add", PL.Controller.UserController.AddNewDeck);
            // Set new active deck (Content: deck_id)
            gameServer.router.PutRoutes.Add("/deck", PL.Controller.UserController.SetActiveDeck);
            
            // Running Battles
            gameServer.router.GetRoutes.Add("/battle/find", PL.Controller.GameController.JoinBattle);

            // Battle Requests Battles
            gameServer.router.GetRoutes.Add("/battle/new", PL.Controller.GameController.NewBattle);
                       
            gameServer.router.GetRoutes.Add("/score", PL.Controller.ScoreboardController.ShowScoreboard);
            
            gameServer.router.GetRoutes.Add("/stats", PL.Controller.ScoreboardController.ShowStats);

            gameServer.Run();

        }
    }
}
