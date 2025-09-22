using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using SQ.Project.Component;
using WPF.Admin.Models;

namespace SQ.Project.ViewModels
{
    public partial class CarouselContent : ObservableObject
    {
        [ObservableProperty] private string _name = string.Empty;

        [ObservableProperty] private object _Page;

        [ObservableProperty] private bool _isActive;
    }

    public partial class SQVisualHomeViewModel : BindableBase, IDisposable
    {
        [ObservableProperty] private bool _isAutoPlaying = true;

        public ObservableCollection<CarouselContent> Items { get; set; }

        private DispatcherTimer? _timer;
        
        private readonly CodeCollectionViewModel _codeCollectionViewModel = new CodeCollectionViewModel();
        private readonly StationFirstViewModel _stationFirstViewModel = new StationFirstViewModel();

        public SQVisualHomeViewModel()
        {
            Items = new ObservableCollection<CarouselContent>(
               [
                  new CarouselContent()
                  {
                      Name= "批量物料采集",
                      IsActive = true,
                      Page = new CodeCollection(_codeCollectionViewModel)
                  },
                  new CarouselContent()
                  {
                      Name= "OP10-产品上线",
                      Page = new StationFirst(_stationFirstViewModel)
                  },
                 
               ]);
        }

        [RelayCommand]
        private void ToggleAutoPlay()
        {
            IsAutoPlaying = !IsAutoPlaying;
            if (IsAutoPlaying)
                _timer?.Start();
            else
                _timer?.Stop();
        }


        public void InitTimer()
        {
            // 初始化计时器
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            _timer.Tick += Timer_Tick;
            if (IsAutoPlaying)
                _timer?.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            NextCommand.Execute(null);
        }

        [RelayCommand]
        private void Next()
        {
            int currentIndex = GetCurrentIndex();
            int nextIndex = (currentIndex + 1) % Items.Count;
            SetActiveItems(nextIndex);
        }
        private void SetActiveItems(int index)
        {
            foreach (var image in Items)
            {
                image.IsActive = false;
            }

            Items[index].IsActive = true;
        }
        private int GetCurrentIndex()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].IsActive)
                    return i;
            }

            return 0;
        }

        [RelayCommand]
        private void Previous()
        {
            int currentIndex = GetCurrentIndex();
            int previousIndex = (currentIndex - 1 + Items.Count) % Items.Count;
            SetActiveItems(previousIndex);
        }


        void Dispose(bool disposing)
        {
            // 清理托管资源
            if (_timer is not null)
            {
                _timer.Stop();
                _timer.Tick -= Timer_Tick;
                _timer = null;
            }

            IsAutoPlaying = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~SQVisualHomeViewModel()
        {
            this.Dispose();
        }
    }
}
