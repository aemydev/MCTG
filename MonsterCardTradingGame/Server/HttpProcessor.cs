using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Server
{
    /*
     * Properly parse the HTTP header 
     */
    class HttpProcessor
    {
        private TcpClient socket;
        private HttpServer httpServer;

        public string Method { get; private set; } // GET, POST 
        public string Path { get; private set; } // bzw. our CMD
        public string Version { get; private set; }
        public string Content { get; private set; }

        public Dictionary<string, string> Headers { get; } // Header e.g. Content-Type

        public HttpProcessor(TcpClient sock, HttpServer httpServer)
        {
            this.socket = sock;
            this.httpServer = httpServer;

            Method = null;
            Headers = new();
        }

        /*
         *  Parse the full HTTP-request
         */
        public void Process()
        {
            StreamWriter writer = new StreamWriter(socket.GetStream()) { AutoFlush = true };
            NetworkStream reader = socket.GetStream();
            //var reader = new StreamReader(socket.GetStream());

            /*
             *  Parse the Header
             */
            string line = null;
            while ((line = ReadLine(reader)) != null)
            {
                Console.WriteLine(line); // Debug

                if (line.Length == 0)
                {
                    break; // End of header reached
                }

                if (Method == null) // handle first line of HTTP
                {
                    var parts = line.Split(' ');
                    Method = parts[0];
                    Path = parts[1];
                    Version = parts[2];
                }
                else // handle HTTP headers
                {
                    var parts = line.Split(": ");
                    Headers.Add(parts[0], parts[1]);
                }
            }


            /*
             *  Parse the Content (only POST)
             */
            if (Headers.ContainsKey("Content-Length"))
            {
                int totalBytes = Convert.ToInt32(Headers["Content-Length"]);

                byte[] data = new BinaryReader(reader).ReadBytes(totalBytes);

                Content = Encoding.UTF8.GetString(data);
                Console.WriteLine(Content);
            }


            /*
             *   Respond according to the Path & Method -> eigene Klasse, Funktion ??
             */
            if (Method == "POST" && Path == "/users")
            {
                // Task ?
                var user = JsonConvert.DeserializeObject<Model.Credentials>(Content);

                // Generate User & Save to DB
                if (Controller.UserController.Register(user))
                {
                    // If successfull, send Response with 201

                    // Generate HttpResponse
                    string responseContent = "<html><body><h1>User successfully registered</h1></body></html>";
                    Console.WriteLine();
                    WriteLine(writer, "HTTP/1.1 200 OK"); //
                    WriteLine(writer, $"Server: {httpServer.serverName}");
                    WriteLine(writer, $"Current Time: {DateTime.Now}");
                    WriteLine(writer, $"Content-Length: {responseContent.Length}");
                    WriteLine(writer, "Content-Type: text/html; charset=utf-8");
                    WriteLine(writer, "");
                    WriteLine(writer, responseContent);
                    writer.WriteLine();
                    writer.Flush();
                    writer.Close();
                }
                else
                {
                    // send correct error eode Response
                    // Generate HttpResponse
                    string responseContent = "<html><body><h1>User registration failed.</h1></body></html>";
                    Console.WriteLine();
                    WriteLine(writer, "HTTP/1.1 409 CONFLICT");
                    WriteLine(writer, $"Server: {httpServer.serverName}");
                    WriteLine(writer, $"Current Time: {DateTime.Now}");
                    WriteLine(writer, $"Content-Length: {responseContent.Length}");
                    WriteLine(writer, "Content-Type: text/html; charset=utf-8");
                    WriteLine(writer, "");
                    WriteLine(writer, responseContent);
                    writer.WriteLine();
                    writer.Flush();
                    writer.Close();
                }
            }
            else
            {
                string responseContent = "<html><body><h1>Error 404 - Not implemented</h1></body></html>";
                Console.WriteLine();
                WriteLine(writer, "HTTP/1.1 404 Not found");
                WriteLine(writer, $"Server: {httpServer.serverName}");
                WriteLine(writer, $"Current Time: {DateTime.Now}");
                WriteLine(writer, $"Content-Length: {responseContent.Length}");
                WriteLine(writer, "Content-Type: text/html; charset=utf-8");
                WriteLine(writer, "");
                WriteLine(writer, responseContent);
                writer.WriteLine();
                writer.Flush();
                writer.Close();
            }
        }

        /*
         *  Write to Stream & Console
         */
        private void WriteLine(StreamWriter writer, string s)
        {
            Console.WriteLine(s);
            writer.WriteLine(s);
        }

        // Change? Are there other options?
        private string ReadLine(Stream stream)
        {
            int nextChar;
            string data = "";
            while (true)
            {
                nextChar = stream.ReadByte();
                if (nextChar == '\n') { break; }
                if (nextChar == '\r') { continue; }
                if (nextChar == -1) { continue; }
                data += Convert.ToChar(nextChar);
            }
            return data;
        }
    }
}
