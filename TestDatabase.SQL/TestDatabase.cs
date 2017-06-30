using System;
using System.Data.SqlClient;

namespace Database.SQL.Test
{
    public class TestDatabase : IDisposable
    {
        private readonly string _dbName;
        private readonly string _connectionId;

        public TestDatabase(string dbName, string connectionId)
        {
            _dbName = dbName + "_Test_" + Guid.NewGuid().ToString("N");
            _connectionId = connectionId;
        }

        public void CreateDatabase()
        {
            using (SqlConnection connection = new SqlConnection(_connectionId))
            {
                connection.Open();

                SqlCommand command = new SqlCommand($"CREATE DATABASE {_dbName}");
                command.ExecuteNonQuery();

                connection.Close();
            }
        }

        public void Migrate()
        {

        }

        public void DeleteDatabase()
        {
            using (SqlConnection connection = new SqlConnection(_connectionId))
            {
                connection.Open();

                SqlCommand command = new SqlCommand($"DROP DATABASE {_dbName}");
                command.ExecuteNonQuery();

                connection.Close();
            }
        }

        public void Dispose()
        {
            DeleteDatabase();
        }
    }
}
