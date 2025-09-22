using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using WPF.Admin.Models;
using WPF.Admin.Models.Models;
using WPF.Admin.Models.Utils;
using WPF.Admin.Service.Logger;
using WPF.Admin.Themes.Converter;
using WPF.Admin.Themes.Helper;

namespace WPF.Admin.Themes;

public partial class ThemeManager : BindableBase {
    private static string? _color;

    public static string Color {
        get
        {
            if (!string.IsNullOrWhiteSpace(_color))
            {
                return _color;
            }

            if(AppSettings.Default is not null)
            {
                _color = AppSettings.Default.Color;
                return _color;
            }

            var settingJsonFile =
                ApplicationConfigConst.SettingJsonFile;
            var settingJsonString = System.IO.File.ReadAllText(settingJsonFile);
            var temp = System.Text.Json.JsonSerializer.Deserialize<AppSettings>(settingJsonString);
            if (temp is null) throw new NullReferenceException(settingJsonFile);
            _color = temp.Color.ToString();

            return _color;
        }
        set
        {
            _color = value;
            RefreCacheTheme();
        }
    }

    private static ThemeManager? _instance;
    public static ThemeManager Instance => _instance ??= NormalThemeManager();

    private static ThemeManager NormalThemeManager() {
        if (AppSettings.Default is not null &&
            AppSettings.Default.ApplicationVersions == ApplicationVersions.NoAuthorization)
        {
           
        }

        return new ThemeManager();
    }

    public static void ChangeDarkTheme() {
        Instance.IsDarkTheme = !Instance.IsDarkTheme;
    }

    public static void SetTheme(string? theme) {
        if (string.IsNullOrEmpty(theme))
            return;
        Instance.IsDarkTheme = theme == "Dark";
    }

    [ObservableProperty] private bool _isDarkTheme;


    public ThemeManager() {
        // 默认使用深色主题
        IsDarkTheme = false;
        ApplyTheme();
    }

    partial void OnIsDarkThemeChanged(bool value) {
        ApplyTheme();
        if (AppSettings.Default == null) return;
        try
        {
            AppSettings.Default.Theme = value ? "Dark" : "Light";
            SerializeHelper.Serialize(ApplicationConfigConst.SettingJsonFile,
                AppSettings.Default);
        }
        catch (Exception ex)
        {
            XLogGlobal.Logger?.LogError("ThemeManager 保存主题失败", ex);
        }

        WeakReferenceMessenger.Default.Send(ThemeModel.Dark);
    }

    /***
     * 新建页面时 配合主题使用
     *
     * 在每个页面的根元素（Page、UserControl等）添加：Background="{DynamicResource Background.Brush}"
     *
     * 对于所有文本元素（TextBlock、TextBox等）使用
     * Foreground="{DynamicResource Text.Primary.Brush}"  <!-- 主要文本 -->
     * Foreground="{DynamicResource Text.Secondary.Brush}"  <!-- 次要文本 -->
     *
     * 对于所有边框和分隔线使用：
     * BorderBrush="{DynamicResource Border.Brush}"
     *
     * 对于面板和容器使用：
     * Background="{DynamicResource Background.Brush}"  <!-- 主背景 -->
     * Background="{DynamicResource Surface.Brush}"  <!-- 次要背景 -->
     *
     * 对于按钮和交互元素使用：
     * Background="{DynamicResource Primary.Brush}"  <!-- 主要按钮 -->
     * Background="{DynamicResource Secondary.Brush}"  <!-- 次要按钮 -->
     *
     * 对于输入控件（TextBox、ComboBox等）使用：
     * Background="{DynamicResource Background.Brush}"
     * Foreground="{DynamicResource Text.Primary.Brush}"
     * BorderBrush="{DynamicResource Border.Brush}"
     *
     */


    // HandyControl主题资源
    private static readonly ResourceDictionary HcLightTheme = new() {
        Source = new Uri("pack://application:,,,/HandyControl;component/Themes/SkinDefault.xaml")
    };

    private static readonly ResourceDictionary HcDarkTheme = new() {
        Source = new Uri("pack://application:,,,/HandyControl;component/Themes/SkinDark.xaml")
    };

    // 自定义主题资源
    private static ResourceDictionary CustomLightTheme = new() {
        Source = new Uri($"pack://application:,,,/WPF.Admin.Themes;component/Themes/{Color}Theme.xaml")
    };

    private static ResourceDictionary CustomDarkTheme = new() {
        Source = new Uri($"pack://application:,,,/WPF.Admin.Themes;component/Themes/Dark{Color}Theme.xaml")
    };

    private static ResourceDictionary GetResourceDictionary(string themeName) {
        return new ResourceDictionary {
            Source = new Uri($"pack://application:,,,/WPF.Admin.Themes;component/Themes/{themeName}Theme.xaml")
        };
    }

    private static void RefreCacheTheme() {
        CustomLightTheme = GetResourceDictionary(Color);
        CustomDarkTheme = GetResourceDictionary($"Dark{Color}");
    }


    private void UpdateThemeResource(Collection<ResourceDictionary> resources,
        ResourceDictionary lightTheme, ResourceDictionary darkTheme, bool hc = false) {
        if (hc)
        {
            // 1. 先移除所有HandyControl相关资源
            var hcResources = resources.Where(x =>
                x.Source?.ToString().Contains("/HandyControl;component/") ?? false).ToList();
            foreach (var resource in hcResources)
            {
                resources.Remove(resource);
            }

            // 2. 重新添加HandyControl基础资源
            resources.Add(new ResourceDictionary {
                Source = new Uri("pack://application:,,,/HandyControl;component/Themes/Theme.xaml")
            });
        }


        // 添加新主题
        resources.Add(IsDarkTheme ? darkTheme : lightTheme);
    }

    private void ApplyTheme() {
        if (ApplicationAuthTaskFactory.AuthFlag)
        {
            throw new Exception("授权失败，无法切换主题");
        }

        var app = Application.Current;
        if (app == null) return;

        Application.Current.Dispatcher.Invoke(() =>
        {
            var resources = app.Resources.MergedDictionaries;

            // 更新HandyControl主题
            UpdateThemeResource(resources, HcLightTheme, HcDarkTheme, true);

            // 更新自定义主题
            UpdateThemeResource(resources, CustomLightTheme, CustomDarkTheme);

            // 更新窗口背景
            UpdateWindowBackground();
        });
    }

    private void UpdateWindowBackground() {
        if (Application.Current.MainWindow?.Template.FindName("MainBorder",
                Application.Current.MainWindow) is Border border)
        {
            border.Background = Application.Current.Resources["Background.Brush"] as SolidColorBrush;
        }
    }

    private static ResourceDictionary? _currentTheme;

    public static void UseTheme(string content) {
        var app = Application.Current;
        if (app == null) return;

        // 移除当前主题（如果存在）
        if (_currentTheme != null)
        {
            app.Resources.MergedDictionaries.Remove(_currentTheme);
        }

        string themePath;
        themePath = Instance.IsDarkTheme ? $"pack://application:,,,/WPF.Admin.Themes;component/Themes/Dark{content}Theme.xaml" 
            : $"pack://application:,,,/WPF.Admin.Themes;component/Themes/{content}Theme.xaml";

        _currentTheme = new ResourceDictionary { Source = new Uri(themePath, UriKind.Absolute) };
        app.Resources.MergedDictionaries.Add(_currentTheme);
        // 更新窗口背景色
        if (Application.Current.MainWindow == null)
        {
            return;
        }

        var border =
            Application.Current.MainWindow.Template.FindName("MainBorder",
                Application.Current.MainWindow) as Border;
        if (border == null)
        {
            return;
        }

        SolidColorBrush? brush = (SolidColorBrush?)Application.Current.Resources["Background.Brush"];
        border.Background = brush;
    }
}