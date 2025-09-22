

namespace WPF.Admin.Themes.Converter
{
    public class NavIndex
    {
        public bool OpenIndex { get; set; }
    }
    public class OpenNavIndexHelper
    {
        public static NavIndex NavIndexConfig { get; set; } = new NavIndex();
    }
}
