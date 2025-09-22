
namespace PressMachineMainModeules.Models
{
    /// <summary>
    /// 精灵按钮触发样式
    /// </summary>
    public enum BtnType
    {
        Success,
        Info,
        Warning,
        Error,
    }

    /// <summary>
    /// 精灵显示数据模型
    /// </summary>
    public class ElfContent
    {
        public string Content { get; set; }
        public string Down { get; set; }
        public bool UsingUp { get; set; }
        public string Up { get; set; }
        public BtnType BtnType { get; set; }
    }
}
