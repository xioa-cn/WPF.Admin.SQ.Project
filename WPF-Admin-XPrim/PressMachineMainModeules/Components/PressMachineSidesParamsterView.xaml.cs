using PressMachineMainModeules.Models;
using System.Windows;

namespace PressMachineMainModeules.Components
{
    /// <summary>
    /// 横移
    /// </summary>
    public partial class PressMachineSidesParamsterView : System.Windows.Controls.UserControl
    {
        public static readonly DependencyProperty PressMachineSideswayDaProperty =
           DependencyProperty.Register(
               "PressMachineSideswayDa",
               typeof(PressMachineSideswayDa), typeof(PressMachineSidesParamsterView),
               new FrameworkPropertyMetadata(new PressMachineSideswayDa(),
                   new PropertyChangedCallback(OnPressMachineSideswayDaChanged)
                   )
               { BindsTwoWayByDefault = true }
               );

        private static void OnPressMachineSideswayDaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PressMachineSidesParamsterView)d).PressMachineSideswayDa = (PressMachineSideswayDa)e.NewValue!;
        }


        public PressMachineSideswayDa? PressMachineSideswayDa
        {
            get { return (PressMachineSideswayDa)GetValue(PressMachineSideswayDaProperty); }
            set { SetValue(PressMachineSideswayDaProperty, value); }
        }






        public static readonly DependencyProperty TitleDaProperty =
           DependencyProperty.Register(
               "Title",
               typeof(string), typeof(PressMachineSidesParamsterView),
               new FrameworkPropertyMetadata("参数",
                   new PropertyChangedCallback(OnTitleDaChanged)
                   )
               { BindsTwoWayByDefault = true }
               );

        private static void OnTitleDaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PressMachineSidesParamsterView)d).Title = (string)e.NewValue!;
        }


        public string? Title
        {
            get { return (string)GetValue(TitleDaProperty); }
            set { SetValue(TitleDaProperty, value); }
        }









        public PressMachineSidesParamsterView()
        {
            InitializeComponent();
        }
    }
}
