using System.Windows.Controls;
using System.Windows;
using WPF.Admin.Themes.Controls;

namespace WPF.Admin.Themes
{
    public class XioaMasks : ContentControl
    {
        public static readonly DependencyProperty LoadingsProperty =
            ElementBase.Property<XioaMasks, string>(nameof(LoadingsProperty));

        public string Loadings
        {
            get => (string)GetValue(LoadingsProperty);
            set => SetValue(LoadingsProperty, value);
        }

        public static readonly DependencyProperty BackgroundOpacityProperty = DependencyProperty.Register(
            nameof(BackgroundOpacity), typeof(double), typeof(XioaMasks), new PropertyMetadata(default(double)));

        public double BackgroundOpacity
        {
            get => (double)GetValue(BackgroundOpacityProperty);
            set => SetValue(BackgroundOpacityProperty, value);
        }

        public async void Loading(Action action)
        {
            this.Visibility = Visibility.Visible;
            await Task.Run(action.Invoke);
            this.Visibility = Visibility.Collapsed;
        }

        //private Border? _border;
        public override void OnApplyTemplate()
        {
            //_border = GetTemplateChild("MasksBorder") as Border;

            //VisualBrush brush = new VisualBrush();
            //brush.Visual = _border;
            //brush.Stretch = Stretch.Uniform;
            //_border.Background = FillRule;
            //_border.Effect = new BlurEffect()
            //{
            //    Radius = 80,
            //    RenderingBias = RenderingBias.Performance
            //};
            //_border.Margin = new Thickness(-this.Margin.Left, -this.Margin.Top, 0, 0);


            this.Visibility = Visibility.Collapsed;
            base.OnApplyTemplate();
        }
    }
}
