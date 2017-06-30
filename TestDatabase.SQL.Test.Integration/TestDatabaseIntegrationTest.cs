using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace TestDatabase.SQL.Test.Integration
{
    [TestFixture]
    public class TestDatabaseIntegrationTest
    {
        [Test]
        public void Constructor_CreateNewInstanceOfObject_DbNameAndConnectionStringHasBeenSet()
        {
            var dbName = "Test";

            var testDb = new TestDatabase(dbName, Config.ConnectionString);

            Assert.NotNull(testDb.DbName);
            Assert.That(testDb.Connectionstring, Is.EqualTo(Config.ConnectionString));
        }

    }
}
