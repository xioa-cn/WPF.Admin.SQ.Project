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
    /// PreParams.xaml 的交互逻辑
    /// </summary>
    public partial class PreParams : Page
    {
        public PreParams()
        {
            InitializeComponent();
            this.ConfigNames.SelectionChanged += ConfigNames_SelectionChanged;
        }

        private void ConfigNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.ConfigNames.SelectedIndex < 0)
            {
                return;
            }

            var name = this.ConfigNames.SelectedItem;
            if(name is string value)
            {
                if(this.DataContext is PressMachineParamsViewModel vm)
                {
                    vm.ConfigName = value;
                }
            }
        }
    }
}
