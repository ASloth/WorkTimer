using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MvvmNano;
using WorkTimer.Interface;
using WorkTimer.Model;
using WorkTimer.Page;
using Xamarin.Forms;

namespace WorkTimer.ViewModel
{
    public class HistoryViewModel : MvvmNanoViewModel
    {
        private readonly IWorkManager _workManager;
        private readonly IDataService _dataService;
        private WorkWeek _selectedItem;

        public ObservableCollection<Grouping<string, WorkWeek>> ItemsGrouped { get; set; } 

        private List<WorkWeek> _workWeeks; 

        public WorkWeek SelectedItem
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

        public HistoryViewModel(IWorkManager workManager, IDataService dataService)
        {
            _workManager = workManager;
            _dataService = dataService;

            _dataService.WeekAddedEvent += WeekAdded;

            Task.Run(async () => await GetData());
        }

        private async void WeekAdded(object sender, WorkWeek workWeek)
        {
            await GetData();
        }

        private async Task GetData()
        {
            _workWeeks = await _workManager.GetAllWeeks();
            SortData();
        }

        private void SortData()
        {
            var sorted = from item in _workWeeks
                         orderby item.FirstDateOfWeek
                         group item by item.FirstDateOfWeek.Year.ToString() into itemGroup
                         select new Grouping<string, WorkWeek>(itemGroup.Key, itemGroup);

            ItemsGrouped = new ObservableCollection<Grouping<string, WorkWeek>>(sorted);

            NotifyPropertyChanged(nameof(ItemsGrouped));
        }

        public override void Dispose()
        {
            _dataService.WeekAddedEvent -= WeekAdded;
            base.Dispose(); 
        }

        Task ItemSet(WorkWeek item)
        {
            return NavigateToAsync<WorkWeekDetailViewModel, WorkWeek>(item);
        }
    }

    public class Grouping<K, T> : ObservableCollection<T>
    {
        public K Key { get; private set; }

        public Grouping(K key, IEnumerable<T> items)
        {
            Key = key;
            foreach (var item in items)
                this.Items.Add(item);
        }
    }
}