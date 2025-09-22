namespace WPF.Admin.Themes.Monitor;

/// <summary>
/// 埋点数据模型
/// </summary>
public class TrackEventData
{
    public string EventId { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public TrackEventType EventType { get; set; }
    public string PageName { get; set; }
    public string ElementId { get; set; }
    public string ElementType { get; set; }
    public Dictionary<string, object> Properties { get; set; }
    public string UserId { get; set; }
    public string SessionId { get; set; }
}