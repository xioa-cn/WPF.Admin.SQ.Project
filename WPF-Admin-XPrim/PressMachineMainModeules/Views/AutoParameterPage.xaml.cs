
namespace PressMachineMainModeules.Views
{
    /// <summary>
    /// AutoParameterPage.xaml 的交互逻辑
    /// </summary>
    public partial class AutoParameterPage : System.Windows.Controls.Page
    {
        public AutoParameterPage()
        {
            HandyControl.Controls.Dialog.Register(
                WPF.Admin.Models.Models.HcDialogMessageToken.DialogPressMachineParametersToken, this);
            InitializeComponent();
            this.ConfigNames.SelectionChanged += ConfigNames_SelectionChanged;
        }

        private void ConfigNames_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.ConfigNames.SelectedIndex < 0)
            {
                return;
            }

            var name = this.ConfigNames.SelectedItem;
            if (name is string value)
            {
                if (this.DataContext is PressMachineMainModeules.ViewModels.AutoParameterViewModel vm)
                {
                    vm.ConfigName = value;
                }
            }
        }
    }
}
