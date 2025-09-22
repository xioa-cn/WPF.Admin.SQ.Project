using System.Globalization;
using System.Reflection;
using WPF.Admin.Themes.I18n;

namespace WPFAdmin
{
    public partial class App
    {
        /**
         *  xmlns:i18n="clr-namespace:WPF.Admin.Themes.I18n;assembly=WPF.Admin.Themes"
         * 使用方法  <TextBlock Width="100" Text="{i18n:Localize user.name}" />
         *
         *
         * 修改语言版本
         * LangManager.Instance.ChangeLanguage(langCode);
         *
         * 资源一定要为 嵌入的资源
         */
        /// <summary>
        /// 配置语言管理器的方法
        /// 用于设置语言管理器的资源程序集、命名空间和默认语言，并进行初始化
        /// </summary>
        public static void ConfigureLangManager()
        {
            // // 获取当前执行程序集
            // var assembly = Assembly.GetExecutingAssembly();
            // // 获取程序集中的所有资源名称
            // var resources = assembly.GetManifestResourceNames();
            //
            // // 输出所有资源名称，用于调试
            // foreach (var res in resources)
            // {
            //     System.Diagnostics.Debug.WriteLine("资源名称: " + res);
            // }

            // 语言文件为程序资源的写法
            {
                // 配置语言管理器使用当前程序集作为资源来源
                I18nManager.Instance.I18nResourceAssembly(Assembly.GetExecutingAssembly());
                // 设置资源在当前程序集中的命名空间
                I18nManager.Instance.I18nResourceNamespace("WPFAdmin");
                // 设置默认语言资源文件 （这里为资源名称 排除后命名空间）
                I18nManager.Instance.DefaultLang("Langs.zh");
            }
            // 语言文件为外部文件的写法
            {
                // 先设置为外部资源模式 JsonMode 默认为 OnApplicationResources
                I18nManager.Instance.JsonMode(I18nJsonMode.OnFileDir);
                // 设置资源文件所在的目录
                I18nManager.Instance.I18nResourceDirectory(
                    System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Langs")
                );
                // 设置默认语言文件
                I18nManager.Instance.DefaultLang("zh");
            }


            // 初始化语言管理器
            I18nManager.Instance.Initialize();

            I18nExtensionStr.I18nUseExtensionFunc();
        }
    }
}