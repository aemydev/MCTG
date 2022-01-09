using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MonsterCardTradingGame.Exceptions;
using MonsterCardTradingGame.Model;

namespace MonsterCardTradingGame.BL.Services
{
    class UserService
    {
        static DAL.Repository.IUserRepository userrepos = new DAL.Respository.UserRepository();
        static DAL.Repository.IDeckRepository deckrepos = new DAL.Repository.DeckRepository();

        /*
         *  Register a new User
         */
        public static bool Register(Utility.Json.CredentialsJson cred)
        {
            // Validate username
            // 
            // Hash the Password
            cred.Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                cred.Password,
                Encoding.UTF8.GetBytes("js83$0jolsod/"),
                KeyDerivationPrf.HMACSHA512,
                10,
                64)
            );

            // Add to Player Table
            try
            {
                userrepos.Create(new Model.Credentials(cred.Username, cred.Password));
                return true;
            }
            catch(RepositoryException e) when (e.Message == "User already exists")
            {
                throw new HttpException("409 Conflict");
            }
            catch (System.Exception) {
                throw new HttpException("500 Interal Server Error");
            }           
        }

        /*
         *  Login User, returns Token if login successful, otherwise Exception is thrown
         */
        public static string Login(Utility.Json.CredentialsJson cred)
        {
            string passwordFromDB;
            try
            {
                Model.User user = userrepos.GetByName(cred.Username);
                passwordFromDB = user.Password;
            }
            catch(RepositoryException e) when(e.Message == "User does not exist")
            {
                throw new HttpException("404 Not Found"); 
            }catch
            {
                throw new HttpException("500 Internal Server Error");
            }

            // Hash the pw the user entered:
            cred.Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                cred.Password,
                Encoding.UTF8.GetBytes("js83$0jolsod/"), // Salt Value
                KeyDerivationPrf.HMACSHA512,
                10,
                64));

            // Check if pw match up:
            if (passwordFromDB == cred.Password)
            {
                return generateToken(cred.Username);
            }
            else
            {
                throw new HttpException("401 Unauthorized");
            }
        }

        /*
         *  Get user via username
         */
        public static Model.User GetUserByUsername(string username)
        {
            try
            {
                return userrepos.GetByName(username);
            }
            catch
            {
                throw new HttpException("not found");
            }
        }
        
        /*
        *  Get userid via username
        */
        public static Guid GetIdByUsername(string username)
        {
            try
            {
                return userrepos.GetIdByUsername(username);
            }
            catch
            {
                throw new HttpException("not found");
            }
        }

        #region deck
        /*
         *  Get active deck per deck_id
         */
        public static Deck GetActiveDeck(Guid deck_id)
        {
            
            
            
            try
            {
                return deckrepos.GetDeckById(deck_id);
            }
            catch
            {
                throw; // Something went wrong
            }
        }

        public static Deck GetActiveDeck(string username)
        {
            User user;
            try
            {
                user = userrepos.GetByName(username);
            }
            catch
            {
                throw;
            }

            if(user.ActiveDeckId.ToString() == "")
            {
                throw new HttpException("no deck set");
            }

            try
            {
                return deckrepos.GetDeckById((Guid)user.ActiveDeckId);
            }
            catch
            {
                throw; // Something went wrong
            }
        }

        /*
         *  Show all decks of user, returns List<Deck> decks
         */
        public static List<Deck> GetAllDecks(Guid userid)
        {
            try
            {
                return deckrepos.GetAll(userid);
            }
            catch
            {
                throw;
            }
            
            
            throw new NotImplementedException();
        }

        /*
        *  Set active deck of user to deck_id
        */
        public static void SetActiveDeck(Guid userid, Guid deck_id)
        {
            // Set active deck in player_tablet
            try
            {
                userrepos.UpdateDeck(userid, deck_id);
            }
            catch { 
                throw; // Update failed
            }
        }

        /*
        *  Add new deck to decks-table
        */
        public static void AddNewDeck(Utility.Json.DeckJson deck, Guid owner)
        {
            Console.WriteLine($"[{DateTime.UtcNow}]\tCreate new Deck \"{deck.Title}\" ");

            // Validate, only 5 cards per deck
            if(deck.Cards.Length != 5)
            {
                throw new HttpException("invalid card count");
            }

            try
            {
                deckrepos.AddDeck(deck, owner);
            }
            catch
            {
                throw new HttpException("not found");
            }
        }
        
        #endregion
        #region helper
        /*
         *  Helper
         */
        private static string generateToken(string username)
        {
            string token = $"Basic {username}-mctgToken";
            return token;
        }
        #endregion
    }
}
