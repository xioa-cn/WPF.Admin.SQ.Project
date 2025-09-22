using System.Windows;
using PressMachineMainModeules.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace PressMachineMainModeules.Components {
    public partial class AutoWorkwearAddView : UserControl {
        public AutoWorkwearAddView() {
            InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {
            if (this.DataContext is AutoWorkwearCheckCodeViewModel viewmodel)
            {
                viewmodel.RefreshAutoWorkwearCodeList();
            }
        }

        private void DataGrid_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}