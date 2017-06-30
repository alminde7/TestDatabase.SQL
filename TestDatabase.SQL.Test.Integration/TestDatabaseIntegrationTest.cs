using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace TestDatabase.SQL.Test.Integration
{
    [TestFixture]
    public class TestDatabaseIntegrationTest
    {
        private string dbName = "Test";
        [Test]
        public void Constructor_CreateNewInstanceOfObject_DbNameAndConnectionStringHasBeenSet()
        {
            var testDb = new TestDatabase(dbName, Config.ConnectionString);

            Assert.NotNull(testDb.DbName);
            Assert.That(testDb.Connectionstring, Is.EqualTo(Config.ConnectionString));
        }

        [Test]
        public void CreateDatabase_CreateADatabase_DatabaseHasBeenCreated()
        {
            string result;

            var testDb = new TestDatabase(dbName, Config.ConnectionString);
            testDb.CreateDatabase();

            using (SqlConnection conn = new SqlConnection(Config.ConnectionString))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"SELECT name FROM sys.databases WHERE Name = '{testDb.DbName}'";

                    result = cmd.ExecuteScalar().ToString();
                }
            }

            Assert.That(testDb.DbName, Is.EqualTo(result));

            testDb.DeleteDatabase();
        }

    }
}
