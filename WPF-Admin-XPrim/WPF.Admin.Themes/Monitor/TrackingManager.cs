using System.Diagnostics;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using WPF.Admin.Service.Logger;
using WPF.Admin.Themes.Converter;

namespace WPF.Admin.Themes.Monitor;

/// <summary>
/// 埋点管理器
/// </summary>
public class TrackingManager : IDisposable, IAsyncDisposable
{
    private static TrackingManager _instance;
    public static TrackingManager Instance => _instance ??= new TrackingManager();

    private readonly string _sessionId = Guid.NewGuid().ToString();
    private readonly Queue<TrackEventData> _eventQueue = new Queue<TrackEventData>();
    private readonly Timer _uploadTimer;

    private TrackingManager()
    {
        // 定时上传数据，比如每30秒
        _uploadTimer = new Timer(UploadEvents, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
    }

    /// <summary>
    /// 初始化埋点
    /// </summary>
    public void Initialize()
    {
        // 注册全局事件
        EventManager.RegisterClassHandler(
            typeof(Button),
            Button.ClickEvent,
            new RoutedEventHandler(OnButtonClick),
            true);

        EventManager.RegisterClassHandler(
            typeof(Window),
            FrameworkElement.LoadedEvent,
            new RoutedEventHandler(OnPageLoaded),
            true);
    }

    /// <summary>
    /// 手动埋点方法
    /// </summary>
    public void TrackEvent(string eventName, Dictionary<string, object> properties = null)
    {
        var data = new TrackEventData
        {
            EventType = TrackEventType.Operation,
            PageName = GetCurrentPageName(),
            Properties = properties ?? new Dictionary<string, object>
            {
                ["EventName"] = eventName
            }
        };

        EnqueueEvent(data);
    }

    private void OnButtonClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            var data = new TrackEventData
            {
                EventType = TrackEventType.Click,
                PageName = GetCurrentPageName(),
                ElementId = button.Name,
                ElementType = "Button",
                Properties = new Dictionary<string, object>
                {
                    ["Content"] = button.Content?.ToString(),
                    ["Tag"] = button.Tag?.ToString()
                }
            };

            EnqueueEvent(data);
        }
    }

    private void OnPageLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is Window window)
        {
            var data = new TrackEventData
            {
                EventType = TrackEventType.PageView,
                PageName = window.GetType().Name,
                Properties = new Dictionary<string, object>
                {
                    ["Title"] = window.Title
                }
            };

            EnqueueEvent(data);
        }
    }

    private void EnqueueEvent(TrackEventData data)
    {
        // 添加通用属性
        data.SessionId = _sessionId;
        data.UserId = GetCurrentUserId();

        lock (_eventQueue)
        {
            _eventQueue.Enqueue(data);
        }
    }

    private void UploadEvents(object state)
    {
        List<TrackEventData> events;
        lock (_eventQueue)
        {
            events = _eventQueue.ToList();
            _eventQueue.Clear();
        }

        if (events.Any())
        {
            // 批量上传数据
            UploadToServer(events);
        }
    }

    #region 编码

    private JsonSerializerOptions options = new JsonSerializerOptions
    {
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,  // 允许所有字符
        WriteIndented = true,  // 格式化输出
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase  // 驼峰命名
    };

    #endregion

    private void UploadToServer(List<TrackEventData> events)
    {
        try
        {
            foreach (var eventData in events)
            {
                var json = JsonSerializer.Serialize(eventData, options);
                XLogGlobal.Logger?.LogUserAction(eventData.UserId, eventData.EventType.ToString(), json);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"上传埋点数据失败：{ex.Message}");
        }
    }

    private string GetCurrentPageName()
    {
        // 获取当前页面名称的实现
        return Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive)?.GetType().Name ??
               "Unknown";
    }

    private string GetCurrentUserId()
    {
        // 获取当前用户ID的实现
        return LoginAuthHelper.LoginUser?.UserName ?? "NONE";
    }

    public void Dispose()
    {
        _uploadTimer.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _uploadTimer.DisposeAsync();
    }
}