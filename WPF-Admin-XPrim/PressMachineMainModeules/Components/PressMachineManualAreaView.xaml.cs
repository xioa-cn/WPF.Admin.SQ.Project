using System.Windows;
using System.Windows.Controls;
using WPF.Admin.Themes.Helper;

namespace PressMachineMainModeules.Components
{
    /// <summary>
    /// PressMachineManualAreaView.xaml 的交互逻辑
    /// </summary>
    public partial class PressMachineManualAreaView : System.Windows.Controls.UserControl
    {
        public PressMachineManualAreaView()
        {
            InitializeComponent();
        }

        private void Bar_Test(object sender, RoutedEventArgs e)
        {
            SnackbarHelper.Show("OK", 3000);
        }
    }
}
