using System;
using System.Data.SqlClient;

namespace TestDatabase.SQL
{
    public class TestDatabase : IDisposable
    {
        public readonly string DbName;
        public readonly string Connectionstring;

        public TestDatabase(string dbName, string connectionstring)
        {
            DbName = dbName + "_Test_" + Guid.NewGuid().ToString("N");
            Connectionstring = connectionstring;
        }

        /// <summary>
        /// Creates a testdatabase
        /// </summary>
        public void CreateDatabase()
        {
            using (SqlConnection connection = new SqlConnection(Connectionstring))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"CREATE DATABASE {DbName}";
                    command.ExecuteNonQuery();
                }

                // Due to https://dba.stackexchange.com/questions/58137/db-owner-unable-to-drop-database-error-615-sql-server
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"ALTER DATABASE [{DbName}] SET AUTO_CLOSE OFF";
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Runs SQL migration scripts on database. A path to the folder containing the SQL scripts must be provided. 
        /// </summary>
        /// <param name="pathToMigrationScripts"></param>
        public void Migrate(string pathToMigrationScripts)
        {
            DbUp.DeployChanges.To
                .SqlDatabase(Connectionstring)
                .WithScriptsFromFileSystem(pathToMigrationScripts)
                .LogToConsole()
                .Build();
        }

        /// <summary>
        /// Deletes the testdatabase
        /// </summary>
        public void DeleteDatabase()
        {
            SqlConnection.ClearAllPools();

            using (SqlConnection connection = new SqlConnection(Connectionstring))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        $"IF EXISTS(select * from sys.databases where name='{DbName}') DROP DATABASE {DbName}";

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Deletes the testdatabase
        /// </summary>
        public void Dispose()
        {
            DeleteDatabase();
        }
    }
}
