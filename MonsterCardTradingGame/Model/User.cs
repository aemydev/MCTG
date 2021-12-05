using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Model
{
    public class User
    {
        public int UserId { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public int Coins { get; set; }
        public string Token { get; private set; }

        public User(string name, string password, string token = "", int userid = 0, int coins = 20)
        {
            this.UserId = userid;
            this.Username = name;
            this.Password = password;
            this.Coins = coins;
            this.Token = token;
        }
    }
}
