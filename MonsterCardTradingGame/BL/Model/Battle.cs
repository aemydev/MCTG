using MonsterCardTradingGame.Exceptions;
using MonsterCardTradingGame.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace MonsterCardTradingGame.Model
{
    public enum BattleStates { Running, Done }

    // Producer

    class Battle
    {
        public Guid Id { get; set; }
        public BattleStates Status { get;  set; }
        public User Player1 { get; private set; }
        public User Player2 { get; private set; }
        public string Winner { get; private set; }

        // Private
        private const int MAX_ROUNDS = 10;

        // Events:
        public event EventHandler OnBattleStart;
        public event EventHandler<OnBattleEndArgs> OnBattleEnd;
        public class OnBattleEndArgs : EventArgs
        {
            public string winner;
            public Guid battleid;
        }
        //AutoResetEvent, ManualResetEvent, WaitHandler 

        /*
         *  Constructor
         */
        public Battle(Guid id, User player1, User player2)
        {
            Id = id;
            Player1 = player1;
            Player2 = player2;
        }

        /*
         *  GameLogic
         */
        public void Start()
        {
            OnBattleStart?.Invoke(this, EventArgs.Empty);
            Status = BattleStates.Running;

            // Local variables:
            bool battleEnd = false;
            int currentRound = 1;

            Console.WriteLine($"[{DateTime.UtcNow}]\tStart new battle ({Id}): {Player1.Username} vs. {Player2.Username}");
            Status = BattleStates.Running;
           
            // Get the decks
            Deck gameDeck1 = BL.Services.UserService.GetActiveDeck((Guid)Player1.ActiveDeckId);
            Deck gameDeck2 = BL.Services.UserService.GetActiveDeck((Guid)Player2.ActiveDeckId);

            // Game-Loop
            while(!battleEnd && currentRound <= MAX_ROUNDS)
            {
                OnBattleStart?.Invoke(this, EventArgs.Empty);

                // do the battle magic
                Console.WriteLine("Fight");
                battleEnd = true;






                currentRound++;
            }

            // Determine the winner:
            Winner = Player1.Username;

            Console.WriteLine($"[{DateTime.UtcNow}]\tBatte ended. The winner is {Winner}!");

            // Transfer the Cards from loser -> winner


            
            OnBattleEnd?.Invoke(this, new OnBattleEndArgs { winner = Winner , battleid=Id});
        }

        /*
         *  Helper
         */
        private Card SelectRandomCard(List<Card> cards)
        {
            Random rand = new Random();
            int randIndex = rand.Next(cards.Count);
            return cards[randIndex];
        }

    }
}
