
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using WPF.Admin.Themes.Controls;

namespace WPF.Admin.Themes.Helper
{
    public class ThemeColorHelper
    {
        public static ThemeColorHelper ThemeColorManager { get; set; } = new ThemeColorHelper();
        public ICommand SystemColorCommand { get; set; }

        public ThemeColorHelper()
        {
            SystemColorCommand = new RelayCommand(SysColorMethod);
        }

        private void SysColorMethod()
        {
            ColorWindow color = new ColorWindow();
            color.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            color.ShowDialog();
        }
    }
}
