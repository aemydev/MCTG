using System;

namespace MonsterCardTradingGame.Model
{
    public class User
    {
        public Guid UserId { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public int Coins { get; set; }
        public Guid? ActiveDeckId { get; private set; }
        public Model.Card[] Deck { get; set; }

        public User(Guid userid, string name, string password, int coins = 20, Guid? deckid = null)
        {
            this.UserId = userid;
            this.Username = name;
            this.Password = password;
            this.Coins = coins;
            this.ActiveDeckId = deckid;
        }
    }
}
