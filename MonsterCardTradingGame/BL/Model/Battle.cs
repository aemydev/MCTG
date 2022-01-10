using MonsterCardTradingGame.BL.Services;
using MonsterCardTradingGame.Exceptions;
using MonsterCardTradingGame.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace MonsterCardTradingGame.Model
{
    public enum BattleStates { Running, Done }
    public enum Player { player1, player2, draw }
    // Producer
    public enum WinLose { Winner, Loser, Draw}

    public class Battle
    {
        private UserService UserService = new UserService();

        public Guid Id { get; set; }
        public BattleStates Status { get; set; }
        public User Player1 { get; private set; }
        public User Player2 { get; private set; }
        public string Winner { get; private set; }
        public List<String> Log { get; private set; }
        private const int MAX_ROUNDS = 100;

        // Events:
        public event EventHandler<OnBattleEndArgs> OnBattleEnd;
        public class OnBattleEndArgs : EventArgs
        {
            public string winner;
            public Guid battleid;
            public bool scoreboardentry = false;
        }

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
            Status = BattleStates.Running;
            Console.WriteLine($"[{DateTime.UtcNow}]\tStart new battle ({Id}): {Player1.Username} vs. {Player2.Username}");

            // Get the active decks
            if ((!UserService.GetActiveDeck((Guid)Player1.ActiveDeckId, out Deck gameDeck1) ||
                (!UserService.GetActiveDeck((Guid)Player2.ActiveDeckId, out Deck gameDeck2))))
            {
                throw new GameException("could not get deck");
            }

            // Game-Loop
            Winner = GameLoop(gameDeck1, gameDeck2);
            Console.WriteLine($"[{DateTime.UtcNow}]\tBatte ended. The winner is {Winner}!");

            // Transfer the Cards from loser -> winner, Persist in DB
            // add cards from losing player to stack of other player?

            // Calc Stats
            int eloPlayer1 = 0, eloPlayer2 = 0;

            if (Winner == Player1.Username)
            {
                eloPlayer1 = CalcElo(WinLose.Winner);
                eloPlayer2 = CalcElo(WinLose.Loser);
            }
            else if(Winner == Player2.Username)
            {
                eloPlayer1 = CalcElo(WinLose.Loser);
                eloPlayer2 = CalcElo(WinLose.Winner);
            }
            else
            {
                eloPlayer1 = CalcElo(WinLose.Draw);
                eloPlayer2 = CalcElo(WinLose.Draw);
            }

            bool scoreboardEntry_ = true;
            try
            {
                BL.Services.ScoreboardService.AddEntry(new ScoreboardEntry(Player1.UserId, eloPlayer1));
                BL.Services.ScoreboardService.AddEntry(new ScoreboardEntry(Player2.UserId, eloPlayer2));
            }catch(System.Exception e)
            {
                Console.WriteLine(e.Message);
                scoreboardEntry_ = false;
            }
           
            OnBattleEnd?.Invoke(this, new OnBattleEndArgs { winner = Winner, battleid = Id, scoreboardentry = scoreboardEntry_ });
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

        private string GameLoop(Deck gameDeck1, Deck gameDeck2)
        {
            // Local variables:
            bool battleEnd = false;
            int currentRound = 1;
            string winnerString = "";

            while (!battleEnd)
            {
                // Get 2 random cards
                Card p1card = SelectRandomCard(gameDeck1.Cards);
                Card p2card = SelectRandomCard(gameDeck2.Cards);

                Console.WriteLine($"[{DateTime.UtcNow}, Id: {Id}]\tRound {currentRound}: {Player1.Username}: \"{ p1card.Title}, Damage: {p1card.Damage}\" vs. {Player2.Username}: \"{ p2card.Title}, Damage: { p2card.Damage}\"");

                Guid winnerCard = EvaluateCards(p1card, p2card);

                // Exchange the Cards
                if(winnerCard == p1card.CardID)
                {
                    // Player1 gets card from player 2
                    Console.WriteLine($"[{DateTime.UtcNow}, {Id}]\t\"{Player1.Username}\" wins round {currentRound}");
                    Console.WriteLine($"[{DateTime.UtcNow}, {Id}]\t\"{p2card.Title},{ p2card.Description}, { p2card.Damage}\" has been added to {Player1.Username}'s deck");

                    gameDeck1.Cards.Add(p2card);
                    gameDeck2.Cards.Remove(p2card);

                }else if(winnerCard == p2card.CardID)
                {
                    // player2 gets card of player1
                    Console.WriteLine($"[{DateTime.UtcNow}, BattleId {Id}]\t{Player2.Username} wins round {currentRound}");
                    Console.WriteLine($"[{DateTime.UtcNow}, {Id}]\t\"{p1card.Title},{ p1card.Description}, { p1card.Damage}\" has been added to {Player2.Username}'s deck");
                    gameDeck2.Cards.Add(p1card);
                    gameDeck1.Cards.Remove(p1card);
                }
                else
                {
                    // nothing happens
                    Console.WriteLine($"[{DateTime.UtcNow}, {Id}]\tNo cards exchanged.");
                }

                // Is Game End?
                if (gameDeck1.Cards.Count == 0)
                {
                    battleEnd = true;
                    winnerString= Player2.Username;
                }
                else if (gameDeck2.Cards.Count == 0)
                {
                    battleEnd = true;
                    winnerString = Player1.Username;
                }
                else if (currentRound == MAX_ROUNDS)
                {
                    battleEnd = true;
                    winnerString = "Draw";
                }
                else
                {
                    currentRound++;
                }
            }

            return winnerString;
        }

        private Guid EvaluateCards(Card p1card, Card p2card)
        {
            int p1Damage = p1card.Damage, p2Damage = p2card.Damage;

            if ((p1card.Type == CardTypes.Monster) && (p2card.Type == CardTypes.Monster))
            {
                if ((p1card.Title == "Goblin") && (p2card.Title == "Dragon"))
                {
                    // Dragon wins
                    p1Damage = 0;
                    p2Damage = p2card.Damage;
                }
                else if ((p1card.Title == "Dragon") && (p2card.Title == "Goblin"))
                {
                    // Dragon wins
                    p1Damage = p1card.Damage;
                    p2Damage = 0;
                }
                else if (p1card.Title.Equals("Wizard") && p2card.Title.Equals("Orc"))
                {
                    p1Damage = p1card.Damage;
                    p2Damage = 0;
                }
                else if (p1card.Title == "Orc" && p2card.Title == "Wizard")
                {
                    p1Damage = 0;
                    p2Damage = p2card.Damage;
                }
                else if (p1card.Title == "Fire Elve" && p2card.Title == "Dragon")
                {
                    p1Damage = p1card.Damage;
                    p2Damage = 0;
                }
                else if (p1card.Title == "Dragon" && p2card.Title == "Fire Elve")
                {
                    p1Damage = 0;
                    p2Damage = p2card.Damage;
                }
                else
                {
                    p1Damage = p1card.Damage;
                    p2Damage = p2card.Damage;
                }
            }
            else if (p1card.Type is CardTypes.Spell && p2card.Type is CardTypes.Monster)
            {
                if (p2card.Title == "Kraken")
                {
                    p1Damage = 0;
                    p2Damage = p2card.Damage;
                }
                else if (p2card.Title == "Knight")
                {
                    p1Damage = p1card.Damage;
                    p2Damage = 0;
                }
                else
                {
                    if ((p1card.ElementType is ElementTypes.Water && p2card.ElementType is ElementTypes.Fire) ||
                       (p1card.ElementType is ElementTypes.Fire && p2card.ElementType is ElementTypes.Normal) ||
                       (p1card.ElementType is ElementTypes.Normal && p2card.ElementType is ElementTypes.Water))
                    {
                        // effective -> double damage
                        p1Damage = p1card.Damage * 2;
                        p2Damage = p2card.Damage;

                    }
                    else if ((p1card.ElementType is ElementTypes.Fire && p2card.ElementType is ElementTypes.Water) ||
                             (p1card.ElementType is ElementTypes.Normal && p2card.ElementType is ElementTypes.Fire) ||
                             (p1card.ElementType is ElementTypes.Water && p2card.ElementType is ElementTypes.Normal))
                    {
                        p1Damage = p1card.Damage / 2;
                        p2Damage = p2card.Damage;
                    }
                    else
                    {
                        p1Damage = p1card.Damage;
                        p2Damage = p2card.Damage;
                    }
                }
            }
            else if (p1card.Type is CardTypes.Monster && p2card.Type is CardTypes.Spell)
            {
                if (p1card.Title == "Kraken")
                {
                    p1Damage = p1card.Damage;
                    p2Damage = 0;
                }
                else if (p1card.Title == "Knight")
                {
                    p1Damage = 0;
                    p2Damage = p1card.Damage;
                }
                else
                {
                    if ((p2card.ElementType is ElementTypes.Water && p1card.ElementType is ElementTypes.Fire) ||
                       (p2card.ElementType is ElementTypes.Fire && p1card.ElementType is ElementTypes.Normal) ||
                       (p2card.ElementType is ElementTypes.Normal && p1card.ElementType is ElementTypes.Water))
                    {
                        // effective -> double damage
                        p1Damage = p1card.Damage;
                        p2Damage = p2card.Damage * 2;

                    }
                    else if ((p2card.ElementType is ElementTypes.Fire && p1card.ElementType is ElementTypes.Water) ||
                             (p2card.ElementType is ElementTypes.Normal && p1card.ElementType is ElementTypes.Fire) ||
                             (p2card.ElementType is ElementTypes.Water && p1card.ElementType is ElementTypes.Normal))
                    {
                        p1Damage = p1card.Damage;
                        p2Damage = p2card.Damage / 2;
                    }
                    else
                    {
                        p1Damage = p1card.Damage;
                        p2Damage = p2card.Damage;
                    }
                }
            }
            else
            {
                if ((p1card.ElementType is ElementTypes.Water && p2card.ElementType is ElementTypes.Fire) ||
                      (p1card.ElementType is ElementTypes.Fire && p2card.ElementType is ElementTypes.Normal) ||
                      (p1card.ElementType is ElementTypes.Normal && p2card.ElementType is ElementTypes.Water))
                {
                    // effective -> double damage
                    p1Damage = p1card.Damage * 2;
                    p2Damage = p2card.Damage;

                }
                else if ((p1card.ElementType is ElementTypes.Fire && p2card.ElementType is ElementTypes.Water) ||
                         (p1card.ElementType is ElementTypes.Normal && p2card.ElementType is ElementTypes.Fire) ||
                         (p1card.ElementType is ElementTypes.Water && p2card.ElementType is ElementTypes.Normal))
                {
                    p1Damage = p1card.Damage / 2;
                    p2Damage = p2card.Damage;
                }
                else if ((p2card.ElementType is ElementTypes.Water && p1card.ElementType is ElementTypes.Fire) ||
                         (p2card.ElementType is ElementTypes.Fire && p1card.ElementType is ElementTypes.Normal) ||
                         (p2card.ElementType is ElementTypes.Normal && p1card.ElementType is ElementTypes.Water))
                {
                    // effective -> double damage
                    p1Damage = p1card.Damage;
                    p2Damage = p2card.Damage * 2;

                }
                else if ((p2card.ElementType is ElementTypes.Fire && p1card.ElementType is ElementTypes.Water) ||
                         (p2card.ElementType is ElementTypes.Normal && p1card.ElementType is ElementTypes.Fire) ||
                         (p2card.ElementType is ElementTypes.Water && p1card.ElementType is ElementTypes.Normal))
                {
                    p1Damage = p1card.Damage;
                    p2Damage = p2card.Damage / 2;
                }
                else
                {
                    p1Damage = p1card.Damage;
                    p2Damage = p2card.Damage;
                }
            }

            // Fight 
            if (p1Damage > p2Damage)
            {
                return p1card.CardID;       
            }else if(p1Damage == p2Damage)
            {
                return Guid.Empty;
            }
            else
            {
                return p2card.CardID;
            }


        }
        
        private int CalcElo(WinLose winOrLose)
        {
            if(winOrLose == WinLose.Winner)
            {
                return 3;
            }
            else if(winOrLose == WinLose.Loser)
            {
                return -5;
            }
            else
            {
                return 1;
            }
        }

        /*
         *  For Testing:
         */
        public Guid CallEvaluateCards(Card p1card, Card p2card)
        {
            return EvaluateCards(p1card, p2card);
        }
        
        public int CallCalculateElo(WinLose winLose)
        {
            return CalcElo(winLose);
        }
    }
}
