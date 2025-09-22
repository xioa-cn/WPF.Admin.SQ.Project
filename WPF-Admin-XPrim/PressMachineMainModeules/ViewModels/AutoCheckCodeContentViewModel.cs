using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
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
    public partial class AutoCheckCodeContentViewModel : BindableBase {
        public static string AutoMainCheckCode => nameof(AutoMainCheckCode);
        public static string AutoPartialCheckCode => nameof(AutoPartialCheckCode);

        public static readonly string DirBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AutoCheckCode"
        );

        public static readonly string Dir = Path.Combine(DirBase, "Parameters");

        private List<string> _configNames = new List<string>();

        [ObservableProperty] private string _configName = string.Empty;
        public ObservableCollection<string> AutoCheckCodeParameterList { get; set; }

        [ObservableProperty] private string _nowUseAutoCheckCodeParameter = "未设置";

        [ObservableProperty] private AutoCheckCodeParameterModel _autoCheckCodeParameterModel;

        public int Step { get; set; }
        public AutoCheckCodeContentViewModel(int step) {
            this.Step = step;
            this.NowUseAutoCheckCodeParameter = AutoCheckCodeModelManager.HistoryAutoCheckCodeParameterName ?? "未设置";
            AutoCheckCodeParameterList = [];
            UpdateAutoCheckCodeParameterList();

            if (AutoCheckCodeModelManager.AutoCheckCodeParameterModelInstance is null)
            {
                this.AutoCheckCodeParameterModel = new AutoCheckCodeParameterModel {
                    ParameterMainModels = []
                };
                Enumerable.Range(0, AutoCheckCodeModelManager.Instance.AutoCheckCodeModel.CheckMainCodeSum).ToList()
                    .ForEach(i =>
                        this.AutoCheckCodeParameterModel.ParameterMainModels.Add(
                            new AutoCheckCodeParameterContent() {
                                Key = AutoMainCheckCode + (i + 1),
                                Step = step
                            }));
                this.AutoCheckCodeParameterModel.ParameterPartialModels =
                    new ObservableCollection<AutoCheckCodeParameterContent>();
                Enumerable.Range(0, AutoCheckCodeModelManager.Instance.AutoCheckCodeModel.CheckPartialCodeSum).ToList()
                    .ForEach(i =>
                        this.AutoCheckCodeParameterModel.ParameterPartialModels.Add(
                            new AutoCheckCodeParameterContent() {
                                Key = AutoPartialCheckCode + (i + 1),
                                Step = step
                            }));
            }
            else
            {
                this.AutoCheckCodeParameterModel = AutoCheckCodeModelManager.AutoCheckCodeParameterModelInstance;
            }
           
        }

        private void UpdateAutoCheckCodeParameterList() {
            AutoCheckCodeParameterList.Clear();
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
                        AutoCheckCodeParameterList.Add(name);
                        _configNames.Add(name);
                    }
                }
            });
        }

        [RelayCommand]
        private void Search() {
            if (string.IsNullOrEmpty(this.ConfigName)) return;
            AutoCheckCodeParameterList.Clear();

            _configNames.ForEach(item =>
            {
                if (item.Contains(this.ConfigName))
                {
                    AutoCheckCodeParameterList.Add(item);
                }
            });
        }

        [RelayCommand]
        private async Task Save() {
            if (!Directory.Exists(Dir))
            {
                Directory.CreateDirectory(Dir);
            }

            if (string.IsNullOrEmpty(ConfigName))
            {
                await AdminDialogHelper.ShowTextDialog("请输入有效的配置名称！",
                    HcDialogMessageToken.DialogCheckCodeToken);
                return;
            }

            string filename = Dir + $"\\{ConfigName}.json";
            if (File.Exists(filename))
            {
                var ret = await AdminDialogHelper.ShowTextDialog("配置文件已存在！\n点击确定将覆盖配置，或者选择取消。",
                    HcDialogMessageToken.DialogCheckCodeToken, buttontype: MessageBoxButton.OKCancel);

                if (ret != MessageBoxResult.OK)
                {
                    return;
                }

                File.Delete(filename);
            }

            await SerializeHelper.SerializeAsync(filename, this.AutoCheckCodeParameterModel);

            await AdminDialogHelper.ShowTextDialog("保存配置成功！",
                HcDialogMessageToken.DialogCheckCodeToken);

            UpdateAutoCheckCodeParameterList();
        }

        [RelayCommand]
        private async Task Delete(System.Windows.Controls.ListBox box) {
            if (box.SelectedIndex < 0)
            {
                await AdminDialogHelper.ShowTextDialog("请从列表中选择配置！",
                    HcDialogMessageToken.DialogCheckCodeToken);
                return;
            }

            var result = await AdminDialogHelper.ShowTextDialog($"请确定是否要删除{ConfigName}配置！",
                HcDialogMessageToken.DialogCheckCodeToken, buttontype: MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                var name = box.SelectedItem as string;
                string filename = Path.Combine(Dir, $"{name}.json");
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
                    HcDialogMessageToken.DialogCheckCodeToken);

                return;
            }

            var name = box.SelectedItem as string;
            if (string.IsNullOrEmpty(name))
            {
                await AdminDialogHelper.ShowTextDialog("请从列表中选择配置！",
                    HcDialogMessageToken.DialogCheckCodeToken);
                return;
            }

            Read(name);
            SnackbarHelper.Show("读取配方成功！！！");
        }

        private void Read(string name) {
            string filename = Dir + $"\\{name}.json";
            var jsonData = SerializeHelper.Deserialize<AutoCheckCodeParameterModel>(filename);

            foreach (var item in this.AutoCheckCodeParameterModel.ParameterMainModels)
            {
                var find = jsonData.ParameterMainModels
                    .FirstOrDefault(x => x.Key == item.Key);
                item.Value = find is not null ? find.Value : "未设置";
            }

            foreach (var item in this.AutoCheckCodeParameterModel.ParameterPartialModels)
            {
                var find = jsonData.ParameterPartialModels
                    .FirstOrDefault(x => x.Key == item.Key);
                item.Value = find is not null ? find.Value : "未设置";
            }

            //jsonData.ParameterMainModels.ToList().ForEach(item =>
            //{
            //    var find = this.AutoCheckCodeParameterModel.ParameterMainModels
            //        .FirstOrDefault(x => x.Key == item.Key);
            //    if (find is not null)
            //    {
            //        find.Value = item.Value;
            //    }
            //});

            //jsonData.ParameterPartialModels.ToList().ForEach(item =>
            //{
            //    var find = this.AutoCheckCodeParameterModel.ParameterPartialModels
            //        .FirstOrDefault(x => x.Key == item.Key);
            //    if (find is not null)
            //        find.Value = item.Value;
            //});
        }

        [RelayCommand]
        private async Task UseParameter(System.Windows.Controls.ListBox box) {
            if (box.SelectedIndex < 0)
            {
                await AdminDialogHelper.ShowTextDialog("请从列表中选择配置！",
                    HcDialogMessageToken.DialogCheckCodeToken);

                return;
            }

            var name = box.SelectedItem as string;
            if (string.IsNullOrEmpty(name))
            {
                await AdminDialogHelper.ShowTextDialog("请从列表中选择配置！",
                    HcDialogMessageToken.DialogCheckCodeToken);
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