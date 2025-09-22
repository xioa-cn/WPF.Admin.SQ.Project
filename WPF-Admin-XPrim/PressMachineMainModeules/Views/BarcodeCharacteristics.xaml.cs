using System.Windows.Controls;
using WPF.Admin.Models.Models;

namespace PressMachineMainModeules.Views {
    public partial class BarcodeCharacteristics : Page {
        public BarcodeCharacteristics() {
            HandyControl.Controls.Dialog.Register(HcDialogMessageToken.DialogPartialCodeToken, this);
            InitializeComponent();
            this.ConfigNames.SelectionChanged += ConfigNames_SelectionChanged;
        }

        private void ConfigNames_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            if (this.ConfigNames.SelectedIndex < 0)
            {
                return;
            }

            var name = this.ConfigNames.SelectedItem;
            if (name is string value)
            {
                if (this.DataContext is PressMachineMainModeules.ViewModels.BarcodeCharacteristicsViewModel vm)
                {
                    vm.ConfigName = value;
                }
            }
        }
    }
}