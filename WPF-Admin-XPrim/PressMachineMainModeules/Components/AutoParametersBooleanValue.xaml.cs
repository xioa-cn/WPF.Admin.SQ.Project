using System.Windows;
using PressMachineMainModeules.Models;
using UserControl = System.Windows.Controls.UserControl;

namespace PressMachineMainModeules.Components
{
    public partial class AutoParametersBooleanValue : UserControl
    {
        public AutoParameterContentModel AutoParameterContent
        {
            get { return (AutoParameterContentModel)GetValue(AutoParameterContentProperty); }
            set { SetValue(AutoParameterContentProperty, value); }
        }

        public static readonly DependencyProperty AutoParameterContentProperty =
            DependencyProperty.Register("AutoParameterContent", typeof(AutoParameterContentModel),
                typeof(AutoParametersBooleanValue), new PropertyMetadata(null, valueCallback));

        private static void valueCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AutoParametersBooleanValue booleanValue)
            {
                var b = (int)((e.NewValue as AutoParameterContentModel).Value) == 1;
                if (b)
                {
                    booleanValue.open.IsChecked = true;
                }
                else
                {
                    booleanValue.close.IsChecked = true;
                }
            }
        }

        public AutoParametersBooleanValue()
        {
            InitializeComponent();
        }

        private void closeChecked(object sender, RoutedEventArgs e)
        {
            if (this.close.IsChecked is not null && (bool)this.close.IsChecked)
                this.AutoParameterContent.Value = 0;
        }

        private void openChecked(object sender, RoutedEventArgs e)
        {
            if (this.open.IsChecked is not null && (bool)this.open.IsChecked)
                this.AutoParameterContent.Value = 1;
        }
    }
}