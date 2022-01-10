using MonsterCardTradingGame.DAL.Repository;
using MonsterCardTradingGame.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterCardTradingGame.Exceptions;

namespace MonsterCardTradingGame.Test
{
    [TestFixture]
    class TestCardService
    {
        BL.Services.CardService cardService, cardService_dbError;
       
        [SetUp]
        public void Setup()
        {
            cardService = new BL.Services.CardService(new FakeCardRepos());
            cardService_dbError = new BL.Services.CardService(new FakeCardReposError());
        }

        #region testAddPackage
        [Test] 
        public void testAddPackage_notEnoughCards()
        {
            // Arrange   
            List<Card> addCards = new();
            addCards.Add(new Card(Guid.NewGuid(), "Goblin", "", 5, CardTypes.Monster, ElementTypes.Normal));
            addCards.Add(new Card(Guid.NewGuid(), "Goblin", "", 5, CardTypes.Monster, ElementTypes.Normal));

            // Act
            var ex = Assert.Throws<ServiceException>(() => cardService.AddPackage(addCards));

            // Act, Assert
            Assert.AreEqual("not enough cards", ex.Message);
        }
  
        [Test] 
        public void testAddPackage_moreThan5Cards()
        {
            // Arrange   
            List<Card> addCards = new();
            addCards.Add(new Card(Guid.NewGuid(), "Goblin", "", 5, CardTypes.Monster, ElementTypes.Normal));
            addCards.Add(new Card(Guid.NewGuid(), "Goblin", "", 5, CardTypes.Monster, ElementTypes.Normal));
            addCards.Add(new Card(Guid.NewGuid(), "Goblin", "", 5, CardTypes.Monster, ElementTypes.Normal));
            addCards.Add(new Card(Guid.NewGuid(), "Goblin", "", 5, CardTypes.Monster, ElementTypes.Normal));
            addCards.Add(new Card(Guid.NewGuid(), "Goblin", "", 5, CardTypes.Monster, ElementTypes.Normal));
            addCards.Add(new Card(Guid.NewGuid(), "Goblin", "", 5, CardTypes.Monster, ElementTypes.Normal));

            // Act
            var ex = Assert.Throws<ServiceException>(() => cardService.AddPackage(addCards));

            // Act, Assert
            Assert.AreEqual("too many cards", ex.Message);
        }


        /*
        [Test] 
        public void TestAddPackage_5Cards()
        {
            // Arrange   
            List<Card> addCards = new();
            addCards.Add(new Card(Guid.NewGuid(), "Goblin", "", 5, CardTypes.Monster, ElementTypes.Normal));
            addCards.Add(new Card(Guid.NewGuid(), "Goblin", "", 5, CardTypes.Monster, ElementTypes.Normal));
            addCards.Add(new Card(Guid.NewGuid(), "Goblin", "", 5, CardTypes.Monster, ElementTypes.Normal));
            addCards.Add(new Card(Guid.NewGuid(), "Goblin", "", 5, CardTypes.Monster, ElementTypes.Normal));
            addCards.Add(new Card(Guid.NewGuid(), "Goblin", "", 5, CardTypes.Monster, ElementTypes.Normal));

            // Act, Assert
            Assert.DoesNotThrow(() => cardService.AddPackage(addCards));
        }*/

        [Test] 
        public void testAddPackage_dbError()
        {
            // Arrange   
            List<Card> addCards = new();
            addCards.Add(new Card(Guid.NewGuid(), "Goblin", "", 5, CardTypes.Monster, ElementTypes.Normal));
            addCards.Add(new Card(Guid.NewGuid(), "Goblin", "", 5, CardTypes.Monster, ElementTypes.Normal));
            addCards.Add(new Card(Guid.NewGuid(), "Goblin", "", 5, CardTypes.Monster, ElementTypes.Normal));
            addCards.Add(new Card(Guid.NewGuid(), "Goblin", "", 5, CardTypes.Monster, ElementTypes.Normal));
            addCards.Add(new Card(Guid.NewGuid(), "Goblin", "", 5, CardTypes.Monster, ElementTypes.Normal));

            // Act
            var ex = Assert.Throws<ServiceException>(() => cardService_dbError.AddPackage(addCards));
        }

        #endregion
        #region testShowPackage

        [Test]
        public void testShowPackage_dbError()
        {
            // Act
            var ex = Assert.Throws<ServiceException>(() => cardService_dbError.ShowAllCards(Guid.NewGuid()));
        }

        #endregion
        #region testAquirePackage
        // not possible
        #endregion

        internal class FakeCardRepos : ICardRepository
        {
            public void CreateMultiple(List<Card> cards)
            {
                
            }

            public List<Card> GetAllByUser(Guid userid)
            {
                throw new NotImplementedException();
            }

            public List<Card> GetPackage(Guid userid)
            {
                throw new NotImplementedException();
            }
        }

        internal class FakeCardReposError : ICardRepository
        {
            // username, 
            Dictionary<string, Card> cards = new();

            public void CreateMultiple(List<Card> cards)
            {
                // do nothing
                throw new SystemException(); // some db error occured
            }

            public List<Card> GetAllByUser(Guid userid)
            {
                throw new SystemException(); // some db error occured
            }

            public List<Card> GetPackage(Guid userid)
            {
                throw new NotImplementedException();
            }
        }
    }
}
