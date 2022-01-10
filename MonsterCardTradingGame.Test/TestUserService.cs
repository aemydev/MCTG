using MonsterCardTradingGame.DAL.Repository;
using MonsterCardTradingGame.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterCardTradingGame.Exceptions;
using MonsterCardTradingGame.BL.Services;
using MonsterCardTradingGame.Utility.Json;

namespace MonsterCardTradingGame.Test
{
    [TestFixture]
    class TestUserService
    {
        UserService userService, userService_dbError;
       
        [SetUp]
        public void Setup()
        {
            userService = new UserService(new FakeUserRepos(), new FakeDeckRepository());
            userService_dbError = new UserService(new FakeUserReposError(), new FakeDeckRepositoryError());
        }

        #region testRegister

        [Test] 
        public void testRegister_success()
        {
            // Arrange
            CredentialsJson jsonCred = new();
            jsonCred.Username = "testuser";
            jsonCred.Password = "secret";

            // Act
            Assert.DoesNotThrow(() => userService.Register(jsonCred));
        }

        [Test]
        public void testRegister_usernameTooLong()
        {
            // Arrange
            CredentialsJson jsonCred = new();
            jsonCred.Username = "testusertestusertestusertestusertestuser";
            jsonCred.Password = "secret";
            
            // Act
            var ex = Assert.Throws<ServiceException>(() => userService.Register(jsonCred));

            // Assert
            Assert.AreEqual("invalid username", ex.Message);
        }

        [Test]
        public void testRegister_dberror()
        {
            // Arrange
            CredentialsJson jsonCred = new();
            jsonCred.Username = "testuser";
            jsonCred.Password = "secret";
            
            // Act, Assert
            var ex = Assert.Throws<ServiceException>(() => userService_dbError.Register(jsonCred));
        }

        #endregion
        #region testLogin
        
        [Test]
        public void testLogin_success()
        {
            // Arrange
            CredentialsJson jsonCred = new();
            jsonCred.Username = "ines";
            jsonCred.Password = "asdf123";

            // Act
            string token = userService.Login(jsonCred);

            Assert.AreEqual("Basic ines-mtcgToken", token);
        }

        [Test]
        public void testLogin_failed_wrongPassword()
        {
            // Arrange
            CredentialsJson jsonCred = new();
            jsonCred.Username = "ines";
            jsonCred.Password = "secret";

            // Act
            var ex = Assert.Throws<ServiceException>(() => userService.Login(jsonCred));

            // Assert
            Assert.AreEqual("login failed", ex.Message);
        }

        [Test]
        public void testLogin_failed_noUser()
        {
            // Arrange
            CredentialsJson jsonCred = new();
            jsonCred.Username = "andi";
            jsonCred.Password = "secret";

            // Act
            var ex = Assert.Throws<ServiceException>(() => userService.Login(jsonCred));

            // Assert
            Assert.AreEqual("user not found", ex.Message);
        }

        #endregion
        #region testGetUserByName

        [Test]
        public void testGetUserByName_success()
        {
            // Arrange
            string username = "ines";

            // Act
            bool res = userService.GetUserByUsername(username, out User user);
            
            // Assert
            Assert.AreEqual(true, res);
            Assert.AreEqual(username, user.Username);

        }

        [Test]
        public void testGetUserByName_notfound()
        {
            // Arrange
            string username = "waldo";

            // Act
            bool res = userService.GetUserByUsername(username, out _);

            // Assert
            Assert.AreEqual(false, res);
        }

        #endregion
        #region testGetIdByName

        [Test]
        public void testGetIdByName_success()
        {
            // Arrange
            string username = "ines";

            // Act
            bool res = userService.GetIdByUsername(username, out Guid userid);

            // Assert
            Assert.AreEqual(true, res);
            Assert.AreEqual(Guid.Parse("c71a111f-491a-4762-a455-f3cab15cf599"), userid);

        }

        [Test]
        public void testGetIdByName_notfound()
        {
            // Arrange
            string username = "waldo";

            // Act
            bool res = userService.GetIdByUsername(username, out _);

            // Assert
            Assert.AreEqual(false, res);
        }

        #endregion
        #region testGetActiveDeck
        
        [Test]
        public void testGetDeckById_success()
        {
            // Arrange
            Guid userid = Guid.Parse("c71a111f-491a-4762-a455-f3cab15cf599");

            // Act
            bool res = userService.GetActiveDeck(userid, out Deck deck);

            // Assert
            Assert.AreEqual(true, res);
        }


        [Test]
        public void testGetDeckById_failed()
        {
            // Arrange
            Guid userid = Guid.Parse("c71a111f-4562-4762-a455-f3cab15cf599");

            // Act
            bool res = userService.GetActiveDeck(userid, out Deck deck);

            // Assert
            Assert.AreEqual(false, res);
        }

        #endregion

        #region stubs
        internal class FakeUserRepos : IUserRepository
        {
            Dictionary<string, User> userTable = new();
            public FakeUserRepos()
            {
                userTable.Add("ines", new User(Guid.Parse("c71a111f-491a-4762-a455-f3cab15cf599"), "ines", "2d/uaEK5UWyiHnaMbZHyvtD/lqrHqD2Lm/I/qpxMEScL2mh/s+gMnoKZVg36MJClVfjTU+9X8HR4FZWyXN33rA=="));
            }
            public void Create(Credentials cred)
            {
                // do nothing
            }

            public void Delete(User user)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<User> GetAll()
            {
                throw new NotImplementedException();
            }

            public User GetById(Guid id)
            {
                throw new NotImplementedException();
            }

            public User GetByName(string username)
            {
                if(userTable.TryGetValue(username, out User user))
                {
                    return user;
                }
                else
                {
                    throw new RepositoryException("user not found");
                }
            }

            public Guid GetIdByUsername(string username)
            {
                if (userTable.TryGetValue(username, out User user))
                {
                    return user.UserId;
                }
                else
                {
                    throw new RepositoryException("user not found");
                }
            }

            public void Update(User user)
            {
            }

            public void UpdateDeck(Guid userid, Guid deckid)
            {
            }
        }

        internal class FakeUserReposError : IUserRepository
        {
            public void Create(Credentials cred)
            {
                throw new System.Exception();
            }

            public void Delete(User user)
            {
                throw new System.Exception();
            }

            public IEnumerable<User> GetAll()
            {
                throw new System.Exception();
            }

            public User GetById(Guid id)
            {
                throw new System.Exception();
            }

            public User GetByName(string username)
            {
                throw new RepositoryException("db error");
            }

            public Guid GetIdByUsername(string username)
            {
                throw new System.Exception();
            }

            public void Update(User user)
            {
                throw new System.Exception();
            }

            public void UpdateDeck(Guid userid, Guid deckid)
            {
                throw new System.Exception();
            }
        }

        internal class FakeDeckRepository : IDeckRepository
        {
            Dictionary<Guid, Deck> deckTable = new();
            
            public FakeDeckRepository()
            {
                deckTable.Add(Guid.Parse("c71a111f-491a-4762-a455-f3cab15cf599"), new Deck(Guid.NewGuid(), Guid.Parse("c71a111f-491a-4762-a455-f3cab15cf599"), "test"));
                deckTable[Guid.Parse("c71a111f-491a-4762-a455-f3cab15cf599")].Cards.Add(new Card(Guid.NewGuid(), "Goblin", "", 5, CardTypes.Monster, ElementTypes.Normal));
                deckTable[Guid.Parse("c71a111f-491a-4762-a455-f3cab15cf599")].Cards.Add(new Card(Guid.NewGuid(), "Goblin", "", 5, CardTypes.Monster, ElementTypes.Normal));
                deckTable[Guid.Parse("c71a111f-491a-4762-a455-f3cab15cf599")].Cards.Add(new Card(Guid.NewGuid(), "Goblin", "", 5, CardTypes.Monster, ElementTypes.Normal));
                deckTable[Guid.Parse("c71a111f-491a-4762-a455-f3cab15cf599")].Cards.Add(new Card(Guid.NewGuid(), "Goblin", "", 5, CardTypes.Monster, ElementTypes.Normal));
                deckTable[Guid.Parse("c71a111f-491a-4762-a455-f3cab15cf599")].Cards.Add(new Card(Guid.NewGuid(), "Goblin", "", 5, CardTypes.Monster, ElementTypes.Normal));
            }
            
            public void AddDeck(DeckJson deck, Guid owner)
            {
                throw new NotImplementedException();
            }

            public List<Deck> GetAll(Guid userid)
            {
                throw new NotImplementedException();
            }

            public Deck GetDeckById(Guid id)
            {
                if(deckTable.TryGetValue(id, out Deck deck))
                {
                    return deck;
                }
                else
                {
                    throw new RepositoryException("db error");
                }
            }
        }

        internal class FakeDeckRepositoryError : IDeckRepository
        {
            public void AddDeck(DeckJson deck, Guid owner)
            {
                throw new NotImplementedException();
            }

            public List<Deck> GetAll(Guid userid)
            {
                throw new NotImplementedException();
            }

            public Deck GetDeckById(Guid id)
            {
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}
