
using PressMachineMainModeules.ViewModels;
using System.Windows.Controls;

namespace PressMachineMainModeules.Views
{
    /// <summary>
    /// IoManager.xaml 的交互逻辑
    /// </summary>
    public partial class IoManagerPage : Page
    {
        public IoManagerPage()
        {
            InitializeComponent();
            this.Loaded += IoManagerPage_Loaded;
            this.Unloaded += IoManagerPage_Unloaded;
        }

        private void IoManagerPage_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.DataContext is IoManagerViewModel vm)
            {
                vm.ViewIsLoaded = false;
            }
        }

        private void IoManagerPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.DataContext is IoManagerViewModel vm)
            {
                vm.ViewIsLoaded = true;
            }
        }
    }
}
