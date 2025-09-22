using CommunityToolkit.Mvvm.ComponentModel;
using WPF.Admin.Models;
using WPF.Admin.Service.Services;

namespace PressMachineMainModeules.Models
{
    public class PressMachineStatusManager
    {
        public int Num { get; set; }
        public PressMachineStatus Status { get; set; }
    }


    public partial class MaualShowFloatData : ObservableObject
    {
        [ObservableProperty] private float? _压力1 = 0;
        [ObservableProperty] private float? _频率1 = 0;
        [ObservableProperty] private float? _压力2 = 0;
        [ObservableProperty] private float? _频率2 = 0;
        [ObservableProperty] private float? _压力3 = 0;
        [ObservableProperty] private float? _频率3 = 0;

        public void Setself(MaualShowFloatData dto)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                this.压力1 = dto.压力1;
                this.压力2 = dto.压力2;
                this.压力3 = dto.压力3;
                this.频率1 = dto.频率1;
                this.频率2 = dto.频率2;
                this.频率3 = dto.频率3;
            });
        }
    }


    public partial class PressMachineStatus : BindableBase
    {
        [ObservableProperty] private bool _电缸启用;
        [ObservableProperty] private bool _电缸屏蔽;
        [ObservableProperty] private bool _手动模式;
        [ObservableProperty] private bool _自动模式;
        [ObservableProperty] private bool _自动启动;
        [ObservableProperty] private bool _自动停止;

        public void Setself(PressMachineStatus dto)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                this.手动模式 = dto.手动模式;
                this.自动模式 = dto.自动模式;
                this.自动启动 = dto.自动启动;
                this.自动停止 = dto.自动停止;
                this.电缸启用 = dto.电缸启用;
                this.电缸屏蔽 = dto.电缸屏蔽;
            });


        }
    }
}
