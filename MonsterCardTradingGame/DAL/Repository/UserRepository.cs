using MonsterCardTradingGame.DAL.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Data;
using MonsterCardTradingGame.Exceptions;

namespace MonsterCardTradingGame.DAL.Respository
{
    class UserRepository : IUserRepository
    {
        DAL.Postgres.PostgresAccess db = DAL.Postgres.PostgresAccess.Instance;
        private const string TABLE_NAME = "player";

        /* 
         * Create
         */
        public void Create(Model.Credentials cred)
        {
            Console.WriteLine($"[{DateTime.UtcNow}]\tCreate new user \"{cred.Username}\"");
            
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
            }catch(PostgresException e) when (e.SqlState == "23505") // User already exists
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
        public Model.User GetByName(string _username)
        {
            Console.WriteLine($"[{DateTime.UtcNow}]\tGet user \"{_username}\"");

            string sql = $"SELECT * FROM {TABLE_NAME} WHERE username=@username;";
            try
            {
                using (var command = db.GetConnection().CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.AddWithValue($"@username", _username);

                    using NpgsqlDataReader reader = command.ExecuteReader();

                    string username, userid, password;
                    int coins;

                    reader.Read();
                    userid = reader.GetString(0);
                    username = reader.GetString(1);
                    password = reader.GetString(2);
                    coins = reader.GetInt32(3);

                    return new Model.User(Guid.Parse(userid), username, password, coins);
                }
            }catch (System.Exception e) when (e.Message == "No row is available")
            {
                Console.WriteLine($"[{DateTime.UtcNow}]\tUser \"{_username}\" does not exist.");
                throw new RepositoryException("User does not exist");
            }
            catch(System.Exception e)
            {
                Console.WriteLine($"[{DateTime.UtcNow}]\tCould not get \"{_username}\", {e.Message}");
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

        /* Update */
        public void Update(Model.User user)
        {
            throw new NotImplementedException();
        }

        /* Delete */
        public void Delete(Model.User user)
        {
            throw new NotImplementedException();
        }
    }
}
