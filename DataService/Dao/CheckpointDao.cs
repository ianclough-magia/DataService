﻿using System;
using System.Data.SqlClient;
 using DataService.Dao;
 using DataService.Model;

 namespace Connector.Dao
{
    public class CheckpointDao : ICheckpointDao
    {
        private const string LoadSql = "select checkpoint_data from [checkpoint] where checkpoint_userid = @Param1 and checkpoint_form = @Param2";

        private const string ExistsSql = "select count(*) from [checkpoint] where checkpoint_userid = @Param1 and checkpoint_form = @Param2";
        
        private const string SaveSql = "insert into [checkpoint] (checkpoint_userid, checkpoint_form, checkpoint_data) values(@Param1, @Param2, @Param3)";

        private const string UpdateSql = "update [checkpoint] set checkpoint_data = @Param3 where  checkpoint_userid = @Param1 and checkpoint_form = @Param2";

        public CheckpointDao()
        {
        }

        public Checkpoint GetCheckpointData(string userId, string formName)
        {
            string load_checkpoint = null;
            
            using (SqlConnection connection = DatabaseHelper.GetDatabaseConnectionSqlServer())
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = LoadSql;
                command.Parameters.AddWithValue("@Param1", userId);
                command.Parameters.AddWithValue("@Param2", formName);
                using (SqlDataReader reader = command.ExecuteReader())
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
            using (SqlConnection connection = DatabaseHelper.GetDatabaseConnectionSqlServer())
            {
                SqlTransaction transaction = connection.BeginTransaction();

                SqlCommand existsCommand = connection.CreateCommand();
                existsCommand.Transaction = transaction;
                existsCommand.CommandText = ExistsSql;
                existsCommand.Parameters.AddWithValue("@Param1", userId);
                existsCommand.Parameters.AddWithValue("@Param2", formName);

                Int32 count = (Int32) existsCommand.ExecuteScalar();

                if (count == 0)
                {
                    SqlCommand insertCommand = connection.CreateCommand();
                    insertCommand.Transaction = transaction;
                    insertCommand.CommandText = SaveSql;

                    insertCommand.Parameters.AddWithValue("@Param1", userId);
                    insertCommand.Parameters.AddWithValue("@Param2", formName);
                    insertCommand.Parameters.AddWithValue("@Param3", checkpointData);

                    int inserts = insertCommand.ExecuteNonQuery();
                    Console.WriteLine("Rows inserted: " + inserts);
                }
                else
                {
                    SqlCommand updateCommand = connection.CreateCommand();
                    updateCommand.Transaction = transaction;
                    updateCommand.CommandText = UpdateSql;

                    updateCommand.Parameters.AddWithValue("@Param1", userId);
                    updateCommand.Parameters.AddWithValue("@Param2", formName);
                    updateCommand.Parameters.AddWithValue("@Param3", checkpointData);

                    int updates = updateCommand.ExecuteNonQuery();
                    Console.WriteLine("Rows updated: " + updates);
                }

                transaction.Commit();
            }

            return "1";
        }
    }
}