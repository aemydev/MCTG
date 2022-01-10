using System;

namespace MonsterCardTradingGame.Model
{
    public enum RequestState { Pending, Accepted };

    class BattleRequest
    {
        public User Player1 { get; init; }
        public User Player2 { get; set; }
        public RequestState RequestState { get; private set; }

        // Event
        public event EventHandler<OnAcceptedArgs> OnAccepted;
        public class OnAcceptedArgs : EventArgs
        {
            public Guid battleId;
        }

        public BattleRequest(User player1)
        {
            Player1 = player1;
        }

        public Guid Accept(User player2)
        {
            Player2 = player2;
            RequestState = RequestState.Accepted;

            Guid battleId = Guid.NewGuid();
            Program.battles.TryAdd(battleId, new Battle(battleId, Player1, Player2));

            OnAccepted?.Invoke(this, new OnAcceptedArgs { battleId = battleId });
            return battleId;
        }
    }
}
