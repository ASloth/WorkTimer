using Xamarin.Forms;

namespace WorkTimer.Page
{
    public partial class WorkWeekDetailPage  
    {
        public WorkWeekDetailPage()
        {
            InitializeComponent();
        }

        private void WorkDaySelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (WorkDaysListView.SelectedItem != null)
                WorkDaysListView.SelectedItem = null;
        }
    }
}
