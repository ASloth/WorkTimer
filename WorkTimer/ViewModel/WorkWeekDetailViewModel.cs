using System.Threading.Tasks;
using MvvmNano;
using WorkTimer.Model;
using Xamarin.Forms;

namespace WorkTimer.ViewModel
{
    public class WorkWeekDetailViewModel : MvvmNanoViewModel<WorkWeek>
    {
        private WorkDay _selectedItem;
        public WorkWeek WorkWeek { get; set; }

        public WorkDay SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                if (_selectedItem != null)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await ItemSet(value);
                    });
                }
            }
        }

        public override void Initialize(WorkWeek parameter)
        {
            base.Initialize(parameter);

            WorkWeek = parameter;
        }

        Task ItemSet(WorkDay item)
        {
            return NavigateToAsync<WorkDayDetailViewModel, WorkDay>(item);
        }
    }
}