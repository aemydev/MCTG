using MonsterCardTradingGame.DAL.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Data;

namespace MonsterCardTradingGame.DAL.Respository
{
    class UserRepository : IUserRepository
    {
        /* Create */
        public void Create(Model.User user)
        {
            try
            {
                DAL.Postgres.DBAccess db = DAL.Postgres.DBAccess.Instance;
                db.Insert(user);
            }
            catch(System.Exception e)
            {
                // Sth went wrong with Creating the User(bzw. Player)
                Console.WriteLine($"[{DateTime.UtcNow}] {e.Message} {e.GetType()}");
                throw;
            }
       
        }

        /* Read */
        public bool UserExistsByUsername(string username)
        {
            throw new NotImplementedException();
        }

        public Model.User GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Model.User GetByName(string username)
        {
            using (NpgsqlConnection _connection = new NpgsqlConnection("Host = localhost; Username=postgres;Password=ines;Database=test;Port=5432"))
            {
                _connection.Open();
                //Use Connection here 
            }

            return new Model.User("ines", "token");
        }

        public IEnumerable<Model.User> GetAllUser()
        {
            throw new NotImplementedException();
        }

        public string GetPwByUsername(string username)
        {
            try
            {
                DAL.Postgres.DBAccess db = DAL.Postgres.DBAccess.Instance;
                return db.SelectPwByUsername(username);
            }
            catch(Npgsql.NpgsqlException e)
            {
                Console.WriteLine($"{DateTime.UtcNow}] {e.Message}");
                throw;
            }
        }

        /* Update */
        public void UpdateUser(Model.User user)
        {
            throw new NotImplementedException();
        }

        /* Delete */
        public void DeleteUser(Model.User user)
        {
            throw new NotImplementedException();
        }
    }
}
