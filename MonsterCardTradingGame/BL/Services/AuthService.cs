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
        * Validate Token 
        */
        public static bool AuthToken(HttpRequest req, out string username, out Guid userid)
        {
            // Default values for out paramaters
            username = "";
            userid = Guid.Empty;

            // Check if Header is set
            if (req.Headers.ContainsKey("Authorization"))
            {
                string token = req.Headers["Authorization"];

                // Check format:
                if (Regex.IsMatch(token, TOKEN_PATTERN) == true)
                {
                    // Split the token and check is user exists in db:
                    string[] tokenSplitBySpace = token.Split(' ');
                    string[] tokenSplitByDash = tokenSplitBySpace[1].Split('-');
                    string username_ = tokenSplitByDash[0];
                    Guid userid_;

                    try
                    {
                        userid_ = BL.Services.UserService.GetIdByUsername(username_);
                    }
                    catch
                    {
                        username = "";
                        userid = Guid.Empty;
                        Console.WriteLine($"[{DateTime.UtcNow}]\tAuthorization failed: User does not exist");
                        return false;
                    }

                    Console.WriteLine($"[{DateTime.UtcNow}]\tToken \"{token}\" is valid for \"{userid_}: {username_}\"");
                    username = username_;
                    userid = userid_;
                    return true;

                }
                Console.WriteLine($"[{DateTime.UtcNow}]\tAuthorization failed: Unknown token format");
            }

            Console.WriteLine($"[{DateTime.UtcNow}]\tAuthorization failed: Token not set");  
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
