using MvvmNano;
using MvvmNano.Forms;
using WorkTimer.ViewModel;
using Xamarin.Forms; 
using Xamarin.Forms.Xaml;

namespace WorkTimer.Page
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage  
    {
        public MainPage()
        {
            InitializeComponent();

            //Manually set view model
            SetViewModel(MvvmNanoIoC.Resolve<MainViewModel>()); 
        } 
    }
}
