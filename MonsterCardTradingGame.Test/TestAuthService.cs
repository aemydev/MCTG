using NUnit.Framework;
using MonsterCardTradingGame.Model;
using MonsterCardTradingGame.BL.Services;

using System;
using Moq;
using MonsterCardTradingGame.Server;
using System.Collections.Generic;

namespace MonsterCardTradingGame.Test
{
    public class TestAuthService
    {
       [SetUp]
        public void Setup()
        {
            string username = "ines";
            Mock<DAL.Repository.IUserRepository> mockUserRepos = new Mock<DAL.Repository.IUserRepository>();
            mockUserRepos.Setup(t => t.GetIdByUsername(username)).Returns(Guid.NewGuid());
        }

        [Test]
        public void testAuthToken_noToken()
        {
            // Arrange
            Dictionary<string, string> header = new();                

            // Act
            bool invalid = AuthService.AuthToken(header, out _, out _);
            
            // Assert
            Assert.AreEqual(false, invalid); 
        }

        [Test]
        public void testAuthToken_invalidToken()
        {
            // Arrange
            Dictionary<string, string> header = new();
            
            header.Add("Authorization", "invalid");

            bool invalid = AuthService.AuthToken(header, out _, out _);

            // Assert
            Assert.AreEqual(false, invalid);
        }

        /*
        [Test]
        public void testAuthToken_validToken()
        {
            // Arrange
            Dictionary<string, string> header = new();

            header.Add("Authorization", "Basic asdf-mtcgToken");

            bool valid = AuthService.AuthToken(header, out _, out _);

            // Assert
            Assert.AreEqual(true, valid);
        }  */   
    }
}