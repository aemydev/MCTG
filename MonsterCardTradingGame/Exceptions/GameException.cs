using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.Exceptions
{
    class GameException : Exception
    {
        public GameException()
        {
        }

        public GameException(string message)
            : base(message)
        {
        }

        public GameException(string message, Exception inner)
               : base(message, inner)
        {
        }
    }
}
