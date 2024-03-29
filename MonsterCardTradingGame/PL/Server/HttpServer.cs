﻿using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MonsterCardTradingGame.Server
{
    public class HttpServer
    {
        protected int port; // Default: 8080
        private IPAddress ip; // Default: IPAddress.Loopback
        public string serverName = "MonsterCardTradingGameServer";
        public Server.Router.Router router = new Server.Router.Router();
        public ConcurrentQueue<Model.Battle> games = new();

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

            Console.WriteLine($"[{DateTime.UtcNow}]\tServer started. Waiting for connections...");

            while (true)
            {
                TcpClient sock = listener.AcceptTcpClient();

                HttpProcessor processor = new HttpProcessor(sock, this);

                new Thread(processor.Process).Start();

                Thread.Sleep(1);
            }
        }
    }
}
