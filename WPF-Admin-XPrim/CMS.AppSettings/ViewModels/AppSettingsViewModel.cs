using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using WPF.Admin.Models;
using WPF.Admin.Models.Models;
using WPF.Admin.Themes.Converter;
using MessageBox = HandyControl.Controls.MessageBox;
using WPF.Admin.Models.Utils;

namespace CMS.AppSettings.ViewModels
{
    public partial class AppContentModel(string itemName, object? value, string propertyName) : BindableBase
    {
        [ObservableProperty] private string _Key = itemName;

        [ObservableProperty] private object? _Value = value;

        public string PropertyName { get; set; } = propertyName;
    }

    public partial class AppSettingsViewModel : BindableBase
    {
        [ObservableProperty] private bool _CodeChange;

        public ObservableCollection<AppContentModel> ContentModels { get; set; } = new();

        private string? _appFile;

        private string AppFile
        {
            get
            {
                return _appFile ??= ApplicationConfigConst.SettingJsonFile;
            }
        }

        public AppSettingsViewModel()
        {
            if (!System.IO.File.Exists(AppFile))
            {
                throw new FieldAccessException(nameof(AppFile));
            }

            var reader = System.IO.File.ReadAllText(AppFile);
            var settings = System.Text.Json.JsonSerializer.Deserialize<WPF.Admin.Models.Models.AppSettings>(reader);

            var properties = settings?.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            if (properties is null)
            {
                return;
            }

            foreach (PropertyInfo item in properties)
            {
                var desc = item.GetCustomAttributes().FirstOrDefault(e => e.GetType() == typeof(DescriptionAttribute));
                if (desc is DescriptionAttribute desca && !string.IsNullOrEmpty(desca.Description))
                {
                    ContentModels.Add(new AppContentModel(desca.Description, item.GetValue(settings), item.Name));
                }
                else
                {
                    ContentModels.Add(new AppContentModel(item.Name, item.GetValue(settings), item.Name));
                }
            }
        }


        private RelayCommand? _saveConCommand;

        public RelayCommand? SaveConCommand
        {
            get { return _saveConCommand ??= new RelayCommand(Save, () => true); }
        }

        private void Save()
        {
            var result = MessageBox.Show("是否同时保存到sln源代码和程序配置中？", "提示", MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Cancel)
            {
                return;
            }

            try
            {
                var temp = CreateAppSettings();
                SaveDebugSettings(temp);

                if (result == MessageBoxResult.Yes)
                {
                    SaveSlnSolution(temp);
                }
                Growl.SuccessGlobal("保存成功");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveSlnSolution(WPF.Admin.Models.Models.AppSettings dto)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo path = new DirectoryInfo(currentPath);
            var dir = string.Empty;
            if (path.Parent.Parent.FullName.EndsWith("WPFAdmin"))
            {
                dir = path.Parent.Parent.FullName;
            }
            else if (path.Parent.Parent.Parent.FullName.EndsWith("WPFAdmin"))
            {
                dir = path.Parent.Parent.Parent.FullName;
            }
            else
            {
                MessageBox.Show("没有找到源代码");
                return;
            }

            var slnFilePath = System.IO.Path.Combine(dir, ApplicationConfigConst.SettingJsonFileName);

            if (System.IO.File.Exists(slnFilePath))
            {
                SerializeHelper.Serialize(slnFilePath, dto);
            }
            else
            {
                MessageBox.Show("没有找到源配置");
                return;
            }

        }

        private void SaveDebugSettings(WPF.Admin.Models.Models.AppSettings dto)
        {
            SerializeHelper.Serialize(AppFile, dto);
        }

        private WPF.Admin.Models.Models.AppSettings CreateAppSettings()
        {
            WPF.Admin.Models.Models.AppSettings settings = new WPF.Admin.Models.Models.AppSettings();
            var st = settings.GetType();
            foreach (var item in ContentModels)
            {
                var property = st.GetProperty(item.PropertyName);
                if (property is null)
                {
                    continue;
                }

                if (item.PropertyName.Contains("ApplicationVersions"))
                {
                    property.SetValue(settings, Enum.Parse<ApplicationVersions>(item.Value?.ToString()), null);
                }
                else if (double.TryParse(item.Value.ToString(), out var value))
                {
                    property.SetValue(settings, value, null);
                }
                else if (item.Value.ToString() == "True")
                {
                    property.SetValue(settings, true, null);
                }
                else if (item.Value.ToString() == "False")
                {
                    property.SetValue(settings, false, null);
                }
                else
                {
                    property.SetValue(settings, item.Value, null);
                }
            }

            return settings;
        }
    }
}