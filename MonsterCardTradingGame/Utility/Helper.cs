using MonsterCardTradingGame.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace MonsterCardTradingGame.Utility
{
    class Helper
    {
        private const string TOKEN_PATTERN = "(Basic ([A-Z]|[a-z])\\w+-mtcgToken)";
        
        public static bool Auth(HttpRequest req)
        {
            // Check if Header is set
            if (req.Headers.ContainsKey("Authorization"))
            {
                // Check if Token is valid
                // Split String Leerzeichen

                // Split String -


            }
            return false;
        }

        public static bool AuthAdmin(HttpRequest req)
        {
            // Check if there is a token
            if (req.Headers.ContainsKey("Authorization"))
            {
                string token = req.Headers["Authorization"];

                // Check if format is correct
                if(Regex.IsMatch(token, TOKEN_PATTERN) == true)
                {
                    string[] tokenSplit = token.Split(' ');
                    string[] tokenSplit2 = tokenSplit[1].Split('-');

                    Console.WriteLine(tokenSplit2[0]);

                    //check if user has admin rights
                    if (tokenSplit2[0].Trim() == "admin")
                    {
                        return true;
                    }
                }   
            }
            return false;
        }
    }
}
