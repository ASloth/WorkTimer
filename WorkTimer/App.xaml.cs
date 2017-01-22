using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MvvmNano;
using MvvmNano.Forms;
using MvvmNano.Ninject;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.LocalNotifications;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Plugin.Vibrate;
using WorkTimer.Implementation;
using WorkTimer.Interface;
using WorkTimer.Page;
using WorkTimer.ViewModel;
using Xamarin.Forms;

namespace WorkTimer
{
    public partial class App 
    {
        public App()
        {
            InitializeComponent();

            MainPage = new ContentPage {Title = "Test"};
        }

        public MvvmNanoTabbedPage<TViewModel> GetTabbedPageFor<TViewModel>() where TViewModel : MvvmNanoViewModel
        {
            var viewModel = MvvmNanoIoC.Resolve<TViewModel>();
            viewModel.Initialize();

            var page = MvvmNanoIoC
                .Resolve<IPresenter>()
                .CreateViewFor<TViewModel>() as MvvmNanoTabbedPage<TViewModel>;

            if (page == null)
            {
                throw new MvvmNanoException($"Could not create a MvvmNanoContentPage for View Model of type {typeof(TViewModel)}.");
            }

            page.SetViewModel(viewModel);

            return page;
        }

        protected override void OnStart()
        {
            base.OnStart();

            SetUpDependencies();

            MainPage = new MvvmNanoNavigationPage(GetTabbedPageFor<MainTabbedViewModel>());

            Task.Run(async () => await Setup());
        }

        private static void SetUpDependencies()
        {
            MvvmNanoIoC.RegisterAsSingleton(CrossLocalNotifications.Current);
            MvvmNanoIoC.RegisterAsSingleton(CrossVibrate.Current);
            MvvmNanoIoC.RegisterAsSingleton(CrossPermissions.Current);
            MvvmNanoIoC.RegisterAsSingleton(CrossGeolocator.Current);
            MvvmNanoIoC.RegisterAsSingleton<ISettingStorage, SettingStorage>();
            MvvmNanoIoC.RegisterAsSingleton<IDataService, DataService>();
            MvvmNanoIoC.RegisterAsSingleton<IWorkManager, WorkManager>(); 
            MvvmNanoIoC.RegisterAsSingleton<ILocationService, LocationService>();
        }

        private async Task Setup()
        {
            try
            {
                await MvvmNanoIoC.Resolve<ILocationService>().StartService();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            } 
        } 

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        protected override IMvvmNanoIoCAdapter GetIoCAdapter()
        {
            return new MvvmNanoNinjectAdapter();
        }
    }
}
