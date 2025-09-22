using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Collections;
using HandyControl.Controls;
using PressMachineMainModeules.Models;
using PressMachineMainModeules.Utils;
using WPF.Admin.Models;
using WPF.Admin.Models.Models;
using WPF.Admin.Service.Services;
using WPF.Admin.Themes.Converter;
using WPF.Admin.Themes.Helper;
using WPF.Admin.Themes.W_Dialogs;

namespace PressMachineMainModeules.ViewModels {
    public partial class BarcodeCharacteristicsViewModel : BindableBase {
        public ObservableCollection<NowVersionCheckCodeModel> NowVersionCheckCodeModels { get; set; } =
            NowVersionCheckCodeManager.Instance.NowVersionCheckCodeModels;

        [ObservableProperty] private string _configName = string.Empty;
        [ObservableProperty] private string _nowUseAutoCheckCodeParameter = "未设置";

        private List<string> _configNames = new List<string>();

        public ObservableCollection<string> PartialParametersList { get; set; } =
            new ManualObservableCollection<string>();

        public BarcodeCharacteristicsViewModel() {
            this.NowUseAutoCheckCodeParameter = AutoCheckCodeModelManager.HistoryAutoCheckCodeParameterName ?? "未设置";
            UpdateAutoCheckCodeParameterList();
        }

        private void UpdateAutoCheckCodeParameterList() {
            PartialParametersList.Clear();
            _configNames.Clear();
            CreateDirPath.CreateFolderIfNotExist(AutoCheckCodeContentViewModel.Dir);
            string[] files = Directory.GetFiles(AutoCheckCodeContentViewModel.Dir);

            if (files.Length < 1) return;

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                foreach (var file in files)
                {
                    if (file.EndsWith(".json"))
                    {
                        string name = Path.GetFileNameWithoutExtension(file);
                        PartialParametersList.Add(name);
                        _configNames.Add(name);
                    }
                }
            });
        }

        [RelayCommand]
        private void Search() {
            if (string.IsNullOrEmpty(this.ConfigName)) return;
            PartialParametersList.Clear();

            _configNames.ForEach(item =>
            {
                if (item.Contains(this.ConfigName))
                {
                    PartialParametersList.Add(item);
                }
            });
        }

        [RelayCommand]
        private async Task Save() {
            if (!Directory.Exists(AutoCheckCodeContentViewModel.Dir))
            {
                Directory.CreateDirectory(AutoCheckCodeContentViewModel.Dir);
            }

            if (string.IsNullOrEmpty(ConfigName))
            {
                await AdminDialogHelper.ShowTextDialog("请输入有效的配置名称！",
                    HcDialogMessageToken.DialogPartialCodeToken);
                return;
            }

            string filename = AutoCheckCodeContentViewModel.Dir + $"\\{ConfigName}.json";
            if (File.Exists(filename))
            {
                var ret = await AdminDialogHelper.ShowTextDialog("配置文件已存在！\n点击确定将覆盖配置，或者选择取消。",
                    HcDialogMessageToken.DialogPartialCodeToken, buttontype: MessageBoxButton.OKCancel);

                if (ret != MessageBoxResult.OK)
                {
                    return;
                }

                File.Delete(filename);
            }

            await SerializeHelper.SerializeAsync(filename, this.NowVersionCheckCodeModels);

            await AdminDialogHelper.ShowTextDialog("保存配置成功！",
                HcDialogMessageToken.DialogPartialCodeToken);

            UpdateAutoCheckCodeParameterList();
        }

        [RelayCommand]
        private async Task Delete(System.Windows.Controls.ListBox box) {
            if (box.SelectedIndex < 0)
            {
                await AdminDialogHelper.ShowTextDialog("请从列表中选择配置！",
                    HcDialogMessageToken.DialogPartialCodeToken);
                return;
            }

            var result = await AdminDialogHelper.ShowTextDialog($"请确定是否要删除{ConfigName}配置！",
                HcDialogMessageToken.DialogPartialCodeToken, buttontype: MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                var name = box.SelectedItem as string;
                string filename = Path.Combine(AutoCheckCodeContentViewModel.Dir, $"{name}.json");
                if (!File.Exists(filename))
                {
                    UpdateAutoCheckCodeParameterList();
                    return;
                }

                File.Delete(filename);
                Growl.SuccessGlobal("删除成功！");
                this.ConfigName = "";
                UpdateAutoCheckCodeParameterList();
            }
            else
            {
                Growl.WarningGlobal($"取消删除操作");
            }
        }

        [RelayCommand]
        private async Task Read(System.Windows.Controls.ListBox box) {
            if (box.SelectedIndex < 0)
            {
                await AdminDialogHelper.ShowTextDialog("请从列表中选择配置！",
                    HcDialogMessageToken.DialogPartialCodeToken);

                return;
            }

            var name = box.SelectedItem as string;
            if (string.IsNullOrEmpty(name))
            {
                await AdminDialogHelper.ShowTextDialog("请从列表中选择配置！",
                    HcDialogMessageToken.DialogPartialCodeToken);
                return;
            }

            Read(name);
            SnackbarHelper.Show("读取配方成功！！！");
        }

        private void Read(string name) {
            string filename = AutoCheckCodeContentViewModel.Dir + $"\\{name}.json";
            var jsonData = SerializeHelper.Deserialize<ObservableCollection<NowVersionCheckCodeModel>>(filename);
            //this.NowVersionCheckCodeModels.Clear();
            foreach (var item in this.NowVersionCheckCodeModels)
            {
                var find = jsonData.FirstOrDefault(e => e.AutoModeName == item.AutoModeName);
                if (find is null) continue;
               
                item.OpenStep = find.OpenStep;
                if (item.CheckRolaValueSetps is null)
                {
                    continue;
                }
                foreach (var itemCheckRolaValueSetp in item.CheckRolaValueSetps)
                {
                    var findValue =
                        find.CheckRolaValueSetps?.FirstOrDefault(e => e.TokenKey == itemCheckRolaValueSetp.TokenKey);
                    if(findValue is null) continue;
                    itemCheckRolaValueSetp.CheckRolaValues = findValue.CheckRolaValues;
                    itemCheckRolaValueSetp.Open = findValue.Open;
                    itemCheckRolaValueSetp.MainCodeRola = findValue.MainCodeRola;
                }
            }
        }

        [RelayCommand]
        private async Task UseParameter(System.Windows.Controls.ListBox box) {
            if (box.SelectedIndex < 0)
            {
                await AdminDialogHelper.ShowTextDialog("请从列表中选择配置！",
                    HcDialogMessageToken.DialogPartialCodeToken);

                return;
            }

            var name = box.SelectedItem as string;
            if (string.IsNullOrEmpty(name))
            {
                await AdminDialogHelper.ShowTextDialog("请从列表中选择配置！",
                    HcDialogMessageToken.DialogPartialCodeToken);
                return;
            }

            Read(name);
            //WeakReferenceMessenger.Default.Send();
            this.NowUseAutoCheckCodeParameter = name;
            await AutoCheckCodeModelManager.SaveHistoryAutoCheckCodeParameterNameAsync(name);
            Growl.SuccessGlobal("配方使用成功");
        }
    }
}