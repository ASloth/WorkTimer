using System;
using System.IO;
using Windows.Storage;
using SQLite.Net;
using SQLite.Net.Async;
using WorkTimer.Interface;
using WorkTimer.UWP.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(UWP_SQLite))]
namespace WorkTimer.UWP.Services
{
    public class UWP_SQLite : IDatabaseService
    {
        public SQLiteAsyncConnection GetConnection()
        { 
            var plat = new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT();
            SQLiteAsyncConnection conn = null;
             
            var path = GetFilePath(ApplicationData.Current.LocalCacheFolder.Path);
            var connectionString = new SQLiteConnectionString(path, false);
            SQLiteConnectionWithLock synchronousConn;

            try
            {
                synchronousConn = new SQLiteConnectionWithLock(plat, connectionString);
                conn = new SQLiteAsyncConnection(()=> synchronousConn);
            } 
            catch(Exception)
            {
                try
                {
                    //Try to open the documents folder if the local cache folder is not available.
                    path = GetFilePath(KnownFolders.DocumentsLibrary.Path);
                    connectionString = new SQLiteConnectionString(path, false);
                    synchronousConn = new SQLiteConnectionWithLock(plat, connectionString);

                    conn = new SQLiteAsyncConnection(() => synchronousConn);
                }
                catch (Exception)
                { 
                    //It is possible that the access to the documents library is denied, catch it.
                    //TODO Handle exception
                }
            }
            
            // Return the database connection 
            return conn;
        }

        string GetFilePath(string folderPath)
        {
            var sqliteFilename = "WorkTimer.db3"; 
            var path = Path.Combine(folderPath, sqliteFilename);

            // This is where we copy in the prepopulated database 
            if (!File.Exists(path))
            {
                File.Create(path);
            }

            return path;
        } 
    }
}