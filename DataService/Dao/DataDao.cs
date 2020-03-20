﻿﻿using System;
using System.Data.SqlClient;
 using Connector.Dao;
  using DataService.Dao;

  namespace OPAService.Dao
{
    public class DataDao : IDataDao
    {
        private string _jsonData;

        private const string LoadSql = "select form_json from form where form_userid = @Param1 and form_form = @Param2";

        private const string ExistsSql = "select count(*) from form where form_userid = @Param1 and form_form = @Param2";
        
        private const string SaveSql = "insert into form (form_userid, form_form, form_json) values(@Param1, @Param2, @Param3)";

        private const string UpdateSql = "update form set form_json = @Param3 where  form_userid = @Param1 and form_form = @Param2";

        private DataDao()
        {
        }

        public void Save(string userId, string formName, string jsonData)
        {
            _jsonData = jsonData;

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
                    insertCommand.Parameters.AddWithValue("@Param3", jsonData);

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
                    updateCommand.Parameters.AddWithValue("@Param3", jsonData);

                    int updates = updateCommand.ExecuteNonQuery();
                    Console.WriteLine("Rows updated: " + updates);
                }

                transaction.Commit();
            }
        }

        public string Load(string userId, string formName)
        {
            string load_json = null;
            
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
                        load_json = reader.GetString(reader.GetOrdinal("form_json"));
                    }
                }
            }
            
            return load_json == null ? load_json : _jsonData;
        }
        
    }
}