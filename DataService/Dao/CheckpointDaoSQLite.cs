﻿using System;
using System.Data.Common;
 using DataService.Dao;
 using DataService.Model;
 using Microsoft.Data.Sqlite;

 namespace Connector.Dao
{
    public class CheckpointDaoSQLite : ICheckpointDao
    {
        private const string LoadSql = "select checkpoint_data from [checkpoint] where checkpoint_userid = @Param1 and checkpoint_form = @Param2";

        private const string ExistsSql = "select count(*) from [checkpoint] where checkpoint_userid = @Param1 and checkpoint_form = @Param2";
        
        private const string SaveSql = "insert into [checkpoint] (checkpoint_userid, checkpoint_form, checkpoint_data) values(@Param1, @Param2, @Param3)";

        private const string UpdateSql = "update [checkpoint] set checkpoint_data = @Param3 where  checkpoint_userid = @Param1 and checkpoint_form = @Param2";

        public CheckpointDaoSQLite()
        {
        }
        
        public Checkpoint GetCheckpointData(string userId, string formName)
        {
            string load_checkpoint = null;
            
            using (DbConnection connection = DatabaseHelper.GetDatabaseConnectionSqlite())
            {
                DbCommand command = connection.CreateCommand();
                command.CommandText = LoadSql;
                command.Parameters.Add(new SqliteParameter("@Param1", userId));
                command.Parameters.Add(new SqliteParameter("@Param2", formName));
                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        load_checkpoint = reader.GetString(reader.GetOrdinal("checkpoint_data"));
                    }
                }
            }
            
            Checkpoint checkpoint = new Checkpoint();
            checkpoint.checkpoint_data = load_checkpoint;
            return checkpoint;
        }

        public string SetCheckpointData(string userId, string formName, string checkpointData)
        {
            using (DbConnection connection = DatabaseHelper.GetDatabaseConnectionSqlite())
            {
                DbCommand createTableCommand = connection.CreateCommand();
                createTableCommand.CommandText = "CREATE TABLE IF NOT EXISTS checkpoint(checkpoint_userid TEXT, checkpoint_form TEXT, checkpoint_data TEXT)";
                createTableCommand.ExecuteNonQuery();
                
                DbTransaction transaction = connection.BeginTransaction();

                DbCommand existsCommand = connection.CreateCommand();
                existsCommand.Transaction = transaction;
                existsCommand.CommandText = ExistsSql;
                existsCommand.Parameters.Add(new SqliteParameter("@Param1", userId));
                existsCommand.Parameters.Add(new SqliteParameter("@Param2", formName));

                Int64 count = (Int64) existsCommand.ExecuteScalar();

                if (count == 0)
                {
                    DbCommand insertCommand = connection.CreateCommand();
                    insertCommand.Transaction = transaction;
                    insertCommand.CommandText = SaveSql;

                    insertCommand.Parameters.Add(new SqliteParameter("@Param1", userId));
                    insertCommand.Parameters.Add(new SqliteParameter("@Param2", formName));
                    insertCommand.Parameters.Add(new SqliteParameter("@Param3", checkpointData));

                    int inserts = insertCommand.ExecuteNonQuery();
                    Console.WriteLine("Rows inserted: " + inserts);
                }
                else
                {
                    DbCommand updateCommand = connection.CreateCommand();
                    updateCommand.Transaction = transaction;
                    updateCommand.CommandText = UpdateSql;

                    updateCommand.Parameters.Add(new SqliteParameter("@Param1", userId));
                    updateCommand.Parameters.Add(new SqliteParameter("@Param2", formName));
                    updateCommand.Parameters.Add(new SqliteParameter("@Param3", checkpointData));

                    int updates = updateCommand.ExecuteNonQuery();
                    Console.WriteLine("Rows updated: " + updates);
                }

                transaction.Commit();
            }

            return "1";
        }
    }
}