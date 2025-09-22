using CommunityToolkit.Mvvm.ComponentModel;
using WPF.Admin.Models;

namespace PressMachineMainModeules.Models {
    public partial class AutoWorkwearBindingParam : BindableBase {
        [ObservableProperty] private string _workwearName;
        [ObservableProperty] private string _paramerterName = "未录入";
    }
}