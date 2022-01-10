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
    public class AuthService
    {
        private const string TOKEN_PATTERN = "(Basic ([A-Z]|[a-z])\\w+-mtcgToken)";
        private static UserService UserService = new UserService();

        /*
        *  Validate Token 
        */
        public static bool AuthToken(Dictionary<string, string> headers, out string username, out Guid userid)
        {
            // Default values for out paramaters
            username = "";
            userid = Guid.Empty;

            // Check if Header is set
            if (headers.ContainsKey("Authorization"))
            {
                string token = headers["Authorization"];

                // Check format:
                if (Regex.IsMatch(token, TOKEN_PATTERN) == true)
                {
                    // Split the token and check is user exists in db:
                    string[] tokenSplitBySpace = token.Split(' ');
                    string[] tokenSplitByDash = tokenSplitBySpace[1].Split('-');
                    string usernameToken = tokenSplitByDash[0];
                    
                    if(IsRegistered(usernameToken, out Guid userid_)){
                        Console.WriteLine($"[{DateTime.UtcNow}]\tToken \"{token}\" is valid for \"{userid_}: {usernameToken}\"");
                        username = usernameToken;
                        userid = userid_;
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"[{DateTime.UtcNow}]\tAuthorization failed: User does not exist");
                        return false;
                    }

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

        private static bool IsRegistered(string username, out Guid userid)
        {
            if(UserService.GetIdByUsername(username, out userid))
            {
                return true;
            }

            return false;
        }
    }
}
