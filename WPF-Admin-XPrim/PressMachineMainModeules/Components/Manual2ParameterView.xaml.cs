using HandyControl.Controls;
using PressMachineMainModeules.Models;
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

namespace PressMachineMainModeules.Components
{
    /// <summary>
    /// Manual2ParameterView.xaml 的交互逻辑
    /// </summary>
    public partial class Manual2ParameterView : System.Windows.Controls.UserControl
    {
        public Manual2ParameterView()
        {
            InitializeComponent();
        }

        private void WriteParameterClick(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn
                && btn.Tag is ManualParametersModel dto && btn.CommandParameter is string value)
            {
                if (float.TryParse(value, out var dvalue))
                {
                    dto.Value = dvalue;
                    (this.DataContext as PreManual2ViewModel).WriteParameterCommand.Execute(dto);
                }
                else
                {
                    Growl.WarningGlobal($"Invalid value: {value}. Please enter a valid number.");
                }

            }
        }
    }
}
