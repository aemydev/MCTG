using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardTradingGame.DAL.Postgres
{
    /*
     *  Access to postgres DB via ADO.NET
     *  Singelton, Lazy initialization
     */
    public sealed class PostgresAccess
    {
        private static readonly Lazy<PostgresAccess> lazy = new Lazy<PostgresAccess>(() => new PostgresAccess());
        public static PostgresAccess Instance { get { return lazy.Value; } }

        private NpgsqlConnection _connection;

        private PostgresAccess()
        {
            try
            {
                _connection = new NpgsqlConnection("Host=localhost; Username=postgres;Password=swen1;Database=mctg_db;Port=5432");
                _connection.Open();
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"[{DateTime.UtcNow}]\tError connecting to PostgresDB: {e.Message}");
                throw; //send internal server error
            }

            Console.WriteLine($"[{DateTime.UtcNow}]\tConnection to PostgresDB established.");
        }

        public NpgsqlConnection GetConnection()
        {
            return _connection;
        }
    }
}