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
    public sealed class DBAccess
    {
        private static readonly Lazy<DBAccess> lazy = new Lazy<DBAccess>(() => new DBAccess());
        public static DBAccess Instance { get { return lazy.Value; } }

        private NpgsqlConnection _connection;

        private DBAccess()
        {
            try
            {
                _connection = new NpgsqlConnection("Host=localhost; Username=postgres;Password=swen1;Database=mctg_db;Port=5432");
                _connection.Open();
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Error");
                Console.WriteLine(e.Message);
                throw;
            }
            Console.WriteLine("Connection established");
        }

        /*
         *  Create
         */
        public void Insert(string table, Dictionary<string, object> keyValue)
        {
            // Create SQL command
            string keys = "";
            string values = "";

            foreach (var item in keyValue)
            {
                keys += $"{item.Key},";
                values += $"@{item.Key.ToString()},";
            }

            keys = keys.Trim(',');
            values = values.Trim(',');

            string sql = $"INSERT INTO {table} ({keys}) VALUES ({values})";

            Console.WriteLine(sql);

            // Create and Execute Command:
            try
            {
                using (var command = _connection.CreateCommand()){
                    command.CommandText = sql;
                    // Add the Parameters:
                    foreach (var item in keyValue)
                    {
                        command.Parameters.AddWithValue($"@{item.Key}", item.Value);
                    }
                    command.ExecuteNonQuery(); 
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                throw; // Better Error Handeling
            }
        }

        #region Read

        /*
         *  Simple Select
         */
        public List<object> Select(string table, List<string> keys)
        {
            /*
             *  Generate the SQL
             */
            string keyString = "";

            foreach (string item in keys)
            {
                keyString += $"{item},";
            }

            keyString = keyString.Trim(',');

            string sql = $"SELECT {keyString} FROM {table};";
            Console.WriteLine(sql);

            /*
             *  Execute the SQL
             */
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = sql;

                using NpgsqlDataReader reader = command.ExecuteReader();

                List<object> result = new();

                while (reader.Read())
                {
                    result.Add(reader.GetValue(0));
                }

                return result;
            }          
        }

        public List<string> Select(string table, List<String> keys, Dictionary<string, string> where = null)
        {
            List<string> results = new();

            // Dictionary to string
            string whereString = "";
            foreach (var entry in where)
            {
                whereString += $"{entry.Key} = @{entry.Key} ";
            }
            Console.WriteLine(whereString);

            string sql = $"SELECT {String.Join(", ", keys.ToArray())} FROM {table} {where};";
            Console.WriteLine(sql);

            try
            {
                var cmd = new NpgsqlCommand(sql, _connection);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    results.Add(reader.GetString(0));
                }
            }
            catch
            {
                throw;
            }

            return results;

            // Dictionary

        }

        public List<string> Select(string table, List<String> keys, Dictionary<string, string> where = null, string limit=null)
        {
            List<string> results = new();


            // Dictionary to string
            string whereString = "";
            foreach (var entry in where)
            {
                whereString += $"{entry.Key} = @{entry.Key} ";
            }
            Console.WriteLine(whereString);

            string sql = $"SELECT {String.Join(", ", keys.ToArray())} FROM {table} {where} {limit};";

            try
            {
                var cmd = new NpgsqlCommand(sql, _connection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    results.Add(reader.GetString(0));
                }
            }
            catch
            {
                throw;
            }

            return results;
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

            if (pw is not null)
            {
                return pw.ToString();
            }
            return ""; // ADASFASFD
        }

        #endregion
        #region Update

        /*
         *  Update
         */
        public void Update(string table, Dictionary<string, string> items, string where)
        {
            string keyValue = "";
            foreach (KeyValuePair<string, string> item in items)
            {
                keyValue += string.Format("{0} = @{0}, ", item.Key, item.Key);

            }
            // Rmv , at the end
            keyValue = keyValue.TrimEnd(',');

            Console.WriteLine(keyValue);

            string sql = $"UPDATE {table} SET {keyValue} {where}";

            Console.WriteLine(sql);

            try
            {
                var command = _connection.CreateCommand();
                command.CommandText = sql;

                foreach (KeyValuePair<string, string> item in items)
                {
                    command.Parameters.AddWithValue($"@{item.Key}", item.Value);
                }

                command.ExecuteScalar();
                command.Dispose(); //??
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                throw; // Better Error Handeling
            }
        }

        #endregion

        /*
         *  Delete
         */


    }
}