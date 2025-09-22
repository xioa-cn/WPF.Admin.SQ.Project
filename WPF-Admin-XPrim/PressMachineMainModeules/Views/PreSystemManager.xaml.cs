using System.Windows.Controls;

namespace PressMachineMainModeules.Views
{
    /// <summary>
    /// PreSystemManager.xaml 的交互逻辑
    /// </summary>
    public partial class PreSystemManager : Page
    {
        public PreSystemManager()
        {
            HandyControl.Controls.Dialog.Register(
                WPF.Admin.Models.Models.HcDialogMessageToken.DialogPressMachineSystemToken, this);
            InitializeComponent();
        }
    }
}
