using HandyControl.Controls;
using PressMachineMainModeules.Config;
using WPF.Admin.Service.Utils;
using UserControl = System.Windows.Controls.UserControl;

namespace PressMachineMainModeules.Components {
    public partial class AutoMesUploadDatView : UserControl {
        public AutoMesUploadDatView() {
            InitializeComponent();
        }

        private void DKRequestBodyClick(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(RequestBody.Text)) return;
                var result = FormatJsonHelper.FormatJson(RequestBody.Text);
                if (result != null && !string.IsNullOrEmpty(result))
                {
                    RequestBody.Text = result;
                }
            }
            catch (Exception)
            {
                Growl.WarningGlobal(Common.t("Msg.MesChecked.FormatError"));
            }
        }

        private void DataGrid_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}