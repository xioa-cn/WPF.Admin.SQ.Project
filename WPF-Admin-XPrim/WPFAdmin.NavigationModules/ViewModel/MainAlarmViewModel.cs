using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using WPF.Admin.Models.Models;
using WPF.Admin.Models;
using WPF.Admin.Service.Services;

namespace WPFAdmin.NavigationModules.ViewModel
{
    public partial class MainAlarmViewModel : BindableBase
    {
        public static MainAlarmViewModel Main { get; set; } = new MainAlarmViewModel();
        [ObservableProperty] private bool _IsAlertActive = false;

        public MainAlarmViewModel()
        {
            WeakReferenceMessenger.Default.Register<ApplicationAlarm>(this, AlarmShow);
        }

        private void AlarmShow(object recipient, ApplicationAlarm message)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                this.IsAlertActive = message.Alarm;
            });
        }
    }
}
