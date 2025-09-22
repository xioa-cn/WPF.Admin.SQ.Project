using CommunityToolkit.Mvvm.ComponentModel;
using WPF.Admin.Models;

namespace PressMachineMainModeules.Models;

public partial class IOPointVal : BindableBase
{
    [ObservableProperty] private bool _自动模式 = true;
    [ObservableProperty] private bool _自动状态 = true;
    [ObservableProperty] private bool _原点触发 = true;
    [ObservableProperty] private bool _报警中 = true;
    [ObservableProperty] private bool _曲线记录中 = true;

    [ObservableProperty] private bool _压机原点 = true;
    [ObservableProperty] private bool _X轴原点 = true;

    [ObservableProperty] private bool _压机待机位 = true;
    [ObservableProperty] private bool _X轴一次待机位 = true;
    [ObservableProperty] private bool _X轴待机位 = true;

    [ObservableProperty] private bool _X轴压装位置一 = true;
    [ObservableProperty] private bool _X轴压装位置二 = true;
    [ObservableProperty] private bool _X轴压装位置三 = true;
    [ObservableProperty] private bool _X轴压装位置四 = true;

    [ObservableProperty] private float _X轴手动速度;
    [ObservableProperty] private float _压机位置指示;
    [ObservableProperty] private float _压机压力指示;
    [ObservableProperty] private float _压机速度指示;
    [ObservableProperty] private float _X轴位置指示;
    [ObservableProperty] private float _手动速度;
    [ObservableProperty] private float _手动保护;




    public void SetSelf(IOPointVal ioPoint)
    {
        // 状态标志
        this.自动模式 = ioPoint.自动模式;
        this.自动状态 = ioPoint.自动状态;
        this.原点触发 = ioPoint.原点触发;
        this.报警中 = ioPoint.报警中;
        this.曲线记录中 = ioPoint.曲线记录中;

        // 原点状态
        this.压机原点 = ioPoint.压机原点;
        this.X轴原点 = ioPoint.X轴原点;

        // 待机位状态
        this.压机待机位 = ioPoint.压机待机位;
        this.X轴一次待机位 = ioPoint.X轴一次待机位;
        this.X轴待机位 = ioPoint.X轴待机位;

        // X轴压装位置
        this.X轴压装位置一 = ioPoint.X轴压装位置一;
        this.X轴压装位置二 = ioPoint.X轴压装位置二;
        this.X轴压装位置三 = ioPoint.X轴压装位置三;
        this.X轴压装位置四 = ioPoint.X轴压装位置四;

        // 速度和指示值
        this.X轴手动速度 = ioPoint.X轴手动速度;
        this.压机位置指示 = ioPoint.压机位置指示;
        this.压机压力指示 = ioPoint.压机压力指示;
        this.压机速度指示 = ioPoint.压机速度指示;
        this.X轴位置指示 = ioPoint.X轴位置指示;

        // 手动控制参数
        this.手动速度 = ioPoint.手动速度;
        this.手动保护 = ioPoint.手动保护;
    }
}