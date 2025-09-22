using System.Windows;
using PressMachineMainModeules.Models;
using PressMachineMainModeules.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace PressMachineMainModeules.Components {
    public partial class AutoWorkwearBindingParameterView : UserControl {
        public AutoWorkwearBindingParameterView() {
            InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {
            if (this.DataContext is AutoWorkwearCheckCodeViewModel viewmodel)
            {
                viewmodel.GetConfigList();
                viewmodel.RefreshAutoWorkwearBindingParameters();
            }
        }

        private void DataGrid_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e) {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            if (this.DataContext is not AutoWorkwearCheckCodeViewModel viewmodel)
            {
                return;
            }

            if (sender is System.Windows.Controls.ComboBox { Tag: AutoWorkwearBindingParam model } cb)
            {
                var value = cb.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }

                model.ParamerterName = value;
            }
            else
            {
                return;
            }


            viewmodel.SaveAutoWorkwearBindingParametersList();
        }
    }
}