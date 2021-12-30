using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.DAL
{
    // Singelton:class which only allows a single instance of itself to be created,
    // and usually gives simple access to that instance 

    // DB Connection schließen?
    public sealed class PostgresDBAccess
    {
        private static readonly Lazy<PostgresDBAccess> lazy = new Lazy<PostgresDBAccess>(() => new PostgresDBAccess());
        public static PostgresDBAccess Instance { get { return lazy.Value; } }
        
        private NpgsqlConnection _connection;

        private PostgresDBAccess()
        {
            try
            {
                _connection = new NpgsqlConnection("Host=localhost; Username=postgres;Password=ines;Database=test;Port=5432");
                _connection.Open();
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Error");
                Console.WriteLine(e.Message);
                throw; // Better Error Handeling
            }
            Console.WriteLine("Connection established");
        }

        // CRUD
        public void Insert(Model.User user)
        {
            try
            {    
                var command = _connection.CreateCommand();
                command.CommandText = "INSERT INTO player (username, password) VALUES (@username,@password);";
                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@password", user.Password);
                command.ExecuteScalar();
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                throw; // Better Error Handeling
            }
        }
    }
}