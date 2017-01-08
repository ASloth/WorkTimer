using System.IO;
using SQLite.Net;
using SQLite.Net.Async;
using WorkTimer.Droid.Services;
using WorkTimer.Interface;
using Xamarin.Forms;

[assembly: Dependency(typeof(Android_SQLite))]
namespace WorkTimer.Droid.Services
{
    public class Android_SQLite : IDatabaseService
    {
        public SQLiteAsyncConnection GetConnection()
        {
            var sqliteFilename = "WorkTimer.db3";
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // Documents folder
            var path = Path.Combine(documentsPath, sqliteFilename); 
            if (!File.Exists(path)) File.Create(path);
            var plat = new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid();
            var connectionString = new SQLiteConnectionString(path, false);
            var synchronousConn = new SQLiteConnectionWithLock(plat, connectionString);
            var conn = new SQLiteAsyncConnection(() => synchronousConn);
            // Return the database connection 
            return conn;
        }
    }
}