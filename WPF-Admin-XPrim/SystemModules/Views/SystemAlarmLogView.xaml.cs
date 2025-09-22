using System.Windows;
using System.Windows.Controls;
using SystemModules.ViewModels;

namespace SystemModules.Views {
    public partial class SystemAlarmLogView : Page {
        public SystemAlarmLogView() {       
            InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {
            if (this.DataContext is SystemAlarmLogViewModel vm)
            {
                vm.Loaded();
            }
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}