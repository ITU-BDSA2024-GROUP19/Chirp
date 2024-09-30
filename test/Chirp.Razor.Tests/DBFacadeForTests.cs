using System.Globalization;
using Microsoft.Data.Sqlite;
using SQLitePCL;

namespace Chirp.Razor.Tests
{
    public static class DBFacadeForTests
    {
        public static string setupDBforTests()
        {
            string tmpDBLocation = createTestFile();
            Environment.SetEnvironmentVariable("CHIRPDBPATH", tmpDBLocation);
            return tmpDBLocation;
        }
        
        private static string createTestFile()
        {
            string originalDB = "../../data/test.db";
            string tmpDB = Path.GetTempFileName();
            File.Copy(originalDB, tmpDB);
            return tmpDB;
        }
        
        public static void removeDBforTests(String? originalDB, String tmpDB)
        {
            Environment.SetEnvironmentVariable("CHIRPDBPATH", originalDB);
            File.Delete(tmpDB);
        }
    }
}