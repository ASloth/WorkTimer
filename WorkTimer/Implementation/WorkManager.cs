using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkTimer.Exceptions;
using WorkTimer.ExtensionMethods;
using WorkTimer.Interface;
using WorkTimer.Model;

namespace WorkTimer.Implementation
{
    public class WorkManager : IWorkManager
    {
        private readonly IDataService _dateService;
        public event EventHandler<WorkDay> DayUpdatedEvent;

        //TODO REPLACE WITH REAL SETTINGS
        private DayOfWeek [] _neededDays = new DayOfWeek[]{DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday};

        public WorkManager(IDataService dateService)
        {
            _dateService = dateService;

            _dateService.DayUpdatedEvent += (sender, day) => FireDayUpdatedEvent(day);
        }

        public async Task<WorkWeek> GetWeek(DateTimeOffset date)
        {
            WorkWeek workWeek;
            try
            {
                workWeek = await _dateService.GetWeek(date);
            }
            catch (WeekDoesNotExistException)
            {
                var firstDayOfGivenWeek = date.GetFirstDayOfWeek();

                //Week did not exist, create a new one.
                workWeek = new WorkWeek(firstDayOfGivenWeek); 

                //Add all default dates 
                foreach (DayOfWeek dayOfWeek in _neededDays)
                {
                    var workDayDate = firstDayOfGivenWeek.AddDays((int)dayOfWeek - 1);
                    var workDay = new WorkDay(workDayDate); 
                    workWeek.WorkDays.Add(workDay);
                }

                workWeek = await _dateService.AddWeek(workWeek);
            }

            return workWeek;
        }

        public async Task<WorkDay> GetDay(DateTimeOffset date)
        {
            WorkDay workDay;
            try
            {
                workDay = await _dateService.GetDay(date);
            } 
            catch (DayDoesNotExistException)
            {
                try
                {
                    workDay = new WorkDay(date);
                    workDay = await _dateService.AddDay(workDay); 
                }
                catch (WeekDoesNotExistException)
                {
                    //Week is missing, create a new week for that day.
                    var workWeek = await GetWeek(date);

                    //Get day out of the week
                    workDay = workWeek.WorkDays.FirstOrDefault(o => o.Date.IsSameDay(date));

                    //Check if day existed
                    if (workDay == null)
                    {
                        //Create a new day for the week.
                        workDay = new WorkDay(date);
                        workDay = await _dateService.AddDay(workDay);
                    }
                }
            }

            return workDay;
        }

        public Task StartDay(WorkDay workDay)
        {
            return _dateService.StartDay(workDay);
        }

        public Task EndDay(WorkDay workDay)
        {
            return _dateService.EndDay(workDay);
        }

        public Task StartBreak(WorkDay workDay)
        {
            return _dateService.StartBreak(workDay);
        }

        public Task EndBreak(WorkDay workDay)
        {
            return _dateService.EndBreak(workDay);
        }

        private void FireDayUpdatedEvent(WorkDay workDay)
        {
            DayUpdatedEvent?.Invoke(this, workDay);
        }

        public Task<List<WorkWeek>> GetAllWeeks()
        {
            return _dateService.GetAllWeeks();
        }
    }
}