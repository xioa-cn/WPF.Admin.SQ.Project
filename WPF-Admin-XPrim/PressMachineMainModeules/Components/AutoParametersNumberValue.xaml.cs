using System.Windows;
using PressMachineMainModeules.Models;
using UserControl = System.Windows.Controls.UserControl;

namespace PressMachineMainModeules.Components {
    public partial class AutoParametersNumberValue : UserControl {
        public AutoParameterContentModel AutoParameterContent {
            get { return (AutoParameterContentModel)GetValue(AutoParameterContentProperty); }
            set { SetValue(AutoParameterContentProperty, value); }
        }

        public static readonly DependencyProperty AutoParameterContentProperty =
            DependencyProperty.Register("AutoParameterContent", typeof(AutoParameterContentModel),
                typeof(AutoParametersNumberValue), new PropertyMetadata(null));

        public AutoParametersNumberValue() {
            InitializeComponent();
        }
    }
}