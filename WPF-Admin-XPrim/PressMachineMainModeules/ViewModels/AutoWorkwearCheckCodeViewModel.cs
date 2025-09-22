using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PressMachineMainModeules.Models;
using PressMachineMainModeules.Utils;
using WPF.Admin.Models;
using WPF.Admin.Models.Models;
using WPF.Admin.Models.Utils;
using WPF.Admin.Service.Logger;
using WPF.Admin.Service.Services;
using WPF.Admin.Themes.Converter;
using WPF.Admin.Themes.Helper;
using WPF.Admin.Themes.W_Dialogs;

namespace PressMachineMainModeules.ViewModels {
    public partial class AutoWorkwearCheckCodeViewModel : BindableBase {
        public AutoCheckCodeModel AutoCheckCodeModel { get; } =
            AutoCheckCodeModelManager.Instance.AutoCheckCodeModel;
        
        public AutoWorkwearCheckCodeViewModel() {
            Initialized();
        }

        #region 获取配方列表

        public ObservableCollection<string> PressMachineParameterList { get; set; } =
            new ObservableCollection<string>();

        [RelayCommand]
        public void GetConfigList() {
            PressMachineParameterList.Clear();
            CreateDirPath.CreateFolderIfNotExist(AutoParameterViewModel.dir);
            string[] files = Directory.GetFiles(AutoParameterViewModel.dir);

            if (files.Length < 1) return;

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                foreach (var file in files)
                {
                    if (file.EndsWith(".json"))
                    {
                        string name = Path.GetFileNameWithoutExtension(file);
                        PressMachineParameterList.Add(name);
                    }
                }
            });
        }

        #endregion

        private void Initialized() {
            GetConfigList();
            RefreshAutoWorkwearBindingParameters();
            RefreshAutoWorkwearCodeList();
        }

        #region WorkwearBindingParameters

        public ObservableCollection<AutoWorkwearBindingParam> AutoWorkwearBindingParameters { get; set; } =
            new ObservableCollection<AutoWorkwearBindingParam>();

        public void RefreshAutoWorkwearBindingParameters() {
            if (!System.IO.File.Exists(ApplicationConfigConst.AutoWorkwearBindingParametersFilePath))
            {
                return;
            }

            var list = SerializeHelper.Deserialize<AutoWorkwearBindingParam[]>(
                ApplicationConfigConst
                    .AutoWorkwearBindingParametersFilePath);
            this.AutoWorkwearBindingParameters.Clear();
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                list.ToList().ForEach(item => this.AutoWorkwearBindingParameters.Add(item));
            });
        }

        public void SaveAutoWorkwearBindingParametersList() {
            try
            {
                SerializeHelper.Serialize(ApplicationConfigConst.AutoWorkwearBindingParametersFilePath,
                    this.AutoWorkwearBindingParameters.ToArray());

                AutoCheckCodeModelManager.Instance.AutoWorkwearRolaCodeEntities.RemoveAll(e =>
                    e.AutoRolaCodeType == AutoRolaCodeType.Workwear);
                this.AutoWorkwearBindingParameters.ToList().ForEach(e =>
                {
                    AutoCheckCodeModelManager.Instance.AutoWorkwearRolaCodeEntities.Add(new AutoWorkwearRolaCodeEntity(0) {
                        RolaString = e.WorkwearName + ApplicationConfigConst.AutoModeJoinChar + e.ParamerterName,
                        AutoRolaCodeType = AutoRolaCodeType.Workwear
                    });
                });
            }
            catch (Exception e)
            {
                XLogGlobal.Logger?.LogError(e.Message, e);
            }
        }

        #endregion


        #region WorkwearCode

        public void RefreshAutoWorkwearCodeList() {
            if (!System.IO.File.Exists(ApplicationConfigConst.AutoCheckCodeWorkwearFilePath))
            {
                return;
            }

            var list = SerializeHelper.Deserialize<string[]>(ApplicationConfigConst.AutoCheckCodeWorkwearFilePath);
            this.AutoWorkwearCodeList.Clear();
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                list.ToList().ForEach(item => this.AutoWorkwearCodeList.Add(item));
            });
        }

        private void SaveAutoWorkwearCodeList() {
            try
            {
                SerializeHelper.Serialize(ApplicationConfigConst.AutoCheckCodeWorkwearFilePath,
                    this.AutoWorkwearCodeList.ToArray());
                SaveAutoWorkwearBindingParametersList();
                SnackbarHelper.Show("操作成功");
            }
            catch (Exception e)
            {
                SnackbarHelper.Show($"操作失败{e.Message}");
                XLogGlobal.Logger?.LogError(e.Message, e);
            }
        }

        public ObservableCollection<string> AutoWorkwearCodeList { get; set; } = new ObservableCollection<string>();

        [RelayCommand]
        private void RomoveWorkwearCode(string code) {
            if (!AutoWorkwearCodeList.Contains(code))
            {
                return;
            }

            AutoWorkwearCodeList.Remove(code);
            var find = this.AutoWorkwearBindingParameters.FirstOrDefault(e => e.WorkwearName == code);
            if (find is not null)
            {
                this.AutoWorkwearBindingParameters.Remove(find);
            }

            SaveAutoWorkwearCodeList();
        }

        [RelayCommand]
        private async Task AddWorkwearCode() {
            var result = await AdminDialogHelper.ShowInputTextDialog("请输入工装码",
                WPF.Admin.Models.Models.HcDialogMessageToken.DialogCheckCodeToken,
                buttontype: MessageBoxButton.OKCancel);

            if (result.Item1 != MessageBoxResult.OK) return;
            var inputText = result.Item2.Replace(" ", "");

            if (string.IsNullOrEmpty(inputText))
            {
                await AdminDialogHelper.ShowTextDialog("工装码不能为空",
                    WPF.Admin.Models.Models.HcDialogMessageToken.DialogCheckCodeToken);
                return;
            }

            if (this.AutoWorkwearCodeList.Contains(inputText))
            {
                await AdminDialogHelper.ShowTextDialog("工装码已存在",
                    WPF.Admin.Models.Models.HcDialogMessageToken.DialogCheckCodeToken);
                return;
            }

            this.AutoWorkwearCodeList.Add(inputText);
            this.AutoWorkwearBindingParameters.Add(new AutoWorkwearBindingParam { WorkwearName = inputText });
            SaveAutoWorkwearCodeList();
        }

        #endregion
    }
}