using System.Threading.Tasks;
using MvvmNano;

namespace WorkTimer.ViewModel
{
    public class MainTabbedViewModel : MvvmNanoViewModel
    {
        public MvvmNanoCommand OpenSettingsCommand => new MvvmNanoCommand(async ()=> await OpenSettings());

        private Task OpenSettings()
        {
            return NavigateToAsync<SettingsViewModel>();
        }
    }
}