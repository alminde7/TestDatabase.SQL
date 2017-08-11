using System;
using System.Data.SqlClient;

namespace TestDatabase.SQL
{
    public class TestDatabase : IDisposable
    {
        public readonly string DbName;
        public readonly string Connectionstring;
        public readonly string ConnectionstringWithDb;

        public TestDatabase(string dbName, string connectionstring)
        {
            DbName = dbName + "_Test_" + Guid.NewGuid().ToString("N");
            Connectionstring = connectionstring;
            ConnectionstringWithDb = CreateConnectionStringWithDatabase();
        }

        /// <summary>
        /// Creates a testdatabase
        /// </summary>
        public void Create()
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
        public bool Migrate(string pathToMigrationScripts)
        {
            var sdf = DbUp.DeployChanges.To
                .SqlDatabase(ConnectionstringWithDb)
                .WithScriptsFromFileSystem(pathToMigrationScripts)
                .LogToConsole()
                .Build();


            var result = sdf.PerformUpgrade();

            if (result.Successful)
                return true;
            else
            {
                throw result.Error;
            }
        }

        /// <summary>
        /// Deletes the testdatabase
        /// </summary>
        public void Delete()
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
            Delete();
        }

        private string CreateConnectionStringWithDatabase()
        {
            string connectionstringDatabase = "Database=" + DbName + ";";

            if (Connectionstring.EndsWith(";"))
                return Connectionstring + connectionstringDatabase;
            else
                return Connectionstring + ";" + connectionstringDatabase;
        }
    }
}
