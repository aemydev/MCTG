﻿using MonsterCardTradingGame.Exceptions;
using MonsterCardTradingGame.Model;
using Npgsql;
using System;
using System.Collections.Generic;

namespace MonsterCardTradingGame.DAL.Repository
{
    class ScoreboardRepository : IScoreboardRepository
    {
        Postgres.PostgresAccess db = Postgres.PostgresAccess.Instance;
        private const string TABLE_NAME = "scoreboard";

        /*
         *  Create
         */
        public void Create(ScoreboardEntry entry)
        {
            try
            {
                using (var command = db.GetConnection().CreateCommand())
                {
                    string sql = $"INSERT INTO {TABLE_NAME} (id, user_id, elo) VALUES (@id, @user_id, @elo);";
                    command.CommandText = sql;
                    command.Parameters.AddWithValue("@id", Guid.NewGuid().ToString());
                    command.Parameters.AddWithValue($"@user_id", entry.UserId.ToString());
                    command.Parameters.AddWithValue($"@elo", entry.Elo+100);
                    command.ExecuteNonQuery();
                }
            }
            catch (PostgresException e) when (e.SqlState == "23505") // User already exists
            {
                Console.WriteLine($"[{DateTime.UtcNow}]\tError creating new scoreboard entry");
                throw new RepositoryException("Entry already exists");
            }
            catch
            {
                Console.WriteLine($"[{DateTime.UtcNow}]\tError creating new scoreboard-entry");
                throw new RepositoryException("Error");
            }
        }

        /*
         *  Read
         */
        public ScoreboardEntry GetEntry(Guid userid)
        {
            try
            {
                using (var command = db.GetConnection().CreateCommand())
                {
                    string sql = $"SELECT * FROM {TABLE_NAME} WHERE user_id=@user_id;";

                    command.CommandText = sql;
                    command.Parameters.AddWithValue($"@user_id", userid.ToString());

                    using NpgsqlDataReader reader = command.ExecuteReader();

                    reader.Read();
                    string id = reader.GetString(0);
                    string user_id = reader.GetString(1);
                    int elo = reader.GetInt32(2);
                    int games_played = reader.GetInt32(3);

                    return new Model.ScoreboardEntry(Guid.Parse(id), Guid.Parse(user_id), elo, games_played);
                }
            }
            catch
            {
                throw new RepositoryException("db error");
            }
        }

        public List<ScoreboardEntry> GetAll()
        {
            try
            {
                using (var command = db.GetConnection().CreateCommand())
                {
                    string sql = $"SELECT * FROM {TABLE_NAME} ORDER BY elo DESC;";
                    command.CommandText = sql;

                    using NpgsqlDataReader reader = command.ExecuteReader();

                    List<ScoreboardEntry> scoreboard = new();

                    while (reader.Read())
                    {
                        string id = reader.GetString(0);
                        string user_id = reader.GetString(1);
                        int elo = reader.GetInt32(2);
                        int games_played = reader.GetInt32(3);

                        scoreboard.Add(new ScoreboardEntry(Guid.Parse(id), Guid.Parse(user_id), elo, games_played));
                    }

                    return scoreboard;
                }
            }
            catch
            {
                throw new RepositoryException("db error");
            }
        }

        /*
         *  Update
         */
        public void Update(ScoreboardEntry entry)
        {
            try
            {
                using (var command = db.GetConnection().CreateCommand())
                {
                    string sql = $"UPDATE {TABLE_NAME} SET elo=elo+@elo, games_played=games_played+1 WHERE user_id=@user_id;";
                    //Console.WriteLine($"[{DateTime.UtcNow}]\tExecute SQL-Statement: {sql}");
                    command.CommandText = sql;
                    command.Parameters.AddWithValue($"@user_id", entry.UserId.ToString());
                    command.Parameters.AddWithValue($"@elo", entry.Elo);
                    var affectedRows = command.ExecuteNonQuery();
                    //Console.WriteLine($"[{DateTime.UtcNow}]\tAffected rows: {affectedRows}");
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                throw; 
            }
        }
    }
}
