using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MonsterCardTradingGame.DAL.Repository;
using MonsterCardTradingGame.Exceptions;
using MonsterCardTradingGame.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterCardTradingGame.BL.Services
{
    public class UserService
    {
        public IUserRepository Userrepos;
        public IDeckRepository Deckrepos;
        private const string SALT_VALUE = "js83$0jolsod/";

        /*
         *  Constructor
         */
        public UserService()
        {
            Userrepos = new UserRepository();
            Deckrepos = new DeckRepository();
        }

        public UserService(IUserRepository userrepos, IDeckRepository deckrepos)
        {
            Userrepos = userrepos;
            Deckrepos = deckrepos;
        }

        /*
         *  Register a new User
         */
        public bool Register(Utility.Json.CredentialsJson cred)
        {
            // Validate username
            if (!ValidateUsername(cred.Username))
            {
                throw new ServiceException("invalid username");
            }

            Console.WriteLine($"[{DateTime.UtcNow}]\tRegister new user \"{cred.Username}\"");

            // Hash the Password
            cred.Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                cred.Password,
                Encoding.UTF8.GetBytes(SALT_VALUE),
                KeyDerivationPrf.HMACSHA512,
                10,
                64)
            );

            // Add to Player Table
            try
            {
                Userrepos.Create(new Credentials(cred.Username, cred.Password));
                return true;
            }
            catch (RepositoryException e) when (e.Message == "User already exists")
            {
                throw new ServiceException("user already exists");
            }
            catch
            {
                throw new ServiceException("db error");
            }
        }

        /*
         *  Login User, returns Token if login successful, otherwise Exception is thrown
         */
        public string Login(Utility.Json.CredentialsJson cred)
        {
            Console.WriteLine($"[{DateTime.UtcNow}]\tLogin \"{cred.Username}\"");

            string passwordFromDB;
            try
            {
                User user = Userrepos.GetByName(cred.Username);
                passwordFromDB = user.Password;
            }
            catch (RepositoryException e) when (e.Message == "user not found")
            {
                throw new ServiceException("user not found");
            }
            catch
            {
                throw new ServiceException("db error");
            }


            // Hash the pw the user entered:
            cred.Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                cred.Password,
                Encoding.UTF8.GetBytes(SALT_VALUE), // Salt Value
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
                throw new ServiceException("login failed");
            }
        }

        /*
         *  Get user via username, out
         */
        public bool GetUserByUsername(string username, out User user)
        {
            try
            {
                user = Userrepos.GetByName(username);
                return true;
            }
            catch
            {
                user = null;
                return false;
            }
        }

        /*
        *  Get userid via username
        */
        public bool GetIdByUsername(string username, out Guid userid)
        {
            try
            {
                userid = Userrepos.GetIdByUsername(username);
                return true;
            }
            catch
            {
                userid = Guid.Empty;
                return false;
            }
        }

        #region deck

        /*
         *  Get active deck per deck_id
         */
        public bool GetActiveDeck(Guid deck_id, out Deck deck)
        {
            try
            {
                deck = Deckrepos.GetDeckById(deck_id);
                return true;
            }
            catch
            {
                deck = null;
                return false;
            }
        }

        /*
         *  Show all decks of user, returns List<Deck> decks
         */
        public List<Deck> GetAllDecks(Guid userid)
        {
            try
            {
                return Deckrepos.GetAll(userid);
            }
            catch
            {
                throw;
            }
        }

        /*
        *  Set active deck of user to deck_id
        */
        public void SetActiveDeck(Guid userid, Guid deck_id)
        {
            // Set active deck in player_tablet
            try
            {
                Userrepos.UpdateDeck(userid, deck_id);
            }
            catch
            {
                throw; 
            }
        }

        /*
        *  Add new deck to decks-table
        */
        public void AddNewDeck(Utility.Json.DeckJson deck, Guid owner)
        {
            Console.WriteLine($"[{DateTime.UtcNow}]\tCreate new Deck \"{deck.Title}\" ");

            // Validate, only 5 cards per deck
            if (deck.Cards.Length != 5)
            {
                throw new ServiceException("invalid card count");
            }
            Console.WriteLine("Another here");
            try
            {
                Deckrepos.AddDeck(deck, owner);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                throw new ServiceException("db error");
            }
        }

        #endregion
        #region helper

        /*
         *  Helper
         */
        private static string generateToken(string username)
        {
            string token = $"Basic {username}-mtcgToken";
            return token;
        }

        private static bool ValidateUsername(string username)
        {
            if (username.Length > 20)
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}
