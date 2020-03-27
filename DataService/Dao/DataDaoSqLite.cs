﻿﻿﻿using System;
  using System.Collections.Generic;
  using System.Data.Common;
  using DataService.Dao;
  using DataService.Model;
  using Microsoft.Data.Sqlite;

  namespace Connector.Dao
{
    public class DataDaoSQLite : IDataDao
    {
        private const string CreateTableSql =
            @"CREATE TABLE IF NOT EXISTS
                form(
                    form_form TEXT,
                    form_request_id INTEGER,
                    form_stage TEXT,
                    form_userid TEXT,
                    form_json TEXT
                 )";

        private const string LoadSql = @"select
             form_stage,
             form_userid,
             form_json
             from form
             where form_form = @Param1
             and form_request_id = @Param2";

        private const string ExistsSql = "select count(*) from form where form_userid = @Param1 and form_form = @Param2";
        
        private const string SaveSql = @"insert into form (form_form, form_stage, form_userid, form_json)
            values(@Param1, @Param2, @Param3, @Param4)";

        private const string UpdateSql = @"update form
            set form_json = @Param4
            where form_userid = @Param1 
            and form_form = @Param2
            and form_request_id = @Param3";

        private const string GetByStageSql = "select form_form, form_request_id, form_stage, form_userid from form where form_stage = @Param1";

        public DataDaoSQLite()
        {
        }

        public string Save(string formName, string stage, string userId, string jsonData)
        {
            Console.WriteLine($"DataDaoSqlLite.Save formName={formName} stage={stage} userId={userId} jsonData={jsonData}");
            using (DbConnection connection = DatabaseHelper.GetDatabaseConnectionSqlite())
            {
                DbCommand createTableCommand = connection.CreateCommand();
                createTableCommand.CommandText = CreateTableSql;
                createTableCommand.ExecuteNonQuery();

                DbTransaction transaction = connection.BeginTransaction();

                    DbCommand insertCommand = connection.CreateCommand();
                    insertCommand.Transaction = transaction;
                    insertCommand.CommandText = SaveSql;

                    insertCommand.Parameters.Add(new SqliteParameter("@Param1", formName));
                    insertCommand.Parameters.Add(new SqliteParameter("@Param2", stage));
                    insertCommand.Parameters.Add(new SqliteParameter("@Param3", userId));
                    insertCommand.Parameters.Add(new SqliteParameter("@Param4", jsonData));

                    int inserts = insertCommand.ExecuteNonQuery();
                    Console.WriteLine("Rows inserted: " + inserts);
                    
                    DbCommand lastRowIdCommand = connection.CreateCommand();
                    lastRowIdCommand.CommandText = "select last_insert_rowid()";
                    // The row ID is a 64-bit value - cast the Command result to an Int64.
                    //
                    Int64 LastRowID64 = (Int64)lastRowIdCommand.ExecuteScalar();
                    // Then grab the bottom 32-bits as the unique ID of the row.
                    //
                    int LastRowID = (int)LastRowID64;

                    DbCommand updateRequestIdCommand = connection.CreateCommand();
                    updateRequestIdCommand.CommandText =
                        "update form set form_request_id = @Param1 where ROWID = @Param1";
                    
                transaction.Commit();
                return LastRowID.ToString();
            }
        }

        public void Save(string formName, string requestId, string stage, string userId, string jsonData)
        {
            Console.WriteLine($"DataDaoSqlLite.Save formName={formName} requestId={requestId} stage={stage} userId={userId} jsonData={jsonData}");
            using (DbConnection connection = DatabaseHelper.GetDatabaseConnectionSqlite())
            {
                DbCommand createTableCommand = connection.CreateCommand();
                createTableCommand.CommandText = CreateTableSql;
                createTableCommand.ExecuteNonQuery();

                DbTransaction transaction = connection.BeginTransaction();

                    DbCommand updateCommand = connection.CreateCommand();
                    updateCommand.Transaction = transaction;
                    updateCommand.CommandText = UpdateSql;

                    updateCommand.Parameters.Add(new SqliteParameter("@Param1", userId));
                    updateCommand.Parameters.Add(new SqliteParameter("@Param2", formName));
                    updateCommand.Parameters.Add(new SqliteParameter("@Param3", requestId));
                    updateCommand.Parameters.Add(new SqliteParameter("@Param4", jsonData));

                    int updates = updateCommand.ExecuteNonQuery();
                    Console.WriteLine("Rows updated: " + updates);

                    transaction.Commit();
            }
        }

        public Form Load(string formName, string requestId)
        {
            Console.WriteLine($"DataDaoSqlLite.Save formName={formName} requestId={requestId}");
            Form form = null;
            using (DbConnection connection = DatabaseHelper.GetDatabaseConnectionSqlite())
            {
                DbCommand createTableCommand = connection.CreateCommand();
                createTableCommand.CommandText = CreateTableSql;
                createTableCommand.ExecuteNonQuery();
                
                DbCommand command = connection.CreateCommand();
                command.CommandText = LoadSql;
                command.Parameters.Add(new SqliteParameter("@Param1", formName));
                command.Parameters.Add(new SqliteParameter("@Param2", requestId));
                form = new Form();
                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        form.form_stage = reader.GetString(reader.GetOrdinal("form_stage"));
                        form.user_id = reader.GetString(reader.GetOrdinal("form_userid"));
                        form.form_data_json = reader.GetString(reader.GetOrdinal("form_json"));
                        form.form_id = formName;
                        form.request_id = requestId;
                    }
                }
            }
            
            return form;
        }

        public List<Form> GetByStage(string stage)
        {
            Console.WriteLine($"DataDaoSqlLite.GetByStage stage={stage}");
            List<Form> forms = new List<Form>();
            using (DbConnection connection = DatabaseHelper.GetDatabaseConnectionSqlite())
            {
                DbCommand createTableCommand = connection.CreateCommand();
                createTableCommand.CommandText = CreateTableSql;
                createTableCommand.ExecuteNonQuery();
                
                DbCommand command = connection.CreateCommand();
                command.CommandText = GetByStageSql;
                command.Parameters.Add(new SqliteParameter("@Param1", stage));
                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Form form = new Form();
                        form.form_id = reader.GetString(reader.GetOrdinal("form_form"));
//                        form.request_id = reader.GetString(reader.GetOrdinal("form_request_id"));
                        form.form_stage = reader.GetString(reader.GetOrdinal("form_stage"));
                        form.user_id = reader.GetString(reader.GetOrdinal("form_userid"));
                        forms.Add(form);
                    }
                }
            }

            return forms;
        }
        
    }
}