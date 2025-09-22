using System.Windows.Controls;

namespace PressMachineMainModeules.Views
{
    public partial class SignalInteractionView : Page
    {
        public SignalInteractionView()
        {
            HandyControl.Controls.Dialog.Register(
                WPF.Admin.Models.Models.HcDialogMessageToken.SignalInteractionViewToken, this);
            InitializeComponent();
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}