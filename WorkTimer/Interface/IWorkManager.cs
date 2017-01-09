using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkTimer.Model;

namespace WorkTimer.Interface
{
    public interface IWorkManager
    {
        event EventHandler<WorkDay> DayUpdatedEvent;

        /// <summary>
        /// Returns the <see cref="WorkWeek"/> object for the given date. Creates a new week object if non exists in the database. 
        /// Will use the current user settings if a new week gets created.
        /// </summary>
        Task<WorkWeek> GetWeek(DateTimeOffset date);

        /// <summary>
        /// Returns all weeks from the database.
        /// </summary> 
        Task<List<WorkWeek>> GetAllWeeks();

        /// <summary>
        /// Returns the <see cref="WorkDay"/> object for the given date. Creates a new date and week if non exists in the database.
        /// Will use the current user settigs if a new week gets created.
        /// </summary> 
        Task<WorkDay> GetDay(DateTimeOffset date);

        /// <summary>
        /// Starts the given day and updates the database.
        /// </summary> 
        Task StartDay(WorkDay workDay);

        /// <summary>
        /// Ends the given day and updates the database.
        /// </summary> 
        Task EndDay(WorkDay workDay);

        /// <summary>
        /// Starts a new break for the given date and updates the database.
        /// </summary> 
        Task StartBreak(WorkDay workDay);

        /// <summary>
        /// Ends the last break for the given date und updates the database.
        /// </summary> 
        Task EndBreak(WorkDay workDay);
    }
}