using PressMachineMainModeules.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PressMachineMainModeules.Views
{
    /// <summary>
    /// PreManual2.xaml 的交互逻辑
    /// </summary>
    public partial class PreManual2 : Page
    {
        public PreManual2()
        {
            InitializeComponent();
            this.Loaded += PreManual2_Loaded;
            this.Unloaded += PreManual2_Unloaded;
        }

        private void PreManual2_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is PreManual2ViewModel vm)
            {
                vm.ViewIsLoaded = false;
            }
        }

        private void PreManual2_Loaded(object sender, RoutedEventArgs e)
        {
            if(this.DataContext is PreManual2ViewModel vm)
            {
                vm.ViewIsLoaded = true;
            }

        }
    }
}
