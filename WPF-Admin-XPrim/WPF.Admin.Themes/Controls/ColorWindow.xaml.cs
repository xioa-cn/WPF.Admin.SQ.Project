using Azure.Core;
using HandyControl.Controls;
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
using System.Windows.Shapes;
using WPF.Admin.Models.Models;

namespace WPF.Admin.Themes.Controls
{
    /// <summary>
    /// ColorWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ColorWindow : System.Windows.Window
    {
        public ColorWindow()
        {
            InitializeComponent();
        }

        private void ThemeChangeColorClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                var t = btn.Content as string;
                ColorText.Text = t;
                if (string.IsNullOrEmpty(t)) return;
                try
                {
                    ThemeManager.UseTheme(t);
                }
                catch
                {

                }
            }

        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ColorText.Text)) return;
            try
            {
                ThemeManager.UseTheme(ColorText.Text);
                ThemeManager.Color = ColorText.Text;
                if (AppSettings.ActionSaveColor is not null)
                {
                    AppSettings.ActionSaveColor.Invoke(ColorText.Text);
                    Growl.SuccessGlobal("主题色保存成功！！！");
                    this.Close();
                }
                
            }
            catch
            {

            }

        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
