using System.Windows;
using WPF.Admin.Themes.Helper;

namespace WPFAdmin.AuthApplication;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void CreateAuthCodeClick(object sender, RoutedEventArgs e)
    {
        var str = this.KeyTxt.Text.Replace("\r","").Replace(" ","");
        if (string.IsNullOrEmpty(str))
        {
            MessageBox.Show("版本号异常");
            return;
        }
        var authCode = TextCodeHelper.Encrypt(key: str);

        this.ValueTxt.Text = authCode;

        MessageBox.Show("Success");
    }

    private void CopyAuthCodeClick(object sender, RoutedEventArgs e)
    {
        Clipboard.SetText(this.ValueTxt.Text);
        MessageBox.Show("复制成功");
    }
}