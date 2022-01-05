using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Server
{
    class HttpServer
    {
        protected int port; // Default: 8080
        private IPAddress ip; // Default: IPAddress.Loopback
        public string serverName = "MonsterCardTradingGameServer";
        public Server.Router.Router router = new Server.Router.Router(); 

        private TcpListener listener;

        public HttpServer(int port, string ip = "")
        {
            this.port = port;
            this.ip = ip == "" ? IPAddress.Any : IPAddress.Parse(ip);
        }

        public void Run()
        {
            listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start(5); // Queue of 5

            Console.WriteLine("Server running...");

            while (true)
            {
                TcpClient sock = listener.AcceptTcpClient();

                HttpProcessor processor = new HttpProcessor(sock, this);

                new Thread(processor.Process).Start(); // Tasks?

                Thread.Sleep(1);
            }
        }
    }
}
