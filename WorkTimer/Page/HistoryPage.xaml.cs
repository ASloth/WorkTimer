using MvvmNano;
using WorkTimer.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkTimer.Page
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoryPage  
    {
        public HistoryPage()
        {
            InitializeComponent();

            //Manually set view model
            SetViewModel(MvvmNanoIoC.Resolve<HistoryViewModel>());
        }

        private void ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
                HistoryListView.SelectedItem = null;
        }
    } 
}
