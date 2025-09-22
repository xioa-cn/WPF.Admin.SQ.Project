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
    /// Manual2OperationDialogView.xaml 的交互逻辑
    /// </summary>
    public partial class Manual2OperationDialogView : System.Windows.Controls.UserControl
    {
        public Manual2OperationDialogView()
        {
            InitializeComponent();
        }

        private void btnClick(object sender, RoutedEventArgs e)
        {
            if(sender is System.Windows.Controls.Button btn && btn.Tag is ManualPointPositionModel dto)
            {
                (this.DataContext as PreManual2ViewModel).StartWriteCommand.Execute(dto);
            }
        }

        private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.Tag is ManualPointPositionModel dto)
            {
                (this.DataContext as PreManual2ViewModel).EnterWriteCommand.Execute(dto);
            }
        }
    }
}
