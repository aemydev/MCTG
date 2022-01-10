using MonsterCardTradingGame.Exceptions;
using MonsterCardTradingGame.Model;
using Npgsql;
using System;
using System.Collections.Generic;

namespace MonsterCardTradingGame.DAL.Repository
{
    class CardRepository : ICardRepository
    {
        Postgres.PostgresAccess db = Postgres.PostgresAccess.Instance;
        private const string TABLE_NAME = "card";


        /*
         *  Create
         */
        /* public void Create(Card card)
         {
             Console.WriteLine($"[{DateTime.UtcNow}]\tCreate new card \"{card.Title}, {card.Description},{card.Damage}\"");

             string sql = $"INSERT INTO {TABLE_NAME} (card_id, title, description, damage) VALUES (@card_id, @title, @description, @damage)";
             try
             {
                 using (var command = db.GetConnection().CreateCommand())
                 {
                     command.CommandText = sql;
                     command.Parameters.AddWithValue($"@card_id", card.CardID);
                     command.Parameters.AddWithValue($"@title", card.Title);
                     command.Parameters.AddWithValue($"@description", card.Description);
                     command.Parameters.AddWithValue($"@damage", card.Damage);
                     command.ExecuteNonQuery();
                 }
             }
             catch (PostgresException e) when (e.SqlState == "23505") // Card already exists
             {
                 Console.WriteLine($"[{DateTime.UtcNow}]\tCard already exists. \"{card.Title}, {card.Description},{card.Damage}");
                 throw new RepositoryException("Card already exists");
             }
             catch (System.Exception e)
             {
                 Console.WriteLine($"[{DateTime.UtcNow}]\tError creating card \"{card.Title}, {card.Description},{card.Damage}\"");
                 Console.WriteLine(e.Message);
                 throw new RepositoryException("Error");
             }
         }
        */
        public void CreateMultiple(List<Card> cards)
        {
            //Console.WriteLine($"[{DateTime.UtcNow}]\tCreate new Package");
            try
            {
                using (var transaction = db.GetConnection().BeginTransaction())
                {
                    string sql = $"INSERT INTO {TABLE_NAME} (card_id, title, description, damage, element_type, card_type) VALUES (@card_id, @title, @description, @damage, @element_type, @card_type)";

                    foreach (var card in cards)
                    {
                        Console.WriteLine($"[{DateTime.UtcNow}]\tCreate new card \"{card.Title}, {card.Description},{card.Damage}\"");

                        using (var command = db.GetConnection().CreateCommand())
                        {
                            command.CommandText = sql;
                            command.Parameters.AddWithValue($"@card_id", card.CardID);
                            command.Parameters.AddWithValue($"@title", card.Title);
                            command.Parameters.AddWithValue($"@description", card.Description);
                            command.Parameters.AddWithValue($"@damage", card.Damage);
                            command.Parameters.AddWithValue($"@element_type", (int)card.ElementType);
                            command.Parameters.AddWithValue($"@card_type", (int)card.Type);

                            command.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[{DateTime.UtcNow}]\t Error creating new Package, {e.Message}");
                throw new RepositoryException("Error");
            }
        }


        /*
         *  Read
         */
        public List<Card> GetAllByUser(Guid id)
        {
            try
            {
                using (var command = db.GetConnection().CreateCommand())
                {
                    string sql = $"SELECT card_id, title, description, damage, element_type, card_type  FROM {TABLE_NAME} WHERE owner=@owner;";
                    command.CommandText = sql;
                    command.Parameters.AddWithValue("@owner", id.ToString());

                    using NpgsqlDataReader reader = command.ExecuteReader();

                    string cardID, title, description;
                    int damage, elementType, cardType;

                    List<Card> cards = new();

                    while (reader.Read())
                    {
                        cardID = reader.GetString(0);
                        title = reader.GetString(1);
                        description = reader.GetString(2);
                        damage = reader.GetInt32(3);
                        cardType = reader.GetInt32(4);
                        elementType = reader.GetInt32(5);

                        cards.Add(new Model.Card(Guid.Parse(cardID), title, description, damage, (CardTypes)cardType, (ElementTypes)elementType));

                    }

                    return cards;
                }
            }
            catch
            {
                //Console.WriteLine(e.Message);
                throw; // Better Error Handeling
            }
        }

        /* public void GetById(Guid id)
         {
             string sql = $"SELECT * FROM {TABLE_NAME} WHERE owner is null;";
             try
             {
                 using (var command = db.GetConnection().CreateCommand())
                 {
                     command.CommandText = sql;
                     var entry = command.ExecuteScalar();

                     List<Card> cards = new();

                     Console.WriteLine(entry);
                 }
             }
             catch (System.Exception e)
             {
                 Console.WriteLine(e.Message);
                 throw; // Better Error Handeling
             }
         }*/


        /*
         *  Update
         */
        /*
        public void UpdateOwner(Guid cardid, Guid userid)
        {
            string sql = $"UPDATE {TABLE_NAME} SET owner = @owner WHERE card_id=@card_id;";
            try
            {
                using (var command = db.GetConnection().CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.AddWithValue("@owner", userid);
                    command.Parameters.AddWithValue($"@card_id", cardid);
                    command.ExecuteNonQuery();
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                throw; // Better Error Handeling
            }
        }*/
        public List<Card> GetPackage(Guid userid)
        {
            // Transaction, Get 5 Cards without owner, update owner
            List<Card> cards = new();

            try
            {
                using (var transaction = db.GetConnection().BeginTransaction())
                {
                    // Select 5 Cards without owner
                    using (var command = db.GetConnection().CreateCommand())
                    {
                        string sql = $"SELECT card_id, title, description, damage, element_type, card_type FROM {TABLE_NAME} WHERE owner is null LIMIT 5;";
                        command.CommandText = sql;

                        using NpgsqlDataReader reader = command.ExecuteReader();
                        string cardID, title, description;
                        int damage, elementType, cardType;

                        while (reader.Read())
                        {
                            cardID = reader.GetString(0);
                            title = reader.GetString(1);
                            description = reader.GetString(2);
                            damage = reader.GetInt32(3);
                            cardType = reader.GetInt32(5);
                            elementType = reader.GetInt32(4);

                            cards.Add(new Card(Guid.Parse(cardID), title, description, damage, (CardTypes)cardType, (ElementTypes)elementType));
                        }
                    }

                    if (cards.Count != 5)
                    {
                        transaction.Rollback("not enough cards");
                    }

                    // Update owner for 5 cards
                    foreach (Card card in cards)
                    {
                        using (var command = db.GetConnection().CreateCommand())
                        {
                            string sql = $"UPDATE {TABLE_NAME} SET owner = @owner WHERE card_id=@card_id;";

                            command.CommandText = sql;
                            command.Parameters.AddWithValue("@owner", userid);
                            command.Parameters.AddWithValue($"@card_id", card.CardID.ToString());
                            command.ExecuteNonQuery();
                        }
                    }

                    // User -5 coins
                    using (var command = db.GetConnection().CreateCommand())
                    {
                        string sql = $"UPDATE player SET coins=coins-5 WHERE user_id=@user_id;";
                        command.CommandText = sql;
                        command.Parameters.AddWithValue("@user_id", userid.ToString());
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"[{DateTime.UtcNow}]\t Error aquiring new Package, {e.Message}");
                throw new RepositoryException("Error");
            }

            // Set owner in return list of cards
            foreach (var card in cards)
            {
                card.OwnerId = userid;
            }

            return cards;
        }

        /*
         *  Delete
         */
        /*public void Delete(Card card)
        {
            string sql = $"DELETE FROM {TABLE_NAME} WHERE card_id=@card_id;";
            try
            {
                using (var command = db.GetConnection().CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.AddWithValue($"@card_id", card.CardID);
                    command.ExecuteNonQuery();
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                throw; // Better Error Handeling
            }
        }  
        */
    }
}
