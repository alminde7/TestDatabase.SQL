using System;
using System.IO;

namespace TestDatabase.SQL.Test.Integration
{
    internal class Config
    {
        internal static string ConnectionString =
            "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;";

        internal static string PathToMigrationScripts = GetPathToScripts("Scripts");

        internal static string PathToInvalidScripts = GetPathToScripts("InvalidScripts");

        internal static string PathToMultipleScripts = GetPathToScripts("MultipleScripts");

        private static string GetPathToScripts(string folder)
        {
            return  Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, folder);
        }
    }
}
