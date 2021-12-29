using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Server
{
    class HttpResponse
    {
        public string StatusCode { get; set; }
        public string Method { get; private set; } // GET, POST 
        public string Version { get; private set; }
        public string Content { get; set; } // only post
        public Dictionary<string, string> Headers { get; set; }
        private Dictionary<HttpStatusCode, string> StatusCodeString;

        public HttpResponse(HttpStatusCode statusCode, string content = null)
        {
            initStatusCodes();
            this.StatusCode = StatusCodeString[statusCode];
            this.Version = "1.1";
            this.Content = content;
        }  

        // bessere Lösung?
        void initStatusCodes()
        {
            StatusCodeString = new();
            StatusCodeString.Add(HttpStatusCode.OK, "200 OK");
            StatusCodeString.Add(HttpStatusCode.Created, "201 CREATED");
            StatusCodeString.Add(HttpStatusCode.Conflict, "409 CONFLICT");
            StatusCodeString.Add(HttpStatusCode.NotFound, "404 NOT FOUND");
        }

        /*
        void addHeader(string key, string value)
        {
            Headers.Add(key, value);
        }
        */

        public void AddContent(string content)
        {
            Content = content;
        }

        public void Send(StreamWriter writer, string serverName)
        {
            string responseContent = Content == null ? "0" : Content;

            WriteLine(writer, $"HTTP/{Version} {StatusCode}"); 
            WriteLine(writer, $"Server: {serverName}");
            WriteLine(writer, $"Current Time: {DateTime.Now}");
            WriteLine(writer, $"Content-Length: {responseContent.Length}");
            WriteLine(writer, "Content-Type: text/html; charset=utf-8");
            WriteLine(writer, ""); // Leerzeile, dann Content
            WriteLine(writer, responseContent);
            writer.WriteLine();
            writer.Flush();
            writer.Close();
        }

        /*
        *  Write to Stream & Console
        */
        private void WriteLine(StreamWriter writer, string s)
        {
            Console.WriteLine(s);
            writer.WriteLine(s);
        }
    }
}
