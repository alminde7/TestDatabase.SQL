using System;
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
            string result = string.Empty;

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
        public void Create_SupplyInvalidConnectionString_ThrowsArgumentException()
        {
            var connectionString = "This_Is_An_Invalid_Connection_String";
            var testDb = new TestDatabase(dbName, connectionString);

            Assert.Throws<ArgumentException>(() => testDb.Create());

        }

        [Test]
        public void Migrate_MigrateCreateTablesSqlScript_TableHasBeenCreated()
        {
            string result = string.Empty;
            var testDb = new TestDatabase(dbName, Config.ConnectionString);
            testDb.Create();
            testDb.Migrate(Config.PathToMigrationScripts);

            using (SqlConnection conn = new SqlConnection(Config.ConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    //Returns the number of columns in table. 
                    cmd.CommandText = $"select count(*) as IsExists from {testDb.DbName}.information_schema.columns " +
                                      $"where table_name = 'Users'";
                    result = cmd.ExecuteScalar().ToString();
                }
            }
            
            Assert.That(result, Is.EqualTo("3"));

            testDb.Delete();
        }

        [Test]
        public void Migrate_MigrateInvalidSqlScript_ThrowsSqlException()
        {
            var testDb = new TestDatabase(dbName, Config.ConnectionString);
            testDb.Create();

            Assert.Throws<SqlException>(() => testDb.Migrate(Config.PathToInvalidScripts));

            testDb.Delete();
        }

        [Test]
        public void Migrate_MigrateTwoScripts_ScriptsHasBeenExecutedInOrderOfName()
        {
            //Arrange
            string result = string.Empty;
            var testDb = new TestDatabase(dbName, Config.ConnectionString);
            testDb.Create();
            //Act
            testDb.Migrate(Config.PathToMultipleScripts);

            //Assert
            using (SqlConnection conn = new SqlConnection(Config.ConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"select count(*) as IsExists from {testDb.DbName}.information_schema.columns " +
                                      $"where table_name = 'Users' and column_name = 'DateOfBirth'";
                    result = cmd.ExecuteScalar().ToString();
                }
            }

            // "0" = Not found; "1" = found column
            Assert.That(result, Is.EqualTo("1"));
            
            testDb.Delete();
        }

        [Test]
        public void Delete_CreateDatabaseAndDeleteIt_DatabaseIsDeleted()
        {
            string result = string.Empty;
            bool throwsException = false;

            var testDb = new TestDatabase(dbName, Config.ConnectionString);
            testDb.Create();
            testDb.Delete();

            using (SqlConnection conn = new SqlConnection(Config.ConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"SELECT name FROM sys.databases WHERE Name = '{testDb.DbName}'";
                    try
                    {
                        result = cmd.ExecuteScalar().ToString();
                    }
                    catch (Exception e)
                    {
                        throwsException = true;
                    }
                }
            }

            Assert.That(throwsException, Is.True);
        }

        [Test]
        public void Delete_DeleteDatabaseIsCalledTwice_DatabaseIsDeleted()
        {
            string result = string.Empty;
            bool throwsException = false;

            var testDb = new TestDatabase(dbName, Config.ConnectionString);
            testDb.Create();
            testDb.Delete();
            testDb.Delete();

            using (SqlConnection conn = new SqlConnection(Config.ConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"SELECT name FROM sys.databases WHERE Name = '{testDb.DbName}'";
                    try
                    {
                        result = cmd.ExecuteScalar().ToString();
                    }
                    catch (Exception e)
                    {
                        throwsException = true;
                    }
                }
            }

            Assert.That(throwsException, Is.True);
        }

        [Test]
        public void Dispose_CreateDatabaseWithUsingStatement_DatabaseHasBeenDeletedWhenUsingStatementIsDone()
        {
            string result = string.Empty;
            bool throwsException = false;
            string testDbName = string.Empty;

            using (var testDb = new TestDatabase(dbName, Config.ConnectionString))
            {
                testDb.Create();
                testDbName = testDb.DbName;
            }

            using (SqlConnection conn = new SqlConnection(Config.ConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"SELECT name FROM sys.databases WHERE Name = '{testDbName}'";
                    try
                    {
                        result = cmd.ExecuteScalar().ToString();
                    }
                    catch (Exception e)
                    {
                        throwsException = true;
                    }
                }
            }

            Assert.That(throwsException, Is.True);
        }

        [Test]
        public void FailingTest_ForTesting()
        {
            Assert.True(false);
        }
    }
}
