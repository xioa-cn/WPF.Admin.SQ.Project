using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using WPF.Admin.Models;

namespace PressMachineMainModeules.Models {
    public partial class AutoCheckCodeParameterContent : BindableBase {
        public int Step { get; set; }
        [ObservableProperty] private string _key;
        [ObservableProperty] private string _value = "未设置";
    }

    public partial class AutoCheckCodeParameterModel : BindableBase {
        [ObservableProperty] private ObservableCollection<AutoCheckCodeParameterContent> _ParameterMainModels;
        [ObservableProperty] private ObservableCollection<AutoCheckCodeParameterContent> _ParameterPartialModels;
    }
}