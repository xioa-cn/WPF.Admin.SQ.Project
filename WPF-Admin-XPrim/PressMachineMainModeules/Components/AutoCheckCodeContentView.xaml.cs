using UserControl = System.Windows.Controls.UserControl;

namespace PressMachineMainModeules.Components
{
    public partial class AutoCheckCodeContentView : UserControl
    {
        public AutoCheckCodeContentView()
        {
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
            if (name is string value && !string.IsNullOrEmpty(value))
            {
                if (this.DataContext is PressMachineMainModeules.ViewModels.AutoCheckCodeContentViewModel vm)
                {
                    vm.ConfigName = value;
                }
            }
        }
    }
}