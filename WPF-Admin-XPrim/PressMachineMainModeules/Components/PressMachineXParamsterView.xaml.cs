using PressMachineMainModeules.Models;
using System.Windows;

namespace PressMachineMainModeules.Components
{
    /// <summary>
    /// X轴参数
    /// </summary>
    public partial class PressMachineXParamsterView : System.Windows.Controls.UserControl
    {
        public static readonly DependencyProperty PressMachineParamsXDaProperty =
          DependencyProperty.Register(
              "PressMachineParamsXDa",
              typeof(PressMachineParamsXDa), typeof(PressMachineXParamsterView),
              new FrameworkPropertyMetadata(new PressMachineParamsXDa(),
                  new PropertyChangedCallback(OnPressMachineSideswayDaChanged)
                  )
              { BindsTwoWayByDefault = true }
              );

        private static void OnPressMachineSideswayDaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PressMachineXParamsterView)d).PressMachineParamsXDa = (PressMachineParamsXDa)e.NewValue!;
        }


        public PressMachineParamsXDa? PressMachineParamsXDa
        {
            get { return (PressMachineParamsXDa)GetValue(PressMachineParamsXDaProperty); }
            set { SetValue(PressMachineParamsXDaProperty, value); }
        }


        public PressMachineXParamsterView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TitleProperty =
         DependencyProperty.Register(
             "Title",
             typeof(string), typeof(PressMachineXParamsterView),
             new FrameworkPropertyMetadata("参数",
                 new PropertyChangedCallback(OnTitleDaChanged)
                 )
             { BindsTwoWayByDefault = true }
             );

        private static void OnTitleDaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PressMachineXParamsterView)d).Title = (string)e.NewValue!;
        }


        public string? Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
    }
}
