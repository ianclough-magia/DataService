using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Data.Sqlite;

namespace DataService.Dao
{
    public class DatabaseHelper
    {

        public static void ListProviders()
        {
            DataTable dataTable = DbProviderFactories.GetFactoryClasses();
            Console.WriteLine("Data providers...");
            foreach (DataRow row in dataTable.Rows)
            {
                Console.WriteLine("  Provider...");
                foreach (DataColumn column in dataTable.Columns)
                {
                    Console.WriteLine("    " + row[column]);
                }
            }
        }
        
        public static DbConnection GetDatabaseConnectionOracle()
        {
            DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.OracleClient");
            DbConnection connection = factory.CreateConnection();
            connection.ConnectionString = "Data Source=localhost:1521/XE;User ID=SBS;Password=password";
            connection.Open();
            return connection;
        }
        
        public static SqlConnection GetDatabaseConnectionSqlServer()
        {
            ListProviders();
            
//            DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
//            DbConnection connection = factory.CreateConnection();
//            connection.ConnectionString =
                string connectionString = "Data Source=IANCLOUGH-PC\\SQLEXPRESS;Initial Catalog=SBS;Integrated Security=true;";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            return (SqlConnection) connection;
        }

        public static DbConnection GetDatabaseConnectionSqlite()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder();

            //Use DB in project directory.  If it does not exist, create it:
            connectionStringBuilder.DataSource = "c:/temp2/SqliteDB.db";

            DbConnection connection = new SqliteConnection(connectionStringBuilder.ConnectionString);
            connection.Open();
            return connection;
        }
    }
}