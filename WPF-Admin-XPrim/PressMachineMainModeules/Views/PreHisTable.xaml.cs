using System.Windows.Controls;

namespace PressMachineMainModeules.Views
{
    /// <summary>
    /// PreHisTable.xaml 的交互逻辑
    /// </summary>
    public partial class PreHisTable : Page
    {
        public PreHisTable()
        {
            InitializeComponent();
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
