using NUnit.Framework;
using MonsterCardTradingGame.Model;
using System;

namespace MonsterCardTradingGame.Test
{
    public class TestBattle
    {
        User player1, player2;
        Battle battle;
        Deck deck1, deck2;

       [SetUp]
        public void Setup()
        {
            player1 = new User(Guid.NewGuid(), "player1", "password");
            player2 = new User(Guid.NewGuid(), "player2", "password");    
            battle = new Battle(Guid.NewGuid(), player1, player2);

            deck1 = new(Guid.NewGuid(), player1.UserId, "TestDeck1");
            deck2 = new(Guid.NewGuid(), player2.UserId, "TestDeck2");
        }


        [Test]
        public void testEvaluateCards_GoblinAginstDragon_WinnerIsDragon1()
        {
            Card dragon_winner = new Card(Guid.NewGuid(), "Dragon", "", 10, CardTypes.Monster, ElementTypes.Normal);
            Card dragon = new Card(Guid.NewGuid(), "Dragon", "", 5, CardTypes.Monster, ElementTypes.Fire);

            Guid winnerCard = battle.CallEvaluateCards(dragon, dragon_winner);
            Assert.AreEqual(dragon_winner.CardID, winnerCard);

            Guid winnerCard2 = battle.CallEvaluateCards(dragon_winner, dragon);
            Assert.AreEqual(dragon_winner.CardID, winnerCard2);
        }

        #region testSpellCards
        [Test]
        public void testEvaluateCards_WaterAgainstFire_WinnerIsWater()
        {
            Card waterspell_winner = new Card(Guid.NewGuid(), "WaterSpell", "", 5, CardTypes.Spell, ElementTypes.Water);
            Card firespell = new Card(Guid.NewGuid(), "FireSpell", "", 5, CardTypes.Spell, ElementTypes.Fire);

            Guid winnerCard = battle.CallEvaluateCards(waterspell_winner, firespell);
            Assert.AreEqual(waterspell_winner.CardID, winnerCard);

            Guid winnerCard2 = battle.CallEvaluateCards(firespell, waterspell_winner);
            Assert.AreEqual(waterspell_winner.CardID, winnerCard2);
        }

        [Test]
        public void testEvaluateCards_FireAgainstNormal_WinnerIsFire()
        {
            Card normalspell = new Card(Guid.NewGuid(), "WaterSpell", "", 5, CardTypes.Spell, ElementTypes.Normal);
            Card firespell_winner = new Card(Guid.NewGuid(), "FireSpell", "", 5, CardTypes.Spell, ElementTypes.Fire);

            Guid winnerCard = battle.CallEvaluateCards(firespell_winner, normalspell);
            Assert.AreEqual(firespell_winner.CardID, winnerCard);

            Guid winnerCard2 = battle.CallEvaluateCards(normalspell, firespell_winner);
            Assert.AreEqual(firespell_winner.CardID, winnerCard2);
        }

        [Test]
        public void testEvaluateCards_NoramlAgainstWater_WinnerIsNormal()
        {
            Card normalspell_winner = new Card(Guid.NewGuid(), "WaterSpell", "", 5, CardTypes.Spell, ElementTypes.Normal);
            Card waterspell = new Card(Guid.NewGuid(), "FireSpell", "", 5, CardTypes.Spell, ElementTypes.Water);

            Guid winnerCard = battle.CallEvaluateCards(normalspell_winner, waterspell);
            Assert.AreEqual(normalspell_winner.CardID, winnerCard);

            Guid winnerCard2 = battle.CallEvaluateCards(waterspell, normalspell_winner);
            Assert.AreEqual(normalspell_winner.CardID, winnerCard2);
        }

        // Test ineffective
       
        // effective


        #endregion
        #region testMonsterCards
        [Test]
        public void testEvaluateCards_GoblinAginstDragon_WinnerIsDragon()
        {
            Card goblin = new Card(Guid.NewGuid(), "Goblin", "", 5, CardTypes.Monster, ElementTypes.Normal);
            Card dragon_winner = new Card(Guid.NewGuid(), "Dragon", "", 5, CardTypes.Monster, ElementTypes.Fire);

            Guid winnerCard = battle.CallEvaluateCards(goblin, dragon_winner);
            Assert.AreEqual(dragon_winner.CardID, winnerCard);

            Guid winnerCard2 = battle.CallEvaluateCards(dragon_winner, goblin);
            Assert.AreEqual(dragon_winner.CardID, winnerCard2);
        }

        [Test]
        public void testEvaluateCards_WizardAgnainstOrcs_WinnerIsOrc()
        {
            Card wizard_winner = new Card(Guid.NewGuid(), "Wizard", "", 5, CardTypes.Monster, ElementTypes.Normal);
            Card orc = new Card(Guid.NewGuid(), "Orc", "", 5, CardTypes.Monster, ElementTypes.Normal);

            Guid winnerCard = battle.CallEvaluateCards(wizard_winner, orc);
            Assert.AreEqual(wizard_winner.CardID, winnerCard);

            Guid winnerCard2 = battle.CallEvaluateCards(orc, wizard_winner);
            Assert.AreEqual(wizard_winner.CardID, winnerCard);
        }

        [Test]
        public void testEvaluateCards_KnightAgainstWaterSpell_WinnerIsWaterSpell()
        {
            Card knight = new Card(Guid.NewGuid(), "Knight", "", 5, CardTypes.Monster, ElementTypes.Normal);
            Card waterSpell_winner = new Card(Guid.NewGuid(), "WaterSpell", "", 5, CardTypes.Spell, ElementTypes.Water);

            Guid winner = battle.CallEvaluateCards(knight, waterSpell_winner);
            Assert.AreEqual(waterSpell_winner.CardID, winner);

            Guid winner2 = battle.CallEvaluateCards(waterSpell_winner, knight);
            Assert.AreEqual(waterSpell_winner.CardID, winner2);
        }

        [Test]
        public void testEvaluateCards_KrakenAgainstAnySpell_WinnerIsKraken()
        {
            Card kraken_winner = new Card(Guid.NewGuid(), "Kraken", "", 5, CardTypes.Monster, ElementTypes.Normal);
            Card waterspell = new Card(Guid.NewGuid(), "Spell", "", 5, CardTypes.Spell, ElementTypes.Water);

            Guid winner = battle.CallEvaluateCards(kraken_winner, waterspell);
            Assert.AreEqual(kraken_winner.CardID, winner);
            Guid winner2 = battle.CallEvaluateCards(waterspell, kraken_winner);
            Assert.AreEqual(kraken_winner.CardID, winner2);

            Card firespell = new Card(Guid.NewGuid(), "Spell", "", 5, CardTypes.Spell, ElementTypes.Fire);
            Guid winner3 = battle.CallEvaluateCards(kraken_winner, firespell);
            Assert.AreEqual(kraken_winner.CardID, winner3);
            Guid winner4 = battle.CallEvaluateCards(firespell, kraken_winner);
            Assert.AreEqual(kraken_winner.CardID, winner4);

            Card normalspell = new Card(Guid.NewGuid(), "Spell", "", 5, CardTypes.Spell, ElementTypes.Fire);
            Guid winner5 = battle.CallEvaluateCards(kraken_winner, normalspell);
            Assert.AreEqual(kraken_winner.CardID, winner5);
            Guid winner6 = battle.CallEvaluateCards(normalspell, kraken_winner);
            Assert.AreEqual(kraken_winner.CardID, winner6);
        }

        [Test]
        public void testEvaluateCards_FireElvesAgainstDragon_WinnerIsFireElve()
        {
            Card fireElve_winner = new Card(Guid.NewGuid(), "Fire Elve", "", 5, CardTypes.Monster, ElementTypes.Fire);
            Card dragon = new Card(Guid.NewGuid(), "Dragon", "", 5, CardTypes.Monster, ElementTypes.Fire);

            Guid winner = battle.CallEvaluateCards(fireElve_winner, dragon);
            Assert.AreEqual(fireElve_winner.CardID, winner);
            Guid winner2 = battle.CallEvaluateCards(dragon, fireElve_winner);
            Assert.AreEqual(fireElve_winner.CardID, winner2);
        }
        #endregion
    }
}