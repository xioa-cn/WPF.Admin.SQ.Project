using CommunityToolkit.Mvvm.Messaging;
using WPF.Admin.Models.Models;
using WPF.Admin.Service.Services;

namespace PressMachineMainModeules.Models
{
   

    public static class AlarmToUiHelper
    {
        public static void Send(this AlarmToUIModels model)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                WeakReferenceMessenger.Default.Send(model);
            });
        }
    }
    
}
