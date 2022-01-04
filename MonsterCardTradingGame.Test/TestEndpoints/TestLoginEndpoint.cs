using NUnit.Framework;

namespace MonsterCardTradingGame.Test
{
    public class TestLoginEndpoint
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Login_correctCredentials()
        {
            Assert.Pass();
        }

        [Test]
        public void Login_wrongPassword()
        {
            Assert.Pass();
        }

        [Test]
        public void Login_userDoesNotExist()
        {
            Assert.Pass();
        }
    }
}