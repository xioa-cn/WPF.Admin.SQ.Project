using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using WPF.Admin.Models;
using WPF.Admin.Themes.I18n;
using XPrism.Core.DI;

namespace WPFAdmin.ViewModels
{
    [AutoRegister(typeof(IChangeLangable), ServiceLifetime.Singleton, nameof(ChangeLangViewModel))]
    public partial class ChangeLangViewModel : BindableBase, IChangeLangable
    {
        [ObservableProperty] private string lang = I18nManager.Instance.DefaultLanguage;

        public ObservableCollection<LangSource> Langs { get; }

        public ICommand ChangeLangCommand { get; }


        public ChangeLangViewModel()
        {
            Langs = new ObservableCollection<LangSource>();

            if (I18nManager.Instance.I18NJsonMode == I18nJsonMode.OnApplicationResources)
            {
                // 获取程序集中的所有资源名称
                var resources = Assembly.GetExecutingAssembly().GetManifestResourceNames();

                // 输出所有资源名称，用于调试
                foreach (var res in resources)
                {
                    if (res.Contains("Langs"))
                    {
                        var name = res.Replace(".json", "").Replace($"{nameof(WPFAdmin)}.", "");
                        Langs.Add(new LangSource
                        {
                            Name = name,
                            SourceKey = name,
                        });
                    }
                }
            }
            else if (I18nManager.Instance.I18NJsonMode == I18nJsonMode.OnFileDir)
            {
                var files = Directory.GetFiles(I18nManager.Instance.ResourceDirectory, "*.json");
                foreach (var file in files)
                {
                    var name = Path.GetFileNameWithoutExtension(file);
                    Langs.Add(new LangSource
                        {
                            Name = name,
                            SourceKey = name,
                        }
                    );
                }
            }


            ChangeLangCommand = new RelayCommand<string>(ChangeLang);
        }

        private void ChangeLang(string? obj)
        {
            if (string.IsNullOrEmpty(obj))
            {
                Growl.ErrorGlobal("未找到语言文件");
                return;
            }

            if (I18nManager.Instance.UsingLanguage == obj)
            {
                return;
            }

            I18nManager.Instance.ChangeLanguage(obj);
            this.Lang = obj;
        }
    }
}