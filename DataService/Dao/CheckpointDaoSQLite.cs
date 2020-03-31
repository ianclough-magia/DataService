﻿using System;
 using System.Collections.Generic;
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

        private const string DeleteSql = "delete from [checkpoint] where  checkpoint_userid = @Param1 and checkpoint_form = @Param2";

        private const string ListSql = "select checkpoint_form from checkpoint where checkpoint_userid = @Param1";
        
        public CheckpointDaoSQLite()
        {
        }
        
        public Checkpoint GetCheckpointData(string userId, string formName)
        {
            Checkpoint checkpoint = null;
            
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
                        string load_checkpoint = reader.GetString(reader.GetOrdinal("checkpoint_data"));
                        checkpoint = new Checkpoint{user_id = userId, form_id = formName, checkpoint_data = load_checkpoint};
                    }
                }
            }
            
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

        public bool DeleteCheckpoint(string userId, string formName)
        {
            Int64 count;
                
            using (DbConnection connection = DatabaseHelper.GetDatabaseConnectionSqlite())
            {
                DbTransaction transaction = connection.BeginTransaction();

                DbCommand existsCommand = connection.CreateCommand();
                existsCommand.Transaction = transaction;
                existsCommand.CommandText = ExistsSql;
                existsCommand.Parameters.Add(new SqliteParameter("@Param1", userId));
                existsCommand.Parameters.Add(new SqliteParameter("@Param2", formName));

                count = (Int64) existsCommand.ExecuteScalar();

                if (count > 0)
                {
                    DbCommand deleteCommand = connection.CreateCommand();
                    deleteCommand.CommandText = DeleteSql;
                    deleteCommand.Parameters.Add(new SqliteParameter("@Param1", userId));
                    deleteCommand.Parameters.Add(new SqliteParameter("@Param2", formName));

                    deleteCommand.ExecuteNonQuery();
                }
                transaction.Commit();
            }

            return count > 0;
        }

        public IEnumerable<Checkpoint> ListCheckpoints(string userId)
        {
            List<Checkpoint> checkpoints = new List<Checkpoint>();
            
            using (DbConnection connection = DatabaseHelper.GetDatabaseConnectionSqlite())
            {
                DbCommand command = connection.CreateCommand();
                command.CommandText = ListSql;
                command.Parameters.Add(new SqliteParameter("@Param1", userId));
                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string formName = reader.GetString(reader.GetOrdinal("checkpoint_form"));
                        Checkpoint checkpoint = new Checkpoint{user_id = userId, form_id = formName};
                        checkpoints.Add(checkpoint);
                    }
                }
            }

            return checkpoints;
        }
    }
}