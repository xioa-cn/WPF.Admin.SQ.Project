namespace WPF.Admin.Themes.Monitor;

/// <summary>
/// 埋点事件类型
/// </summary>
public enum TrackEventType
{
    Click,      // 点击事件
    PageView,   // 页面访问
    Input,      // 输入行为
    Operation,  // 业务操作
    Exception   // 异常情况
}