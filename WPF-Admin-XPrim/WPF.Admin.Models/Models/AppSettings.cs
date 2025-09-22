using System.ComponentModel;
using System.Text.Json.Serialization;

namespace WPF.Admin.Models.Models;

public class AppSettings {
    [JsonPropertyName("Color")]
    [Description("系统主题颜色：")]
    public string Color { get; set; }

    [JsonPropertyName("OpenGarnet")]
    [Description("启动Garnet服务：")]
    public bool OpenGarnet { get; set; }

    [JsonPropertyName("GarnetPort")]
    [Description("Garnet端口：")]
    public int GarnetPort { get; set; }

    [JsonPropertyName("height")]
    [Description("系统Normal高度：")]
    public double Height { get; set; }

    [JsonPropertyName("width")]
    [Description("系统Normal宽度：")]
    public double Width { get; set; }

    [JsonPropertyName("index")]
    [Description("首页登录页切换：")]
    public string? IndexStatus { get; set; }

    [JsonPropertyName("api")]
    [Description("网络请求路由：")]
    public string? ApiBaseUrl { get; set; }

    [JsonPropertyName("auth")]
    [Description("权限限制：")]
    public string? ViewAuthSwitch { get; set; } //Visibility IsEnabled

    [JsonPropertyName("autoOpenNavIndex")]
    [Description("启动时打开路由首项：")]
    public bool AutoOpenNavIndex { get; set; }

    [JsonPropertyName("Theme")]
    [Description("启动时的主题方案：")]
    public string Theme { get; set; }

    [JsonPropertyName("AppName")]
    [Description("启动时系统名称：")]
    public string AppName { get; set; }

    [JsonPropertyName("logDuration")]
    [Description("日志留存时间：")]
    public double logDuration { get; set; }

    [JsonPropertyName("WindowSize")]
    [Description("启动时界面状态：")]
    public string WindowSize { get; set; }

    [JsonPropertyName("GlobalHotKey")]
    [Description("启动全局热键：")]
    public bool GlobalHotKey { get; set; }

    [JsonPropertyName("RouteName")]
    [Description("启动的路由名称：")]
    public string RouteName { get; set; }

    [JsonPropertyName("OpenDblog")]
    [Description("日志同步到数据库：")]
    public bool OpenDblog { get; set; }

    [JsonPropertyName("OpenTrackingManager")]
    [Description("启动系统埋点监控：")]
    public bool OpenTrackingManager { get; set; }

    [JsonPropertyName("OpenAniGrid")]
    [Description("启动系统动画容器：")]
    public bool OpenAniGrid { get; set; }

    [JsonPropertyName("ThemeCursor")]
    [Description("启动系统鼠标：")]
    public bool ThemeCursor { get; set; }

    [JsonPropertyName("ApplicationVersions")]
    [Description("系统的授权启动版本：")]
    public ApplicationVersions ApplicationVersions { get; set; }

    [JsonPropertyName("OpenExternalGrpc")]
    [Description("开启GRPC外部服务：")]
    public bool OpenExternalGrpc { get; set; }

    [JsonPropertyName("OpenAlarmLog")]
    [Description("报警日志到数据库：")]
    public bool OpenAlarmLog { get; set; }

    [JsonPropertyName("IsRememberThePasswordToLogInAutomatically")]
    [Description("自动登录显示界面：")]
    public bool IsRememberThePasswordToLogInAutomatically { get; set; }

    [JsonPropertyName("AuthOutTime")]
    [Description("权限过期时间：")]
    public string AuthOutTime { get; set; }

    [JsonPropertyName("OpenEnvelope")]
    [Description("是否启用包络线：")]
    public bool OpenEnvelope { get; set; }

    public static AppSettings? Default { get; set; }

    public static Action<string>? ActionSaveColor;

    public static ApplicationCms ApplicationCms { get; set; } = ApplicationCms.Normal;

    public static string? ConfigRouter { get; set; }
}