﻿﻿using System;
using System.Data.Common;
  using DataService.Dao;
  using Microsoft.Data.Sqlite;

  namespace Connector.Dao
{
    public class DataDaoSQLite : IDataDao
    {
        private string _jsonData;

        private const string LoadSql = "select form_json from form where form_userid = @Param1 and form_form = @Param2";

        private const string ExistsSql = "select count(*) from form where form_userid = @Param1 and form_form = @Param2";
        
        private const string SaveSql = "insert into form (form_userid, form_form, form_json) values(@Param1, @Param2, @Param3)";

        private const string UpdateSql = "update form set form_json = @Param3 where  form_userid = @Param1 and form_form = @Param2";

        public DataDaoSQLite()
        {
        }

        public void Save(string userId, string formName, string jsonData)
        {
            _jsonData = jsonData;

            using (DbConnection connection = DatabaseHelper.GetDatabaseConnectionSqlite())
            {
                DbTransaction transaction = connection.BeginTransaction();

                DbCommand existsCommand = connection.CreateCommand();
                existsCommand.Transaction = transaction;
                existsCommand.CommandText = ExistsSql;
//                existsCommand.Parameters.Add(new SqliteParameter("@Param1", userId));
//                existsCommand.Parameters.Add(new SqliteParameter("@Param2", formName));

                Int32 count = (Int32) existsCommand.ExecuteScalar();

                if (count == 0)
                {
                    DbCommand insertCommand = connection.CreateCommand();
                    insertCommand.Transaction = transaction;
                    insertCommand.CommandText = SaveSql;

//                    insertCommand.Parameters.Add(new SqliteParameter("@Param1", userId));
//                    insertCommand.Parameters.Add(new SqliteParameter("@Param2", formName));
//                    insertCommand.Parameters.Add(new SqliteParameter("@Param3", jsonData));

                    int inserts = insertCommand.ExecuteNonQuery();
                    Console.WriteLine("Rows inserted: " + inserts);
                }
                else
                {
                    DbCommand updateCommand = connection.CreateCommand();
                    updateCommand.Transaction = transaction;
                    updateCommand.CommandText = UpdateSql;

//                    updateCommand.Parameters.Add(new SqliteParameter("@Param1", userId));
//                    updateCommand.Parameters.Add(new SqliteParameter("@Param2", formName));
//                    updateCommand.Parameters.Add(new SqliteParameter("@Param3", jsonData));

                    int updates = updateCommand.ExecuteNonQuery();
                    Console.WriteLine("Rows updated: " + updates);
                }

                transaction.Commit();
            }
        }

        public string Load(string userId, string formName)
        {
            string load_json = null;
            
            using (DbConnection connection = DatabaseHelper.GetDatabaseConnectionSqlite())
            {
                DbCommand createTableCommand = connection.CreateCommand();
                createTableCommand.CommandText = "CREATE TABLE IF NOT EXISTS form(form_userid TEXT, form_form TEXT, form_json TEXT)";
                createTableCommand.ExecuteNonQuery();
                
                DbCommand command = connection.CreateCommand();
                command.CommandText = LoadSql;
                command.Parameters.Add(new SqliteParameter("@Param1", userId));
                command.Parameters.Add(new SqliteParameter("@Param2", formName));
                using (DbDataReader reader = command.ExecuteReader())
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