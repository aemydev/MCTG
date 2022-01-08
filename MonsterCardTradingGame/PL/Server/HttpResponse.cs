using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Server
{
    public class HttpResponse
    {
        public string StatusCode { get; set; }
        public string Version { get; private set; }
        public string ContentType { get; set; }
        public string Content { get; set; } // only post
        public Dictionary<string, string> Headers { get; set; }
        private Dictionary<HttpStatusCode, string> StatusCodeString;

        public HttpResponse(HttpStatusCode statusCode)
        {
            initStatusCodes(); 
            this.StatusCode = StatusCodeString[statusCode];
            this.Version = "1.1"; // -> we only use Http Version 1.1
            this.Content = null;
            Headers = new();
        }  

        void initStatusCodes()
        {
            StatusCodeString = new();
            StatusCodeString.Add(HttpStatusCode.OK, "200");
            StatusCodeString.Add(HttpStatusCode.Created, "201");
            StatusCodeString.Add(HttpStatusCode.Conflict, "409");
            StatusCodeString.Add(HttpStatusCode.NotFound, "404");
            StatusCodeString.Add(HttpStatusCode.Forbidden, "403");
            StatusCodeString.Add(HttpStatusCode.InternalServerError, "500");
            StatusCodeString.Add(HttpStatusCode.Unauthorized, "401");
        }

        public void addHeader(string key, string value)
        {
            Headers.Add(key, value);
        }
       
        // Set Content-Type and Content of Response:
        public void AddContent(string contentType, string content)
        {
            ContentType = contentType;
            Content = content;
        }

        public void Send(StreamWriter writer, string serverName)
        {
            /* 
            HTTP/Version Response-Code (Status)
            Klartext-Meldung (Reason)
            General Header
            Response Header
            Entity Header (optional)
            Leerzeile (!)
            Resource Entity (falls vorhanden)
            */

            Console.WriteLine($"[{DateTime.UtcNow}]\tSend HTTP-Response, Statuscode: {StatusCode}");

            // If there is no content -> Content-Length = 0
            string responseContent = Content == null ? "0" : Content;

            // HTTP/Version Response-Code (Status)
            WriteLine(writer, $"HTTP/{Version} {StatusCode}"); // !!

            // Klartext - Meldung(Reason)
            WriteLine(writer, $"Current Time: {DateTime.Now}");

            // General Header
            if (Headers.Count() > 0)
            {
                foreach (KeyValuePair<string, string> header_ in Headers)
                {
                    WriteLine(writer, $"{header_.Key}: {header_.Value}");
                }
            }

            // Response Header -> Header mit weiteren Informationen zur
            // Antwort, wie etwa ihres Orts oder den Server selbst (Name und Version etc.)
            WriteLine(writer, $"Server: {serverName}"); // !!

            // Entity Header(optional) -> Infos about Body, if it exists
            WriteLine(writer, $"Content-Length: {responseContent.Length}");
            WriteLine(writer, $"Content-Type: {ContentType}");

            // Leerzeile(!)
            WriteLine(writer, ""); 

            WriteLine(writer, responseContent); // only if set
            
            writer.WriteLine();
            writer.Flush();
            writer.Close();
        }

        /*
        *  Write to Stream & Console
        */
        private void WriteLine(StreamWriter writer, string s)
        {
            //Console.WriteLine(s);
            writer.WriteLine(s);
        }
    }
}
