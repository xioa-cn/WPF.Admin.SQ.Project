
namespace PressMachineMainModeules.Views {
    public partial class AutoMesTemplateView : WPF.Admin.Service.Common.RegisterPage {
        public AutoMesTemplateView() {
            HandyControl.Controls.Dialog.Register(
                WPF.Admin.Models.Models.HcDialogMessageToken.DialogMesKeyValueToken, this);
            InitializeComponent();
        }

        public string RegisterNavName { get; set; } = "MesTemplateView";
    }
}