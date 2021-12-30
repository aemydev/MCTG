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
     * Properly parse the HTTP header & content
     */
    class HttpProcessor
    {
        private TcpClient socket;
        private HttpServer httpServer;

        public HttpProcessor(TcpClient sock, HttpServer httpServer)
        {
            this.socket = sock;
            this.httpServer = httpServer;
        }

        /*
         *  Parse the full HTTP-request
         */
        public void Process()
        {
            StreamWriter writer = new StreamWriter(socket.GetStream()) { AutoFlush = true };
            NetworkStream reader = socket.GetStream();

            /*
             *  Parse the Request
             */
            HttpRequest httpRequest = ParseHeader(reader);

            // For POST only: Parse Content
            if (httpRequest.Method.Equals("POST"))
            {
                if (httpRequest.Headers.ContainsKey("Content-Length"))
                {
                    httpRequest.Content = ParseContent(reader, httpRequest.Headers["Content-Length"]);
                }
                else
                {
                    // Error Handling
                    // POST must contain content
                    HttpResponse httpResponse = new HttpResponse(HttpStatusCode.BadRequest);
                    httpResponse.AddContent("application/json", "{\"response\":\"Internal Server Error\"}");
                    httpResponse.Send(writer, httpServer.serverName);
                }
            }

            /*
             *  Endpoints
             */
            if (httpServer.router.GetRoutes.ContainsKey(httpRequest.Path) || httpServer.router.PostRoutes.ContainsKey(httpRequest.Path))
            {
                HttpResponse res;

                if (httpRequest.Method == "POST")
                {
                    res = httpServer.router.invoke(httpRequest.Method, httpRequest.Path, httpRequest.Content);
                }
                else
                {
                    res = httpServer.router.invoke(httpRequest.Method, httpRequest.Path);
                }

                res.Send(writer, httpServer.serverName);
            }
            else
            {
                // Error 404 - Not found
                HttpResponse httpResponse = new HttpResponse(HttpStatusCode.NotFound);
                httpResponse.AddContent("application/json", "{\"response\":\"Error 404 - not found\"}");
                httpResponse.Send(writer, httpServer.serverName);
            }
        }


        /*
         *  Parse the Header
         */
        private HttpRequest ParseHeader(NetworkStream reader)
        {
            string method = null, path = null, version = null;
            Dictionary<string, string> headers = new();

            string line = null;

            while ((line = ReadLine(reader)) != null)
            {

                if (line.Length == 0)
                {
                    break; // End of header reached
                }

                if (method == null) // handle first line of HTTP
                {
                    var parts = line.Split(' ');
                    method = parts[0];
                    path = parts[1];
                    version = parts[2];
                }
                else // handle HTTP headers
                {
                    var parts = line.Split(": ");
                    headers.Add(parts[0], parts[1]);
                }
            }

            return new HttpRequest(method, path, version, headers);
        }

        /*
         *  Parse the Content, only POST
         */
        private string ParseContent(NetworkStream reader, string contentLength)
        {
           int totalBytes = Convert.ToInt32(contentLength);

           byte[] data = new BinaryReader(reader).ReadBytes(totalBytes);

           string Content = Encoding.UTF8.GetString(data);

           return Content; 
        }

        /*
         *  Own Readline 
         */
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
