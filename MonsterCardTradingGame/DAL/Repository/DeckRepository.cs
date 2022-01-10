using MonsterCardTradingGame.Exceptions;
using MonsterCardTradingGame.Model;
using Npgsql;
using System;
using System.Collections.Generic;

namespace MonsterCardTradingGame.DAL.Repository
{
    public class DeckRepository : IDeckRepository
    {
        Postgres.PostgresAccess db = Postgres.PostgresAccess.Instance;
        private const string TABLE_NAME = "deck";
        private const string TABLE_NAME_REL = "cards_in_deck";

        /*
         *  Create
         */
        public void AddDeck(Utility.Json.DeckJson deck, Guid owner)
        {
            Guid deckid = deck.Id == null ? Guid.NewGuid() : Guid.Parse(deck.Id);
            Console.WriteLine("HEre");
            Console.WriteLine(deckid);
            try
            {
                using (var transaction = db.GetConnection().BeginTransaction())
                {
                    // Create new Deck
                    using (var command = db.GetConnection().CreateCommand())
                    {
                        string sql = $"INSERT INTO {TABLE_NAME} (deck_id, owner, title) VALUES (@deck_id, @owner, @title)";
                        Console.WriteLine($"[{DateTime.UtcNow}]\tExecute SQL-Statement: {sql}");
                        command.CommandText = sql;
                        command.Parameters.AddWithValue($"@deck_id", deckid.ToString());
                        command.Parameters.AddWithValue($"@owner", owner.ToString());
                        command.Parameters.AddWithValue($"@title", deck.Title);
                        int affectedRows = command.ExecuteNonQuery();
                        Console.WriteLine($"[{DateTime.UtcNow}]\tAffected rows: {affectedRows}");
                    }

                    // Check if Cards are owned by user ???

                    // Add Cards
                    foreach (var card in deck.Cards)
                    {
                        using (var command = db.GetConnection().CreateCommand())
                        {
                            string sql = $"INSERT INTO {TABLE_NAME_REL} (deck_id, card_id) VALUES (@deck_id, @card_id)";
                            Console.WriteLine($"[{DateTime.UtcNow}]\tExecute SQL-Statement: {sql}");
                            command.CommandText = sql;
                            command.Parameters.AddWithValue($"@deck_id", deckid);
                            command.Parameters.AddWithValue($"@card_id", card);
                            int affectedRows = command.ExecuteNonQuery();
                            Console.WriteLine($"[{DateTime.UtcNow}]\tAffected rows: {affectedRows}");
                        }
                    }

                    transaction.Commit();
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"[{DateTime.UtcNow}]\tError creating new Deck: {e.Message}");
                throw new RepositoryException("Error");
            }
        }

        /*
         *  Read
         */
        public Deck GetDeckById(Guid id)
        {
            Deck deck;
            try
            {
                using (var transaction = db.GetConnection().BeginTransaction())
                {
                    // Get details about deck
                    using (var command = db.GetConnection().CreateCommand())
                    {
                        string sql = $"SELECT deck_id, owner, title  FROM {TABLE_NAME} WHERE deck_id=@deck_id;";
                        Console.WriteLine($"[{DateTime.UtcNow}]\tExecute SQL-Statement: {sql}");
                        command.CommandText = sql;
                        command.Parameters.AddWithValue("@deck_id", id.ToString());

                        using NpgsqlDataReader reader = command.ExecuteReader();

                        string deck_id, owner_, title;

                        reader.Read();
                        deck_id = reader.GetString(0);
                        owner_ = reader.GetString(1);
                        title = reader.GetString(2);

                        deck = new Deck(Guid.Parse(deck_id), Guid.Parse(owner_), title);
                    }

                    // Get all cards in deck
                    using (var command = db.GetConnection().CreateCommand())
                    {
                        string sql = $"SELECT c.card_id, title, description, damage, owner, card_type, element_type FROM cards_in_deck cd JOIN card c ON cd.card_id = c.card_id WHERE cd.deck_id=@deck_id;";
                        //Console.WriteLine($"[{DateTime.UtcNow}]\tExecute SQL-Statement: {sql}");
                        command.CommandText = sql;
                        command.Parameters.AddWithValue("@deck_id", id.ToString());

                        using NpgsqlDataReader reader = command.ExecuteReader();

                        string _cardid, _title, _description, _owner;
                        int _damage, _cardType, _elementType;

                        while (reader.Read())
                        {
                            _cardid = reader.GetString(0);
                            _title = reader.GetString(1);
                            _description = reader.GetString(2);
                            _damage = reader.GetInt32(3);
                            _owner = reader.GetString(4);
                            _cardType = reader.GetInt32(5);
                            _elementType = reader.GetInt32(6);

                            deck.Cards.Add(new Card(Guid.Parse(_cardid), _title, _description, _damage, (CardTypes)_cardType, (ElementTypes)_elementType, Guid.Parse(_owner)));
                        }
                    }

                    transaction.Commit();
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"[{DateTime.UtcNow}]\t Error getting Deck, {e.Message}");
                throw new RepositoryException("db error");
            }

            return deck;
        }

        /*
         *  Get all decks for specific user
         */
        public List<Deck> GetAll(Guid userid)
        {
            List<Deck> decks = new();
            try
            {
                using (var transaction = db.GetConnection().BeginTransaction())
                {
                    // Get details about deck
                    using (var command = db.GetConnection().CreateCommand())
                    {
                        string sql = $"SELECT deck_id, owner, title  FROM {TABLE_NAME} WHERE owner=@owner;";
                        command.CommandText = sql;
                        command.Parameters.AddWithValue("@owner", userid.ToString());

                        using NpgsqlDataReader reader = command.ExecuteReader();

                        string deck_id, owner_, title;

                        while (reader.Read())
                        {
                            deck_id = reader.GetString(0);
                            owner_ = reader.GetString(1);
                            title = reader.GetString(2);
                            decks.Add(new Deck(Guid.Parse(deck_id), Guid.Parse(owner_), title));
                        }
                    }

                    // Get all cards in deck
                    using (var command = db.GetConnection().CreateCommand())
                    {
                        string sql = $"SELECT * FROM cards_in_deck cd JOIN card c ON cd.card_id = c.card_id WHERE cd.deck_id=@deck_id;";
                        //Console.WriteLine($"[{DateTime.UtcNow}]\tExecute SQL-Statement: {sql}");

                        command.CommandText = sql;
                        command.Parameters.AddWithValue("@deck_id", userid.ToString());

                        using NpgsqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            string _deckid = reader.GetString(0);
                            string _owner = reader.GetString(1);
                            string _title = reader.GetString(2);

                            decks.Add(new Deck(Guid.Parse(_deckid), Guid.Parse(_owner), _title));
                        }
                    }

                    // Get Cards for each deck
                    foreach (Deck deck in decks)
                    {
                        using (var command = db.GetConnection().CreateCommand())
                        {
                            string sql = $"SELECT c.card_id, title, description, damage, owner, card_type, element_type FROM cards_in_deck cd JOIN card c ON cd.card_id = c.card_id WHERE cd.deck_id=@deck_id;";
                            //Console.WriteLine($"[{DateTime.UtcNow}]\tExecute SQL-Statement: {sql}");
                            command.CommandText = sql;
                            command.Parameters.AddWithValue("@deck_id", deck.DeckId.ToString());

                            using NpgsqlDataReader reader = command.ExecuteReader();

                            while (reader.Read())
                            {
                                string _cardid = reader.GetString(0);
                                string _title = reader.GetString(1);
                                string _description = reader.GetString(2);
                                int _damage = reader.GetInt32(3);
                                string _owner = reader.GetString(4);
                                int _cardType = reader.GetInt32(5);
                                int _elementType = reader.GetInt32(6);

                                deck.Cards.Add(new Card(Guid.Parse(_cardid), _title, _description, _damage, (CardTypes)_cardType, (ElementTypes)_elementType, Guid.Parse(_owner)));
                            }
                        }
                    }

                    transaction.Commit();
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"[{DateTime.UtcNow}]\t Could not get decks for user: {e.Message}");
                throw new RepositoryException("Error");
            }

            return decks;
        }
    }
}
