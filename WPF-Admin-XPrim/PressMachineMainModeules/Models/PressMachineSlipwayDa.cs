using CommunityToolkit.Mvvm.ComponentModel;

namespace PressMachineMainModeules.Models;

/// <summary>
/// 滑台参数
/// </summary>
public partial class PressMachineSlipwayDa : ObservableObject {
    [ObservableProperty] private float _待机速度 =5;
    [ObservableProperty] private float _待机位置=2;
    
    [ObservableProperty] private float _第一速度=4;
    [ObservableProperty] private float _第一位置=5;
    
    [ObservableProperty] private float _第二速度=3;
    [ObservableProperty] private float _第二位置=8;
    
    [ObservableProperty] private float _第三速度=1;
    [ObservableProperty] private float _第三位置=10;

    [ObservableProperty] private float _第四速度 = 3;
    [ObservableProperty] private float _第四位置 = 12;
}