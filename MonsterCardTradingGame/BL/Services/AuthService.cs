using MonsterCardTradingGame.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MonsterCardTradingGame.Exceptions;

namespace MonsterCardTradingGame.BL.Services
{
    class AuthService
    {
        private const string TOKEN_PATTERN = "(Basic ([A-Z]|[a-z])\\w+-mtcgToken)";

        /*
         * Validate Token 
         */
        public static bool Auth(HttpRequest req)
        {
            // Check if Header is set
            if (req.Headers.ContainsKey("Authorization"))
            {
                string token = req.Headers["Authorization"];

                // Check if format is correct
                if (Regex.IsMatch(token, TOKEN_PATTERN) == true)
                {
                    string[] tokenSplit = token.Split(' ');
                    string[] tokenSplit2 = tokenSplit[1].Split('-');

                    Console.WriteLine($"[{DateTime.UtcNow}]\tToken \"{token}\" valid");
                    return true;

                }
            }

            Console.WriteLine($"[{DateTime.UtcNow}]\tToken invalid");
            return false;
        }
        
        /*
        *  Check if token is "admin-Token"
        */
        public static bool AuthAdmin(HttpRequest req)
        {
            // Check if there is a token
            if (req.Headers.ContainsKey("Authorization"))
            {
                string token = req.Headers["Authorization"];

                // Check if format is correct
                if (Regex.IsMatch(token, TOKEN_PATTERN) == true)
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

        /*
         *  Split up Token to get Username
         */
        public static string getUserNameFromToken(string token)
        {
            string[] tokenSplit = token.Split(' '); //[Basic, name-mtcgToken]
            string[] tokenSplit2 = tokenSplit[1].Split('-');// [name, mtcgToken]

            return tokenSplit2[0];
        }
    }
}
