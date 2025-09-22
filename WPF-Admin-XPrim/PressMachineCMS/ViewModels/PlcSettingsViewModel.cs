using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using PressMachineCMS.Models;
using System.Text.Json;
using System.IO;
using System.Windows;
using CMS.ReaderConfigLIbrary.Models;
using PressMachineCMS.Config;
using WPF.Admin.Models;

namespace PressMachineCMS.ViewModels {
    public partial class PlcSettingsViewModel : BindableBase {
        private PlcViewSettingModel _settings;
        private PlcViewSettingContent _selectedPlc;

        public PlcSettingsViewModel() {
            Settings = new PlcViewSettingModel {
                Contents = new ObservableCollection<PlcViewSettingContent>()
            };
        }

        public PlcViewSettingModel Settings {
            get => _settings;
            set => SetProperty(ref _settings, value);
        }

        public PlcViewSettingContent SelectedPlc {
            get => _selectedPlc;
            set
            {
                SetProperty(ref _selectedPlc, value);
                _deletePlcCommand?.NotifyCanExecuteChanged();
            }
        }

        private RelayCommand? _addPlcCommand;
        public ICommand AddPlcCommand => _addPlcCommand ??= new RelayCommand(AddPlc);

        private void AddPlc() {
            var plc = new PlcViewSettingContent {
                Key = "PLC" + (Settings.Contents.Count + 1),
                Ip = "192.168.0.1",
                Desc = "新PLC",
                Type = PlcType.Modbus_TCP,
                Port = 502,
                Station = 1,
                ConnectTimeOut = 2000,
                ReceiveTimeOut = 5000,
                DataFormat = true,
                SerialPort = "COM1",
                BaudRate = 9600,
                DataBits = 8,
                StopBits = 1,
                Parity = 0
            };
            Settings.Contents.Add(plc);
            SelectedPlc = plc;
        }

        private RelayCommand? _deletePlcCommand;
        public ICommand DeletePlcCommand => _deletePlcCommand ??= new RelayCommand(DeletePlc, CanDeletePlc);

        private void DeletePlc() {
            if (SelectedPlc != null)
            {
                Settings.Contents.Remove(SelectedPlc);
                SelectedPlc = null;
            }
        }

        private bool CanDeletePlc() => SelectedPlc != null;

        private RelayCommand? _saveCommand;
        public ICommand SaveCommand => _saveCommand ??= new RelayCommand(Save);

        private void Save() {
            try
            {
                string msg;
                if (!Settings.Verification(out msg))
                {
                    MessageBox.Show(msg, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var options = new JsonSerializerOptions { WriteIndented = true,Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping, };
                string jsonString = JsonSerializer.Serialize(Settings, options);
                File.WriteAllText(CMSConfig.ConfigJsonDir("plcSettings.json"), jsonString);
                MessageBox.Show("保存成功！", "提示");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存失败：{ex.Message}", "错误");
            }
        }

        private RelayCommand? _loadCommand;
        public ICommand LoadCommand => _loadCommand ??= new RelayCommand(Load);

        private void Load() {
            try
            {
                var dialog = new Microsoft.Win32.OpenFileDialog {
                    Filter = "JSON 文件|*.json|所有文件|*.*"
                };

                if (dialog.ShowDialog() == true)
                {
                    string jsonString = File.ReadAllText(dialog.FileName);
                    var options = new JsonSerializerOptions { WriteIndented = true ,Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,};
                    Settings = JsonSerializer.Deserialize<PlcViewSettingModel>(jsonString, options);
                    MessageBox.Show("加载成功！", "提示");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载失败：{ex.Message}", "错误");
            }
        }
    }
}