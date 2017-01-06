using MvvmNano;
using MvvmNano.Forms;
using MvvmNano.Ninject;
using WorkTimer.Page;
using WorkTimer.ViewModel;

namespace WorkTimer
{
    public partial class App 
    {
        public App()
        {
            InitializeComponent();   
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
             
            MainPage = new MvvmNanoNavigationPage(GetTabbedPageFor<MainTabbedViewModel>());
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
