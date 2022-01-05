using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.DAL.Postgres
{
    // Singelton:class which only allows a single instance of itself to be created,
    // and usually gives simple access to that instance 

    // DB Connection schließen?
    public sealed class DBAccess
    {
        private static readonly Lazy<DBAccess> lazy = new Lazy<DBAccess>(() => new DBAccess());
        public static DBAccess Instance { get { return lazy.Value; } }
        
        private NpgsqlConnection _connection;

        private DBAccess()
        {
            try
            {
                //_connection = new NpgsqlConnection("Host=localhost; Username=postgres;Password=ines;Database=test;Port=5432");
                _connection = new NpgsqlConnection("Host=localhost; Username=postgres;Password=swen1;Database=mctg_db;Port=5432");

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
                command.Dispose(); //??
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                throw; // Better Error Handeling
            }
        }

        public void Insert(Model.Card card)
        {
            try
            {
                var command = _connection.CreateCommand();
                command.CommandText = "INSERT INTO card (card_id, title, description, damage) VALUES (@card_id, @title, @description, @damage);";
                command.Parameters.AddWithValue("@card_id", card.CardID);
                command.Parameters.AddWithValue("@title", card.Title);
                command.Parameters.AddWithValue("@description", card.Description);
                command.Parameters.AddWithValue("@damage", card.Damage);
                command.ExecuteScalar();

                command.Dispose(); //??
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                throw; // Better Error Handeling
            }
        }

        public string SelectPwByUsername(string username)
        {
            object pw;
            try
            {
                var sql = $"SELECT password FROM player WHERE username='{username}';";
              
                    var cmd = new NpgsqlCommand(sql, _connection);
                    pw = cmd.ExecuteScalar();
                
                
             
                // handle if user exists
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                throw; // Better Error Handeling
            }

            if(pw is not null)
            {
                return pw.ToString();
            }
            return ""; // ADASFASFD
        }
    }
}