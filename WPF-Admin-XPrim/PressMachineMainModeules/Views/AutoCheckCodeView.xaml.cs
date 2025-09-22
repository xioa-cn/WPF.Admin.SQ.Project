using System.Windows.Controls;

namespace PressMachineMainModeules.Views {
    public partial class AutoCheckCodeView : Page {
        public AutoCheckCodeView() {
            HandyControl.Controls.Dialog.Register(
                WPF.Admin.Models.Models.HcDialogMessageToken.DialogCheckCodeToken, this);
            InitializeComponent();
        }
    }
}