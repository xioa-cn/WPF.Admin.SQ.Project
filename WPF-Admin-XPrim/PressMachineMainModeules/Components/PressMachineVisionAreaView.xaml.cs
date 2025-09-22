using PressMachineMainModeules.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PressMachineMainModeules.Components
{
    /// <summary>
    /// PressMachineVisionAreaView.xaml 的交互逻辑
    /// </summary>
    public partial class PressMachineVisionAreaView : System.Windows.Controls.UserControl
    {
        #region 可视化区域

        public static readonly DependencyProperty CurveParaProperty =
            DependencyProperty.Register(
                "CurvePara",
                typeof(CurvePara), typeof(PressMachineVisionAreaView),
                new FrameworkPropertyMetadata(new CurvePara(),
                    new PropertyChangedCallback(OnCurveParaPropertyChanged)
                    )
                { BindsTwoWayByDefault = true }
                );

        private static void OnCurveParaPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PressMachineVisionAreaView)d).CurvePara = (CurvePara)e.NewValue!;
        }

        public CurvePara CurvePara
        {
            get { return (CurvePara)GetValue(CurveParaProperty); }
            set { SetValue(CurveParaProperty, value); }
        }

        #endregion
        public PressMachineVisionAreaView()
        {
            InitializeComponent();
        }
    }
}
