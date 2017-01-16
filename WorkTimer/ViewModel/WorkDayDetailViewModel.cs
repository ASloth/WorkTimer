using MvvmNano;
using WorkTimer.Interface;
using WorkTimer.Model;

namespace WorkTimer.ViewModel
{
    public class WorkDayDetailViewModel : WorkDayDetailViewModelBase<WorkDay>
    {
        public WorkDayDetailViewModel(IWorkManager workManager) : base(workManager)
        {
            
        } 

        public string Title { get; private set; }

        public override void Initialize(WorkDay parameter)
        {
            base.Initialize(parameter);
            SetDay(parameter);

            Title = $"{TodaysWorkDay.Date:d} {TodaysWorkDay.DayOfWeek}";
        }
    }
}