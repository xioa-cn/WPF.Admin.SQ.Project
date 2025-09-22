using System.Windows;
using WPF.Admin.Models.Models;
using WPF.Admin.Models.Utils;
using WPF.Admin.Service.Utils;
using WPF.Admin.Themes.Converter;

namespace WPFAdmin.Config;

public static class Configs {
    public static AppSettings? Default {
        get => AppSettings.Default;
        private set
        {
            if (value == null)
            {
                return;
            }

            AppSettings.Default = value;
            SerializeHelper.Serialize(SettingJsonFile,
                value);
        }
    }

    private static readonly string SettingJsonFile =
        ApplicationConfigConst.SettingJsonFile;

    static Configs() {
        if (System.IO.File.Exists(SettingJsonFile))
        {
            var settingJsonString = System.IO.File.ReadAllText(SettingJsonFile);
            Default = System.Text.Json.JsonSerializer.Deserialize<AppSettings>(settingJsonString);
        }
        else
        {
            var localSettingsString = ApplicationUtils.FindApplicationResourceFile(
                nameof(WPFAdmin), ApplicationConfigConst.SettingJsonFileName);
            Default = System.Text.Json.JsonSerializer.Deserialize<AppSettings>(localSettingsString);

            SerializeHelper.Serialize(SettingJsonFile, Default);
            MessageBox.Show($"{ApplicationConfigConst.SettingJsonFileName}文件不存在，已创建默认配置文件，请修改配置文件");
        }

        if (Default is null)
            throw new NullReferenceException(nameof(Default));
        AppSettings.ConfigRouter = Default.RouteName;
        ListenApplicationVersions.NormalVersion = Default.ApplicationVersions;
        if (Default.RouteName.Contains("cms"))
        {
            //Default.AppName += "_CMS";
            Default.Theme = "Dark";
        }

        if (Default.WindowSize == "Max")
        {
            Default.Width = SystemParameters.WorkArea.Width;
            Default.Height = SystemParameters.WorkArea.Height;
        }

        if (Default == null)
            throw new System.IO.FileNotFoundException(SettingJsonFile);
        LoginAuthHelper.SetViewAuthSwitch(Default.ViewAuthSwitch);
        OpenNavIndexHelper.NavIndexConfig.OpenIndex = Default.AutoOpenNavIndex;

        AppSettings.ActionSaveColor += ActionSaveColor;
    }

    private static void ActionSaveColor(string color) {
        if (Default is null) return;
        Default.Color = color;
        if (AppSettings.ApplicationCms == ApplicationCms.BatCms)
        {
            Default.RouteName = AppSettings.ConfigRouter ?? Default.RouteName;
        }

        SerializeHelper.Serialize(SettingJsonFile, Default);
    }

    /**
     2025.3.21 更新 按钮级权限管理 使用方法

     xmlns:auth="clr-namespace:WPF.Admin.Themes.Converter;assembly=WPF.Admin.Themes"

      IsEnabled="{Binding Source={x:Static auth:LoginAuthHelper.LoginUser},
    Path=LoginAuth, Converter={StaticResource EngineerEnabled}}"


    Visibility="{Binding Source={x:Static auth:LoginAuthHelper.LoginUser},
    Path=LoginAuth, Converter={StaticResource EngineerVisibility}}"

     */


    /**

     *
     *
     * 权限全局配置文件 appSetting.json => auth 字段
      XAML界面内容鉴权
      命名控件引入
      xmlns:model="clr-namespace:WPF.Admin.Models.Models;assembly=WPF.Admin.Models"
      xmlns:converter="clr-namespace:WPF.Admin.Themes.Converter;assembly=WPF.Admin.Themes"


      <converter:LoginAuthToEnabledConverter x:Key="RouterIsEnabledConverter" />
      <converter:LoginAuthToVisibilityConverter x:Key="RouterVisibilityConverter" />

      控件上使用
      IsEnabled="{Binding Source={x:Static model:PageContentAuthHelper.ContentAuth},
                    Path=Admin, Converter={StaticResource RouterIsEnabledConverter}}"

      Visibility="{Binding Source={x:Static model:PageContentAuthHelper.ContentAuth},
                    Path=Admin, Converter={StaticResource RouterVisibilityConverter}}"

     */
}