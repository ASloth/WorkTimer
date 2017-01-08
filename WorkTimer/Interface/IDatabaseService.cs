using SQLite.Net;
using SQLite.Net.Async;

namespace WorkTimer.Interface
{
    public interface IDatabaseService
    {
        /// <summary>
        /// Connect to a local sqlight database on the device.
        /// </summary>
        /// <returns></returns>
        SQLiteAsyncConnection GetConnection();
    }
}