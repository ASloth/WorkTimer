using System;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using MvvmNano;
using WorkTimer.ExtensionMethods;
using WorkTimer.Interface;
using WorkTimer.Model;
using Xamarin.Forms;

namespace WorkTimer.ViewModel
{
    public class MainViewModel : WorkDayDetailViewModelBase<WorkDay>
    { 
        public bool CanEndWork => GetCanEndWork();  

        //Commands
        public MvvmNanoCommand StartWorkCommand => new MvvmNanoCommand(async ()=> await StartWork());

        public MvvmNanoCommand EndWorkCommand => new MvvmNanoCommand(async () => await EndWork());

        public MainViewModel(IWorkManager workManager) : base(workManager)
        {
            Task.Run(async () =>
            {
                try
                {
                    await MvvmNanoIoC.Resolve<IDataService>().Initialize();
                    var workDay = await _workManager.GetDay(DateTimeOffset.Now.Date);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        SetDay(workDay);
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            });  
        }  

        #region work methods

        private async Task StartWork()
        {
            await _workManager.StartDay(TodaysWorkDay);
            NotifyPropertyChanged(nameof(StartTimeFormatted));
            NotifyPropertyChanged(nameof(CanStartWork));
            NotifyPropertyChanged(nameof(CanEndWork));
            NotifyPropertyChanged(nameof(IsBreakPossible));
        }

        private async Task EndWork()
        {
            await _workManager.EndDay(TodaysWorkDay);
            NotifyPropertyChanged(nameof(EndTimeFormatted));
            NotifyPropertyChanged(nameof(CanEndWork));
            NotifyPropertyChanged(nameof(IsBreakPossible)); 
        } 

        private bool GetCanEndWork()
        {
            return TodaysWorkDay != null && !TodaysWorkDay.Start.IsNull();
        }

        #endregion  

        protected override void BreakStateChanged(bool inBreak)
        {
            base.BreakStateChanged(inBreak);
            if (inBreak && !TodaysWorkDay.IsInBreak()) //Was not in break, start break.
            {
                Task.Run(async () => await StartBreak());
            }
            if (!inBreak && TodaysWorkDay.IsInBreak()) //Was in break, end break.
            {
                Task.Run(async () => await EndBreak());
            }
        }

        private async Task StartBreak()
        {
            await _workManager.StartBreak(TodaysWorkDay);  
            NotifyPropertyChanged(nameof(IsInBreak));
        }

        private async Task EndBreak()
        {
            await _workManager.EndBreak(TodaysWorkDay);
            NotifyPropertyChanged(nameof(IsInBreak));
        }
    }
}