using System;
using System.Data.SqlClient;

namespace TestDatabase.SQL
{
    public class TestDatabase : IDisposable
    {
        private readonly string _dbName;
        private readonly string _connectionstring;

        public TestDatabase(string dbName, string connectionstring)
        {
            _dbName = dbName + "_Test_" + Guid.NewGuid().ToString("N");
            _connectionstring = connectionstring;
        }

        /// <summary>
        /// Creates a testdatabase
        /// </summary>
        public void CreateDatabase()
        {
            using (SqlConnection connection = new SqlConnection(_connectionstring))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"CREATE DATABASE {_dbName}";
                    command.ExecuteNonQuery();
                }

                // Due to https://dba.stackexchange.com/questions/58137/db-owner-unable-to-drop-database-error-615-sql-server
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"ALTER DATABASE [{_dbName}] SET AUTO_CLOSE OFF";
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
                .SqlDatabase(_connectionstring)
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

            using (SqlConnection connection = new SqlConnection(_connectionstring))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        $"IF EXIST(select * from sys.databases where name='{_dbName}') DROP DATABASE {_dbName}";

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
