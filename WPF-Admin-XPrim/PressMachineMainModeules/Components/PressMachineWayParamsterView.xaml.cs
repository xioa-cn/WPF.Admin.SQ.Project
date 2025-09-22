using PressMachineMainModeules.Models;
using System.Windows;

namespace PressMachineMainModeules.Components
{
    /// <summary>
    /// 滑台
    /// </summary>
    public partial class PressMachineWayParamsterView : System.Windows.Controls.UserControl
    {

        public static readonly DependencyProperty PressMachineSlipwayDaProperty =
          DependencyProperty.Register(
              "PressMachineSlipwayDa",
              typeof(PressMachineSlipwayDa), typeof(PressMachineWayParamsterView),
              new FrameworkPropertyMetadata(new PressMachineSlipwayDa(),
                  new PropertyChangedCallback(OnPressMachineSlipwayDaChanged)
                  )
              { BindsTwoWayByDefault = true }
              );

        private static void OnPressMachineSlipwayDaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PressMachineWayParamsterView)d).PressMachineSlipwayDa = (PressMachineSlipwayDa)e.NewValue!;
        }


        public PressMachineSlipwayDa? PressMachineSlipwayDa
        {
            get { return (PressMachineSlipwayDa)GetValue(PressMachineSlipwayDaProperty); }
            set { SetValue(PressMachineSlipwayDaProperty, value); }
        }






        public static readonly DependencyProperty TitleDaProperty =
           DependencyProperty.Register(
               "Title",
               typeof(string), typeof(PressMachineWayParamsterView),
               new FrameworkPropertyMetadata("参数",
                   new PropertyChangedCallback(OnTitleDaChanged)
                   )
               { BindsTwoWayByDefault = true }
               );

        private static void OnTitleDaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PressMachineWayParamsterView)d).Title = (string)e.NewValue!;
        }


        public string? Title
        {
            get { return (string)GetValue(TitleDaProperty); }
            set { SetValue(TitleDaProperty, value); }
        }

        public PressMachineWayParamsterView()
        {
            InitializeComponent();
        }
    }
}
