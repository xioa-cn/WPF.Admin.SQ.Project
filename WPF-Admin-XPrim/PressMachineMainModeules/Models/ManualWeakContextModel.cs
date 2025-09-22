using CommunityToolkit.Mvvm.ComponentModel;
using WPF.Admin.Models;

namespace PressMachineMainModeules.Models
{
    public partial class ManualWeakContextModel : BindableBase
    {
        [ObservableProperty] private float _Position;
        [ObservableProperty] private float _Press;
        [ObservableProperty] private float _Speed;
        [ObservableProperty] private float _ManualProdPress;
        [ObservableProperty] private float _JogHigh;
        [ObservableProperty] private float _JogLow;

        public void SetSelf(ManualWeakContextModel dto)
        {
            this.Position = dto.Position;
            this.Press = dto.Press;
            this.Speed = dto.Speed;
            this.ManualProdPress = dto.ManualProdPress;
            this.JogHigh = dto.JogHigh;
            this.JogLow = dto.JogLow;
        }
    }
}
