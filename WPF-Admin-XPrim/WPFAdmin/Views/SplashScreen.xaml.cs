using System.Windows;
using System.Windows.Media.Animation;
using WPF.Admin.Models.Models;
using WPF.Admin.Models.Utils;
using WPF.Admin.Themes.Helper;

namespace WPFAdmin.Views;

public partial class SplashScreen : Window {
    public SplashScreen() {
        this.SetWindowIcon();
        var createTime = ApplicationAuthModule.DllCreateTime;

        var code =
            $"{createTime.ToString("yyyy")} © {nameof(WPFAdmin)} BY {AppAuthor.Author}. {createTime.TimeYearMonthDayHourString()}";

        InitializeComponent();

        this.sText.Text = code;
        Loaded += SplashScreen_Loaded;
    }

    private void SplashScreen_Loaded(object sender, RoutedEventArgs e) {
        var storyboard = FindResource("LoadingAnimation") as Storyboard;
        storyboard?.Begin();
    }
}