using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using WPF.Admin.Models;
using WPF.Admin.Themes.Converter;

namespace PressMachineMainModeules.Models;

public partial class SystemConfigModel : BindableBase {
    [JsonPropertyName("Ip")]
    [ObservableProperty] private string? _Ip;
    [JsonPropertyName("Ip1")]
    [ObservableProperty] private string? _Ip01;
    [JsonPropertyName("Ip2")]
    [ObservableProperty] private string? _Ip02;
    [JsonPropertyName("Ip3")]
    [ObservableProperty] private string? _Ip03;
    [JsonPropertyName("Com")]
    [ObservableProperty] private string? _Com;
    [JsonPropertyName("DataFolder")]
    [ObservableProperty] private string? _DataFolder;
    [JsonPropertyName("UsingCodeList")]
    [ObservableProperty] private bool _UsingCodeList;

    [JsonPropertyName("PreAdd")]
    [ObservableProperty] private float _压机传感器补偿;

    /// <summary>
    /// 防护门屏蔽
    /// </summary>
    [JsonPropertyName("DoorStatus")]
    [ObservableProperty] private bool _DoorStatus;

    /// <summary>
    /// 光幕屏蔽
    /// </summary>
    [JsonPropertyName("LightCurtainStatus")]
    [ObservableProperty] private bool _LightCurtainStatus;

    /// <summary>
    /// 蜂鸣器屏蔽
    /// </summary>
    [JsonPropertyName("BuzzerStatus")]
    [ObservableProperty] private bool _BuzzerStatus;

    /// <summary>
    /// PC结果屏蔽
    /// </summary>
    [JsonPropertyName("PCResultStatus")]
    [ObservableProperty] private bool _PCResultStatus;

    /// <summary>
    /// NG续压屏蔽
    /// </summary>
    [JsonPropertyName("NGConStatus")]
    [ObservableProperty] private bool _NGConStatus;


    [JsonPropertyName("Danger")]
    [ObservableProperty] private bool _安全互锁屏蔽;

    /// <summary>
    /// 扫码屏蔽
    /// </summary>
    [JsonPropertyName("CodeStatus")]
    [ObservableProperty] private bool _CodeStatus;


    /// <summary>
    /// 远程本地控制
    /// </summary>
    [JsonPropertyName("InterStatus")]
    [ObservableProperty] private bool _InterStatus;


    private static string _file = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
        "Config","SystemManager.json");
    public static SystemConfigModel Create() {
        if (System.IO.File.Exists(_file))
        {
            return SerializeHelper.Deserialize<SystemConfigModel>(_file);
        }
        else
        {
            var cacheSystemConfig = new SystemConfigModel()
            {
                Ip = "192.168.0.10",
                Ip01 = "192.168.0.10",
                Ip02 = "192.168.0.10",
                Ip03 = "192.168.0.10",
                Com = "COM1",
                DataFolder = "D://Data",
                UsingCodeList = false,
                
            };
            SerializeHelper.Serialize(_file, cacheSystemConfig);
            return cacheSystemConfig;
        }

       
    }

    public void Save() {       
        SerializeHelper.Serialize(_file, this);
    }
}