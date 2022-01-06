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
        DAL.Postgres.DBAccess db = DAL.Postgres.DBAccess.Instance;
        private const string TABLE_NAME = "player"; 
        
        /* Create */
        public void Create(Model.Credentials cred)
        {
            try
            {
                Dictionary<string, object> keyValue = new();
                keyValue.Add("username", cred.Username);
                keyValue.Add("password", cred.Password);
                db.Insert(TABLE_NAME, keyValue);
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
            throw new NotImplementedException();

            /*
            try
            {
                List<string> keys = new() { "*" };
                Dictionary<string, string> where = new();
                where.Add("username", username);

                db.Select(TABLE_NAME, keys, where);
            }
            catch (System.Exception e)
            {
                // Sth went wrong with Creating the User(bzw. Player)
                Console.WriteLine($"[{DateTime.UtcNow}] {e.Message} {e.GetType()}");
                throw;
            }


        /*    using (NpgsqlConnection _connection = new NpgsqlConnection("Host = localhost; Username=postgres;Password=ines;Database=test;Port=5432"))
            {
                _connection.Open();
                //Use Connection here 
            }

            return new Model.User("ines", "token");
            */
        }

        public IEnumerable<Model.User> GetAll()
        {
            throw new NotImplementedException();
        }

        public string GetPasswordByUsername(string username)
        {
            try
            {
                List<string> keys = new() { "password" };
                string where = $"WHERE username == {username}";
                //return db.Select(TABLE_NAME, keys, where);
                return "";
                
            }
            catch(Npgsql.NpgsqlException e)
            {
                Console.WriteLine($"{DateTime.UtcNow}] {e.Message}");
                throw;
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
