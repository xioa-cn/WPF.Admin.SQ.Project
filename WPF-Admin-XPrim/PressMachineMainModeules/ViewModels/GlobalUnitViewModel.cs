using CommunityToolkit.Mvvm.ComponentModel;
using WPF.Admin.Models;

namespace PressMachineMainModeules.ViewModels
{
    public partial class GlobalUnitViewModel : BindableBase
    {
        [ObservableProperty] private string _title = nameof(GlobalUnitViewModel);

        [ObservableProperty] private string? _speedUnitName;
        [ObservableProperty] private string? _positionUnitName;
        [ObservableProperty] private string? _pressUnitName;

        public static GlobalUnitViewModel Insance { get; set; }

        static GlobalUnitViewModel()
        {
            Insance = new GlobalUnitViewModel();
            if (PressMachineParamsViewModel.PressMachineParam is not null)
            {
                Insance.SpeedUnitName = PressMachineParamsViewModel.PressMachineParam.SpeedUnitName;
                Insance.PositionUnitName = PressMachineParamsViewModel.PressMachineParam.PositionUnitName;
                Insance.PressUnitName = PressMachineParamsViewModel.PressMachineParam.PressUnitName;
            }
            else
            {
                Insance.SpeedUnitName = "mm/s";
                Insance.PositionUnitName = "mm";
                Insance.PressUnitName = "N";
            }
            
        }
    }
}
