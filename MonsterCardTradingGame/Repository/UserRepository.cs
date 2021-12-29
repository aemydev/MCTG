using MonsterCardTradingGame.DB.Repository;
using MonsterCardTradingGame.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Data;

namespace MonsterCardTradingGame.DB.Respository
{
    class UserRepository : IUserRepository
    {
        /* Create */
        public void Create(User user)
        {
            // Connection String Env-Varibale or config file
            using (NpgsqlConnection _connection  = new NpgsqlConnection("Host = localhost; Username=postgres;Password=ines;Database=test;Port=5432"))
            {
                _connection.Open();

                using (var command = _connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO player (username, password) VALUES (@username,@password);";
                    command.Parameters.AddWithValue("@username", user.Username);
                    command.Parameters.AddWithValue("@password", user.Password);
                    
                    // Try Catch Needed?
                    try
                    {
                        command.ExecuteScalar();

                    }catch
                    {
                        throw;
                    }
                    finally
                    {
                        _connection.Close(); // needed?
                    }
                }
            }
        }

        /* Read */
        public User GetById(int id)
        {
            throw new NotImplementedException();
        }

        public User GetByName(string username)
        {
            using (NpgsqlConnection _connection = new NpgsqlConnection("Host = localhost; Username=postgres;Password=ines;Database=test;Port=5432"))
            {
                _connection.Open();
                //Use Connection here 
            }

            return new User("ines", "token");
        }

        public IEnumerable<User> GetAllUser()
        {
            throw new NotImplementedException();
        }

        /* Update */
        public void UpdateUser(User user)
        {
            throw new NotImplementedException();
        }

        /* Delete */
        public void DeleteUser(User user)
        {
            throw new NotImplementedException();
        }

    }
}
