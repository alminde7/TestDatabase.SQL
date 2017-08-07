using System.Data.SqlClient;
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
        public void Create_CreateADatabase_DatabaseHasBeenCreated()
        {
            string result;

            var testDb = new TestDatabase(dbName, Config.ConnectionString);
            testDb.Create();

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

            testDb.Delete();
        }

        [Test]
        public void Create_SupplyInvalidConnectionString_ThrowsException()
        {

        }

        [Test]
        public void Migrate_MigrateCreateTablesSqlScript_TableHasBeenCreated()
        {
            var testDb = new TestDatabase(dbName, Config.ConnectionString);
            testDb.Create();
            testDb.Migrate(Config.PathToMigrationScripts);
        }

        [Test]
        public void Migrate_MigrateCreateTableSqlScript_ColumnsHasBeenCreatedWithCorrectConstraints()
        {
            
        }

        [Test]
        public void Migrate_MigrateInvalidSqlScript_ThrowsInvalidScriptException()
        {
            //TODO:: Create InvalidScriptException
        }

        [Test]
        public void Delete_CreateDatabaseAndDeleteIt_DatabaseIsDeleted()
        {
            
        }

        [Test]
        public void Delete_DeleteDatabaseIsCalledTwice_DatabaseIsDeleted()
        {
            
        }

    }
}
