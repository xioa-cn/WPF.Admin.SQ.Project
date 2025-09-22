using CMS.AppRouters.Models;
using System.Text.Json;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using WPF.Admin.Themes.Helper;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using CMS.AppRouters.ViewModels;
using WPF.Admin.Service.Services;

namespace CMS.AppRouters.Views
{
    public partial class RoutersSettingsView : Page
    {
        private GlobalHotKey? _globalHotKey;
        public RoutersSettingsView()
        {
            InitializeComponent();


            this.Loaded += (_, _) =>
            {
                try
                {
                    if (_globalHotKey is not null) return;
                    var window = Window.GetWindow(this);
                    _globalHotKey = new GlobalHotKey(window);
                    _globalHotKey.RegisterHotKey(
                            GlobalHotKey.ModControl,
                                'S',
                            KDJsonString);
                }
                catch (Exception)
                {
                }
            };

        }

        private void KDJsonString()
        {
            try
            {
                string input = ContentText.Text.Trim();
                if (string.IsNullOrWhiteSpace(input))
                {
                    HandyControl.Controls.MessageBox.Show("请输入JSON字符串", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                JToken parsedJson;
                if (input.TrimStart().StartsWith("{"))
                {
                    parsedJson = JObject.Parse(input);
                }
                else if (input.TrimStart().StartsWith("["))
                {
                    parsedJson = JArray.Parse(input);
                }
                else
                {
                    MessageBox.Show("无效的JSON格式。JSON必须以'{'或'['开头。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 格式化JSON
                string formattedJson = parsedJson.ToString(Formatting.Indented);
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    if (this.DataContext is RoutersSettingsViewModel rss)
                    {

                        rss.RouterText = formattedJson;


                    }
                    else
                    {
                        ContentText.Text = formattedJson;
                    }
                });
                SnackbarHelper.Show("格式化完成");
            }
            catch (JsonReaderException ex)
            {
                HandyControl.Controls.MessageBox.Show($"JSON解析错误：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                HandyControl.Controls.MessageBox.Show($"发生未知错误：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static readonly JsonSerializerOptions _options = new()
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,

            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public static readonly Encoding _encoding = new UTF8Encoding(true);

        private void ContentClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                if (btn.Tag is BaseRouters br)
                {
                    Clipboard.SetText(System.Text.Json.JsonSerializer.Serialize(br, _options) + ",");
                    SnackbarHelper.Show("已复制到剪贴板");
                }
            }
        }

        private void KDTextClick(object sender, RoutedEventArgs e)
        {
            KDJsonString();
        }
    }
}