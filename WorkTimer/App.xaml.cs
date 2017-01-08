using System;
using System.Threading.Tasks;
using MvvmNano;
using MvvmNano.Forms;
using MvvmNano.Ninject;
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

            //Task.Run(async () => await Setup());
        }

        private static void SetUpDependencies()
        {
            MvvmNanoIoC.Register<IDataService, DataService>();
            MvvmNanoIoC.Register<IWorkManager, WorkManager>(); 
        }

        private async Task Setup()
        {
            //await MvvmNanoIoC.Resolve<IDataService>().Initialize(); 
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
