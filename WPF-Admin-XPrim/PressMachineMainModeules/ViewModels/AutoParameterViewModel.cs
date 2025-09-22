using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HandyControl.Controls;
using PressMachineMainModeules.Models;
using PressMachineMainModeules.Utils;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using WPF.Admin.Models;
using WPF.Admin.Models.Background;
using WPF.Admin.Models.Models;
using WPF.Admin.Models.Utils;
using WPF.Admin.Service.Logger;
using WPF.Admin.Service.Services;
using WPF.Admin.Themes.Converter;
using WPF.Admin.Themes.W_Dialogs;

namespace PressMachineMainModeules.ViewModels
{
    public partial class AutoParameterViewModel : BindableBase
    {
        public static readonly string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Paramster");
        [ObservableProperty] private AutoParameterModel? _autoParameterModel;
        [ObservableProperty] private string? configName;

        public ObservableCollection<string> ConfigNames { get; set; }
            = new ObservableCollection<string>();

        public AutoParameterViewModel()
        {
            Task.Run(() =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    // TODO 耗时
                    AutoParameterModel = AutoParameterModelManager.Instance.AutoParameterModel;
                    UpdateConfigList();
                });
            });
        }

        public ObservableCollection<string> BaseConfigNames { get; set; } = new ObservableCollection<string>();

        private void UpdateConfigList()
        {
            CreateDirPath.CreateFolderIfNotExist(dir);
            string[] files = Directory.GetFiles(dir);
            ConfigNames.Clear();
            BaseConfigNames.Clear();
            foreach (var file in files)
            {
                if (file.EndsWith(".json"))
                {
                    string name = Path.GetFileNameWithoutExtension(file);
                    ConfigNames.Add(name);
                    BaseConfigNames.Add(name);
                }
            }
        }


        [RelayCommand]
        private async Task Save()
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (string.IsNullOrEmpty(ConfigName))
            {
                await AdminDialogHelper.ShowTextDialog("请输入有效的配置名称！",
                    HcDialogMessageToken.DialogPressMachineParametersToken);
                return;
            }

            string filename = dir + $"\\{ConfigName}.json";
            if (File.Exists(filename))
            {
                var ret = await AdminDialogHelper.ShowTextDialog("配置文件已存在！\n点击确定将覆盖配置，或者选择取消。",
                    HcDialogMessageToken.DialogPressMachineParametersToken, buttontype: MessageBoxButton.OKCancel);
                // var ret = HandyControl.Controls.MessageBox.Show(
                //     "配置文件已存在！\n点击确定将覆盖配置，或者选择取消。",
                //     "警告",
                //     MessageBoxButton.OKCancel,
                //     MessageBoxImage.Warning);
                if (ret != MessageBoxResult.OK)
                {
                    return;
                }

                File.Delete(filename);
            }


            var data = AutoParameterModel?.ToSaveEntity();
            SerializeHelper.Serialize(filename, data);


            await AdminDialogHelper.ShowTextDialog("保存配置成功！",
                HcDialogMessageToken.DialogPressMachineParametersToken);
            UpdateConfigList();
        }

        [RelayCommand]
        private async Task Read(System.Windows.Controls.ListBox box)
        {
            if (box.SelectedIndex < 0)
            {
                await AdminDialogHelper.ShowTextDialog("请从列表中选择配置！",
                    HcDialogMessageToken.DialogPressMachineParametersToken);

                return;
            }

            var name = box.SelectedItem as string;
            if (string.IsNullOrEmpty(name))
            {
                await AdminDialogHelper.ShowTextDialog("请从列表中选择配置！",
                    HcDialogMessageToken.DialogPressMachineParametersToken);
                return;
            }

            Read(name);
            Growl.SuccessGlobal("读取本地程序成功");
        }

        private void Read(string name)
        {
            QueuedHostedService.TaskQueue.QueueBackgroundWorkItem(async (token) =>
            {
                string filename = dir + $"\\{name}.json";
                var jsonData = SerializeHelper.Deserialize<AutoParameterSaveEntity>(filename);

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    AutoParameterModel?.GetSaveEntity(jsonData);

                    UpdateConfigList();
                });
            });
        }

        [RelayCommand]
        private async Task Delete(System.Windows.Controls.ListBox box)
        {
            if (box.SelectedIndex < 0)
            {
                await AdminDialogHelper.ShowTextDialog("请从列表中选择配置！",
                    HcDialogMessageToken.DialogPressMachineParametersToken);
                return;
            }

            var result = await AdminDialogHelper.ShowTextDialog($"请确定是否要删除{ConfigName}配置！",
                HcDialogMessageToken.DialogPressMachineParametersToken, buttontype: MessageBoxButton.YesNo);
            // var result = HandyControl.Controls.MessageBox.Show($"请确定是否要删除{ConfigName}配置！",
            //     "提示",
            //     MessageBoxButton.YesNo,
            //     MessageBoxImage.Information);
            if (result == MessageBoxResult.Yes)
            {
                var name = box.SelectedItem as string;
                string filename = Path.Combine(dir, $"{name}.json");
                if (!File.Exists(filename))
                {
                    UpdateConfigList();
                    return;
                }

                File.Delete(filename);
                Growl.SuccessGlobal("删除成功！");
                //HandyControl.Controls.MessageBox.Show(
                // "删除成功！",
                // "提示",
                // MessageBoxButton.OK,
                // MessageBoxImage.Information);
                this.ConfigName = "";
                UpdateConfigList();
            }
            else
            {
                Growl.WarningGlobal($"取消删除操作");
            }
        }

        [RelayCommand]
        private async Task UseParamster()
        {
            if (string.IsNullOrWhiteSpace(ConfigName))
            {
                await AdminDialogHelper.ShowTextDialog("请先选择配方！",
                    HcDialogMessageToken.DialogPressMachineParametersToken);

                return;
            }

            var has = BaseConfigNames.Contains(ConfigName);
            if (!has)
            {
                Growl.ErrorGlobal($"配方{ConfigName}不存在！！！");
                return;
            }

            Read(ConfigName);


            Growl.SuccessGlobal($"正在加载配方{ConfigName}到PLC！！！");
            QueuedHostedService.TaskQueue.QueueBackgroundWorkItem(async (token) =>
            {
                string filename = dir + $"\\{ConfigName}.json";
                var jsonData = SerializeHelper.Deserialize<AutoParameterSaveEntity>(filename);
                AutoParameterModel m = DeepCopyHelper.JsonClone(AutoParameterModelManager.Instance.AutoParameterModel);
                m?.GetSaveEntity(jsonData, false);
                try
                {
                    if (AutoParameterModel is null)
                    {
                        Growl.ErrorGlobal("参数写入错误，全局参数集合异常");
                        return;
                    }

                    var result = await m.WriteToPlcAsync();
                    var weakModel = m.ToAutoParameterWeakRefEntity(ConfigName);
                    WeakReferenceMessenger.Default.Send(weakModel);
                    if (result)
                    {
                        Growl.SuccessGlobal("参数写入成功");
                    }
                    else
                    {
                        Growl.ErrorGlobal("参数写入失败请检查PLC连接");
                    }
                }
                catch (Exception ex)
                {
                    XLogGlobal.Logger?.LogError(ex.Message, ex);
                }
            });
        }


        [RelayCommand]
        private void ReadPlcContent()
        {
            Task.Run(async () =>
            {
                try
                {
                    if (AutoParameterModel is null)
                    {
                        Growl.ErrorGlobal("程序错误，参数没有加载正确");
                        return;
                    }

                    var result = await AutoParameterModel.ReadToPlcAsync();
                    if (result)
                    {
                        Growl.SuccessGlobal("参数读取成功");
                    }
                    else
                    {
                        Growl.ErrorGlobal("参数读取失败请检查PLC连接");
                    }
                }
                catch (Exception ex)
                {
                    XLogGlobal.Logger?.LogError(ex.Message, ex);
                }
            });
        }

        [RelayCommand]
        private void Search()
        {
            ConfigNames.Clear();
            if (string.IsNullOrWhiteSpace(this.ConfigName))
            {
                foreach (var item in BaseConfigNames)
                {
                    ConfigNames.Add(item);
                }
            }
            else
            {
                foreach (var item in BaseConfigNames)
                {
                    if (item.Contains(this.ConfigName))
                    {
                        ConfigNames.Add(item);
                    }
                }
            }

            Growl.SuccessGlobal($"加载完成");
        }
    }
}