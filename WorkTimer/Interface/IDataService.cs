using System;
using System.Threading.Tasks;
using WorkTimer.Model;

namespace WorkTimer.Interface
{
    public interface IDataService
    {
        Task Initialize();

        event EventHandler<WorkDay> DayUpdatedEvent; 

        Task<WorkDay> AddDay(WorkDay workDay);

        Task<WorkWeek> AddWeek(WorkWeek workWeek);

        Task<WorkDay> GetDay(DateTimeOffset date);

        Task<WorkWeek> GetWeek(DateTimeOffset date);

        Task StartDay(WorkDay day);

        Task EndDay(WorkDay day);

        Task StartBreak(WorkDay day);

        Task EndBreak(WorkDay day);
    }
}