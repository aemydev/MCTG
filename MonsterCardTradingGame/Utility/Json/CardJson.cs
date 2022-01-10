namespace MonsterCardTradingGame.Utility.Json
{
    class CardJson
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Damage { get; set; }
        public Model.CardTypes Type { get; set; }
        public Model.ElementTypes ElementType { get; set; }
        public string Description { get; set; }
    }
}
