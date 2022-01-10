using MonsterCardTradingGame.Exceptions;
using Npgsql;
using System;
using System.Collections.Generic;

namespace MonsterCardTradingGame.DAL.Repository
{
    class UserRepository : IUserRepository
    {
        Postgres.PostgresAccess db = Postgres.PostgresAccess.Instance;
        private const string TABLE_NAME = "player";

        /* 
         * Create
         */
        public void Create(Model.Credentials cred)
        {
            //Console.WriteLine($"[{DateTime.UtcNow}]\tCreate new user \"{cred.Username}\"");

            string sql = $"INSERT INTO {TABLE_NAME} (user_id, username, password) VALUES (@user_id, @username, @password)";
            try
            {
                using (var command = db.GetConnection().CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.AddWithValue("@user_id", Guid.NewGuid());
                    command.Parameters.AddWithValue($"@username", cred.Username);
                    command.Parameters.AddWithValue($"@password", cred.Password);
                    command.ExecuteNonQuery();
                }
            }
            catch (PostgresException e) when (e.SqlState == "23505") // User already exists
            {
                Console.WriteLine($"[{DateTime.UtcNow}]\tError creating new user \"{cred.Username}\"\n{e.Message}");
                throw new RepositoryException("User already exists");
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"[{DateTime.UtcNow}]\tError creating new user ({cred.Username}), {e.Message}");
                throw new RepositoryException("Error");
            }
        }

        /*
         *  Read
         */
        public Model.User GetByName(string username)
        {
            try
            {
                using (var command = db.GetConnection().CreateCommand())
                {
                    string sql = $"SELECT user_id, username, password, coins, active_deck FROM {TABLE_NAME} WHERE username=@username;";
                    //Console.WriteLine($"[{DateTime.UtcNow}]\tExecute SQL-Statement: {sql}");
                    command.CommandText = sql;
                    command.Parameters.AddWithValue($"@username", username);
                    using NpgsqlDataReader reader = command.ExecuteReader();

                    reader.Read();
                    string _userid = reader.GetString(0);
                    string _username = reader.GetString(1);
                    string _password = reader.GetString(2);
                    int _coins = reader.GetInt32(3);
                    var _active_deck = reader.GetValue(4);

                    if (_active_deck.ToString() == "")
                    {
                        return new Model.User(Guid.Parse(_userid), _username, _password, _coins);
                    }
                    else
                    {
                        string deckid = _active_deck.ToString();
                        return new Model.User(Guid.Parse(_userid), _username, _password, _coins, (Guid)Guid.Parse(deckid));
                    }
                }
            }
            catch (System.Exception e) when (e.Message == "No row is available")
            {
                Console.WriteLine($"[{DateTime.UtcNow}]\tUser \"{username}\" does not exist.");
                throw new RepositoryException("User does not exist");
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"[{DateTime.UtcNow}]\tCould not get \"{username}\", {e.Message}");
                throw new RepositoryException("Error");
            }
        }

        public IEnumerable<Model.User> GetAll()
        {
            string sql = $"SELECT * FROM {TABLE_NAME};";
            try
            {
                using (var command = db.GetConnection().CreateCommand())
                {
                    command.CommandText = sql;
                    using NpgsqlDataReader reader = command.ExecuteReader();

                    string username, password, userId;
                    int coins;

                    List<Model.User> users = new();

                    while (reader.Read())
                    {
                        userId = reader.GetString(0);
                        username = reader.GetString(1);
                        password = reader.GetString(2);
                        coins = reader.GetInt32(3);

                        users.Add(new Model.User(Guid.Parse(userId), username, password, coins));
                    }

                    return users;
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                throw; // Better Error Handeling
            }
        }

        public Guid GetIdByUsername(string username)
        {
            //Console.WriteLine($"[{DateTime.UtcNow}]\tGet userid for \"{username}\"");

            try
            {
                using (var command = db.GetConnection().CreateCommand())
                {
                    string sql = $"SELECT user_id FROM {TABLE_NAME} WHERE username=@username;";
                    command.CommandText = sql;
                    //Console.WriteLine($"[{DateTime.UtcNow}]\tExecute SQL-Statement: {sql}");
                    command.Parameters.AddWithValue($"@username", username);
                    string userid = command.ExecuteScalar().ToString();
                    return Guid.Parse(userid);
                }
            }
            catch (Exception e) when (e.Message == "No row is available")
            {
                Console.WriteLine($"[{DateTime.UtcNow}]\tUser \"{username}\" does not exist.");
                throw new RepositoryException("user not found");
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"[{DateTime.UtcNow}]\tCould not get \"{username}\", {e.Message}");
                throw new RepositoryException("db error");
            }
        }

        /*
        public Model.User GetById(Guid userid)
        {
            string sql = $"SELECT * FROM {TABLE_NAME} WHERE user_id=@user_id;";
            try
            {
                using (var command = db.GetConnection().CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.AddWithValue($"@user_id", userid);

                    using NpgsqlDataReader reader = command.ExecuteReader();

                    string username, userId, password;
                    int coins;

                    reader.Read();
                    userId = reader.GetString(0);
                    username = reader.GetString(1);
                    password = reader.GetString(2);
                    coins = reader.GetInt32(3);

                    return new Model.User(Guid.Parse(userId), username, password, coins);
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                throw; // Better Error Handeling
            }
        }
        */

        /* Update */
        public void UpdateDeck(Guid userid, Guid deckid)
        {
            try
            {
                using (var command = db.GetConnection().CreateCommand())
                {
                    string sql = $"UPDATE {TABLE_NAME} SET active_deck=@deck_id WHERE user_id=@user_id;";
                    //Console.WriteLine($"[{DateTime.UtcNow}]\tExecute SQL-Statement: {sql}");
                    command.CommandText = sql;
                    command.Parameters.AddWithValue($"@deck_id", deckid.ToString());
                    command.Parameters.AddWithValue($"@user_id", userid.ToString());
                    var affectedRows = command.ExecuteNonQuery();
                    //Console.WriteLine($"[{DateTime.UtcNow}]\tAffected rows: {affectedRows}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw; 
            }
        }
    }
}
