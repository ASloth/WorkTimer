using MvvmNano;
using WorkTimer.Model;

namespace WorkTimer.ViewModel
{
    public class WorkDayDetailViewModel : MvvmNanoViewModel<WorkDay>
    {
        public WorkDay WorkDay { get; set; }

        public string Title { get; private set; }

        public override void Initialize(WorkDay parameter)
        {
            base.Initialize(parameter);
            WorkDay = parameter;

            Title = $"{WorkDay.Date:d} {WorkDay.DayOfWeek}";
        }
    }
}