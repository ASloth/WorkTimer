using System;
using System.IO;
using SQLite.Net;
using SQLite.Net.Async;
using WorkTimer.iOS.Services;
using WorkTimer.Interface;
using Xamarin.Forms;

[assembly: Dependency(typeof(iOS_SQLite))]
namespace WorkTimer.iOS.Services
{
    public class iOS_SQLite : IDatabaseService
    { 
        #region ISQLite implementation
        public SQLiteAsyncConnection GetConnection()
        {
            var sqliteFilename = "WorkTimer.db3";
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            string libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder
            var path = Path.Combine(libraryPath, sqliteFilename);

            // This is where we copy in the prepopulated database 
            if (!File.Exists(path))
            {
                File.Create(path);
            }

            var plat = new SQLite.Net.Platform.XamarinIOS.SQLitePlatformIOS();
            var connectionString = new SQLiteConnectionString(path, false);
            var synchronousConn = new SQLiteConnectionWithLock(plat, connectionString);
            var conn =new SQLiteAsyncConnection(()=>synchronousConn);

            // Return the database connection 
            return conn;
        }
        #endregion
    }
}