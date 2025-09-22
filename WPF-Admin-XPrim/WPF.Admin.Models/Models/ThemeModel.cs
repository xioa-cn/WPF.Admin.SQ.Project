

namespace WPF.Admin.Models.Models
{
    public class ThemeModel
    {
        public string Theme { get; set; }

        public static ThemeModel Dark { get; set; } = new ThemeModel
        {
            Theme = "Dark"
        };
        public static ThemeModel Light { get; set; } = new ThemeModel
        {
            Theme = "Light"
        };
    }
}
