using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SQLite.Net;
using SQLite.Net.Async;
using SQLiteNetExtensionsAsync.Extensions;
using WorkTimer.Exceptions;
using WorkTimer.ExtensionMethods;
using WorkTimer.Interface;
using WorkTimer.Model;
using Xamarin.Forms;

namespace WorkTimer.Implementation
{
    public class DataService : IDataService
    {
        private SQLiteAsyncConnection _databaseConnection;

        public event EventHandler<WorkDay> DayUpdatedEvent;
        public event EventHandler<WorkDay> DayAddedEvent;
        public event EventHandler<WorkWeek> WeekAddedEvent;
        public event EventHandler<WorkWeek> WeekUpdatedEvent;

        public DataService()
        {
            _databaseConnection = DependencyService.Get<IDatabaseService>().GetConnection(); 
        }

        public async Task Initialize()
        {
            await _databaseConnection.DeleteAllAsync<WorkWeek>();
            await _databaseConnection.DeleteAllAsync<WorkDay>();
            await _databaseConnection.DeleteAllAsync<Break>();


            await _databaseConnection.CreateTableAsync<Break>();
            await _databaseConnection.CreateTableAsync<WorkDay>(); 
            await _databaseConnection.CreateTableAsync<WorkWeek>(); 
        }

        public async Task<WorkDay> AddDay(WorkDay workDay)
        {  
            //Get the week, week has to be created bevore a day can get added.
            WorkWeek workWeek; 

            workWeek = await GetWeek(workDay.Date); 

            //Check if a day for this date already exists in the week.
            foreach (WorkDay day in workWeek.WorkDays)
            {
                if (day.Date.IsSameDay(workDay.Date))
                    throw new Exception($"Day for {day.Date:d} does already exist.");
            }
            workDay.Date = workDay.Date.AddHours(1); //Workaround for a bug that decreases the date for one hour
            workWeek.WorkDays.Add(workDay);
            await _databaseConnection.InsertAsync(workDay);
            await _databaseConnection.UpdateWithChildrenAsync(workWeek);

            FireDayAddedEvent(workDay);
            FireWeekUpdatedEvent(workWeek);

            return workDay;
        }

        public async Task<WorkWeek> AddWeek(WorkWeek workWeek)
        {
            workWeek.FirstDateOfWeek = workWeek.FirstDateOfWeek.AddHours(1); //Workaround for a bug that decreases the date for one hour
            foreach (WorkDay workDay in workWeek.WorkDays)
            {
                workDay.Date = workDay.Date.AddHours(1); //Workaround for a bug that decreases the date for one hour
            }

            await _databaseConnection.InsertWithChildrenAsync(workWeek); 
            FireWeekAddedEvent(workWeek);
            foreach (WorkDay workDay in workWeek.WorkDays)
            {
                FireDayAddedEvent(workDay);
            }
            return workWeek;
        }

        public async Task<WorkDay> GetDay(DateTimeOffset date)
        {
            try
            {
                var workDays = await _databaseConnection.GetAllWithChildrenAsync<WorkDay>();
                var workDay = workDays.First(o => o.Date.IsSameDay(date));
                await _databaseConnection.GetChildrenAsync(workDay);
                return workDay;
            }
            catch (SQLiteException sqLiteException)
            {
                Debug.WriteLine(sqLiteException);
                throw sqLiteException;
            }
            catch (Exception)
            {
                throw new DayDoesNotExistException();
            } 
        }

        public async Task<WorkWeek> GetWeek(DateTimeOffset date)
        {
            try
            {
                var workWeeks = await _databaseConnection.GetAllWithChildrenAsync<WorkWeek>(); 
                var workWeek = workWeeks.First(o => o.FirstDateOfWeek.IsSameWeek(date));
                await _databaseConnection.GetChildrenAsync(workWeek);
                return workWeek;
            }
            catch (SQLiteException sqLiteException)
            {
                Debug.WriteLine(sqLiteException);
                throw sqLiteException;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                throw new WeekDoesNotExistException();
            }
        } 

        public async Task StartDay(WorkDay workDay)
        { 
            if (workDay == null) throw new ArgumentNullException();
            workDay.StartWork();
            await _databaseConnection.UpdateAsync(workDay);
            FireDayUpdatedEvent(workDay);
        }

        public async Task EndDay(WorkDay workDay)
        { 
            if (workDay == null) throw new ArgumentNullException();
            workDay.EndWork();
            await _databaseConnection.UpdateAsync(workDay);
            FireDayUpdatedEvent(workDay);
        }

        public async Task StartBreak(WorkDay workDay)
        { 
            if (workDay == null) throw new ArgumentNullException();

            if (workDay.IsInBreak()) throw new AlreadyInBreakException(); 

            var newBreak = new Break()
            {
                Start = DateTimeOffset.Now
            };
            newBreak.WorkDayId = workDay.Id;

            await _databaseConnection.InsertAsync(newBreak);

            workDay.Breaks.Add(newBreak);
            
            //newBreak.WorkDay = workDay;
            await _databaseConnection.UpdateWithChildrenAsync(workDay);
            FireDayUpdatedEvent(workDay);
        }

        public async Task EndBreak(WorkDay workDay)
        { 
            if (workDay == null) throw new ArgumentNullException();
            
            if(!workDay.IsInBreak()) throw new NotInBreakException();

            workDay.LastBreak.End = DateTimeOffset.Now;

            await _databaseConnection.UpdateWithChildrenAsync(workDay.LastBreak);

            FireDayUpdatedEvent(workDay);
        }

        private void FireDayAddedEvent(WorkDay workDay)
        {
            InvokeEventOnMainThread(DayAddedEvent, workDay);
            Debug.WriteLine("Day: " + workDay.Date.ToString("d") + " added.");
        }

        private void FireWeekAddedEvent(WorkWeek workWeek)
        {
            InvokeEventOnMainThread(WeekAddedEvent, workWeek);
            Debug.WriteLine("Week: " + workWeek.FirstDateOfWeek.ToString("d") + " added.");
        }

        private void FireDayUpdatedEvent(WorkDay workDay)
        {
            InvokeEventOnMainThread(DayUpdatedEvent, workDay);
            Debug.WriteLine("Day: " + workDay.Date.ToString("d") + " updated.");
        }

        private void FireWeekUpdatedEvent(WorkWeek workWeek)
        {
            InvokeEventOnMainThread(WeekUpdatedEvent, workWeek);
            Debug.WriteLine("Week: " + workWeek.FirstDateOfWeek.ToString("d") + " updated.");
        }

        private void InvokeEventOnMainThread<TParameter>(EventHandler<TParameter> eventHandler, TParameter parameter)
        {
            Device.BeginInvokeOnMainThread(()=> eventHandler?.Invoke(this, parameter));
        }

        public async Task<List<WorkWeek>> GetAllWeeks()
        {
            try
            {
                var workWeeks = await _databaseConnection.GetAllWithChildrenAsync<WorkWeek>();
                return workWeeks;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return new List<WorkWeek>();
            }
        } 
    }
}