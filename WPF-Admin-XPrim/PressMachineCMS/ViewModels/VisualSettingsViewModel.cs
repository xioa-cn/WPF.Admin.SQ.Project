using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using PressMachineCMS.Models;
using System.Text.Json;
using System.IO;
using System.Text.Encodings.Web;
using System.Windows;
using PressMachineCMS.Config;
using WPF.Admin.Models;

namespace PressMachineCMS.ViewModels
{
    public class VisualSettingsViewModel : BindableBase
    {
        private VisualViewSettingModel _settings;
        private VisualContent _selectedContent;
        private VisualContentContent _selectedContentContent;
        private VisualContentContent _selectedShowContent;  // 添加显示内容选择

        public VisualSettingsViewModel()
        {
            Settings = new VisualViewSettingModel
            {
                Column = 2,
                Content = new ObservableCollection<VisualContent>()
            };
        }

        public VisualViewSettingModel Settings
        {
            get => _settings;
            set => SetProperty(ref _settings, value);
        }

        public VisualContent SelectedContent
        {
            get => _selectedContent;
            set
            {
                SetProperty(ref _selectedContent, value);
                _deleteContentCommand?.NotifyCanExecuteChanged();
                _addContentContentCommand?.NotifyCanExecuteChanged();
            }
        }

        public VisualContentContent SelectedContentContent
        {
            get => _selectedContentContent;
            set
            {
                SetProperty(ref _selectedContentContent, value);
                _deleteContentContentCommand?.NotifyCanExecuteChanged();
            }
        }

        private RelayCommand? _addContentCommand;
        public ICommand AddContentCommand => _addContentCommand ??= new RelayCommand(AddContent);

        private void AddContent()
        {
            var content = new VisualContent
            {
                VisualContentContent = new ObservableCollection<VisualContentContent>(),
                ShowVisualContent = new ObservableCollection<VisualContentContent>()  // 添加初始化
            };
            Settings.Content.Add(content);
            SelectedContent = content;
        }

        private RelayCommand? _deleteContentCommand;

        public ICommand DeleteContentCommand =>
            _deleteContentCommand ??= new RelayCommand(DeleteContent, CanDeleteContent);

        private void DeleteContent()
        {
            if (SelectedContent != null)
            {
                Settings.Content.Remove(SelectedContent);
                SelectedContent = null;
            }
        }

        private bool CanDeleteContent() => SelectedContent != null;

        private RelayCommand? _addContentContentCommand;

        public ICommand AddContentContentCommand =>
            _addContentContentCommand ??= new RelayCommand(AddContentContent, CanAddContentContent);

        private void AddContentContent()
        {
            if (SelectedContent != null)
            {
                var contentContent = new VisualContentContent();
                SelectedContent.VisualContentContent.Add(contentContent);
                SelectedContent.HavingVisualContentContent = true;
                SelectedContentContent = contentContent;
            }
        }

        private bool CanAddContentContent() => SelectedContent != null;

        private RelayCommand? _deleteContentContentCommand;

        public ICommand DeleteContentContentCommand => _deleteContentContentCommand ??=
            new RelayCommand(DeleteContentContent, CanDeleteContentContent);

        private void DeleteContentContent()
        {
            if (SelectedContent != null && SelectedContentContent != null)
            {
                SelectedContent.VisualContentContent.Remove(SelectedContentContent);
                if (SelectedContent.VisualContentContent.Count == 0)
                {
                    SelectedContent.HavingVisualContentContent = false;
                }

                SelectedContentContent = null;
            }
        }

        private bool CanDeleteContentContent() => SelectedContentContent != null;

        private RelayCommand _saveCommand;
        public ICommand SaveCommand => _saveCommand ??= new RelayCommand(Save);

        private JsonSerializerOptions options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
        };

        private void Save()
        {
            try
            {
                // 确保所有集合已初始化
                if (Settings.Content == null)
                {
                    Settings.Content = new ObservableCollection<VisualContent>();
                }

                foreach (var content in Settings.Content)
                {
                    if (content.VisualContentContent == null)
                    {
                        content.VisualContentContent = new ObservableCollection<VisualContentContent>();
                    }
                }

                string jsonString = JsonSerializer.Serialize(Settings, options);
                File.WriteAllText(CMSConfig.ConfigJsonDir("visualSettings.json"), jsonString);
                MessageBox.Show("保存成功！", "提示");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存失败：{ex.Message}\n{ex.InnerException?.Message}", "错误");
            }
        }

        private RelayCommand _loadCommand;
        public ICommand LoadCommand => _loadCommand ??= new RelayCommand(Load);

        private void Load()
        {
            try
            {
                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "JSON 文件|*.json|所有文件|*.*"
                };

                if (dialog.ShowDialog() == true)
                {
                    string jsonString = File.ReadAllText(dialog.FileName);
                    Settings = JsonSerializer.Deserialize<VisualViewSettingModel>(jsonString, options);
                   
                    MessageBox.Show("加载成功！", "提示");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载失败：{ex.Message}", "错误");
            }
        }

        private RelayCommand? _addShowContentCommand;
        public ICommand AddShowContentCommand => _addShowContentCommand ??= new RelayCommand(AddShowContent);

        private void AddShowContent()
        {
            if (SelectedContent != null)
            {
                if (SelectedContent.ShowVisualContent == null)
                {
                    SelectedContent.ShowVisualContent = new ObservableCollection<VisualContentContent>();
                }
                var showContent = new VisualContentContent
                {
                    Name = "新显示项",
                    Point = "DB1.0",
                    Unit = "mm"
                };
                SelectedContent.ShowVisualContent.Add(showContent);
                SelectedShowContent = showContent;
            }
        }

        private RelayCommand? _deleteShowContentCommand;
        public ICommand DeleteShowContentCommand => _deleteShowContentCommand ??= new RelayCommand(DeleteShowContent, CanDeleteShowContent);

        private void DeleteShowContent()
        {
            if (SelectedContent?.ShowVisualContent != null && SelectedShowContent != null)
            {
                SelectedContent.ShowVisualContent.Remove(SelectedShowContent);
                SelectedShowContent = null;
            }
        }

        private bool CanDeleteShowContent() => SelectedShowContent != null;

        public VisualContentContent SelectedShowContent
        {
            get => _selectedShowContent;
            set
            {
                SetProperty(ref _selectedShowContent, value);
                _deleteShowContentCommand?.NotifyCanExecuteChanged();
            }
        }
    }
}