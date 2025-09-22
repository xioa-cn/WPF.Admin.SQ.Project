using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PressMachineMainModeules.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;
using System.Windows;
using WPF.Admin.Models;
using WPF.Admin.Service.Services;
using PressMachineMainModeules.Utils;
using PressMachineMainModeules.Config;
using HandyControl.Controls;
using MessageBox = HandyControl.Controls.MessageBox;
using WPF.Admin.Themes.Converter;
using ListBox = System.Windows.Controls.ListBox;
using WPF.Admin.Service.Logger;

namespace PressMachineMainModeules.ViewModels
{
    public partial class PressMachineParamsViewModel : BindableBase
    {
        public PressMachineParamsViewModel()
        {
            this.Init();
            UpdateConfigList();
        }

        [ObservableProperty] private PressMachineCoreParamsDa currentRecipeDetail = new();
        [ObservableProperty] private string? configName;
        [ObservableProperty] private ObservableCollection<string> configNames = new();

        public ObservableCollection<string> BaseConfigNames { get; set; } = new ObservableCollection<string>();

        [ObservableProperty] private PressMachineSumNoContentName pressMachineSumNoContentName = new();

        private void UpdateConfigList()
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Paramster");
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

        /// <summary>
        /// 配方查找
        /// </summary>
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

        #region 配方CURD

        [RelayCommand]
        private void Read(ListBox box)
        {
            if (box.SelectedIndex < 0)
            {
                MessageBox.Show(
                    "请从列表中选择配置！",
                    "提示",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            var name = box.SelectedItem as string;
            Read(name!);
        }

        [RelayCommand]
        private void Save()
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Paramster");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (string.IsNullOrEmpty(ConfigName))
            {
                MessageBox.Show(
                    "请输入有效的配置名称！",
                    "提示",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            string filename = dir + $"\\{ConfigName}.json";
            if (File.Exists(filename))
            {
                var ret = MessageBox.Show(
                    "配置文件已存在！\n点击确定将覆盖配置，或者选择取消。",
                    "警告",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning);
                if (ret != MessageBoxResult.OK)
                {
                    return;
                }

                File.Delete(filename);
            }

            var data = CurrentRecipeDetail;
            SerializeHelper.Serialize(filename, data);

            MessageBox.Show("保存配置成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);

            UpdateConfigList();
        }

        [RelayCommand]
        private void Delete(ListBox box)
        {
            if (box.SelectedIndex < 0)
            {
                MessageBox.Show(
                    "请从列表中选择配置！",
                    "提示",
                    MessageBoxButton.OK
                );
                return;
            }


            var result = MessageBox.Show($"请确定是否要删除{ConfigName}配置！",
                "提示",
                MessageBoxButton.YesNo,
                MessageBoxImage.Information);
            if (result == MessageBoxResult.Yes)
            {
                var name = box.SelectedItem as string;
                Delete(name!);
            }
            else
            {
                Growl.WarningGlobal($"取消删除操作");
            }
        }


        [RelayCommand]
        private void UseParamster()
        {
            if (string.IsNullOrWhiteSpace(ConfigName))
            {
                Growl.ErrorGlobal($"请先选择配方！！！");
                return;
            }

            var has = BaseConfigNames.Contains(ConfigName);
            if (!has)
            {
                Growl.ErrorGlobal($"配方{ConfigName}不存在！！！");
                return;
            }

            UseBase(ConfigName);

        }


        private void UseBase(string name, bool isSaveConfigName = true)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            Read(name);
            PressMachieChangedOxyPlotViewWeak dto = new PressMachieChangedOxyPlotViewWeak()
            {
                Token = "",

                PressMachineCoreParamsDa = CurrentRecipeDetail,
                RecipeName = ConfigName,
            };
            WeakReferenceMessenger.Default.Send(dto);
            Growl.SuccessGlobal($"正在加载配方{name}到PLC！！！");
            Task.Run(async () =>
            {
                try
                {
                    await CurrentRecipeDetail.WriteSiemens3();
                    Growl.SuccessGlobal($"配方{name}应用完成！！！");
                }
                catch (Exception ex)
                {
                    XLogGlobal.Logger?.LogError(ex.Message, ex);
                }
            });

            if (isSaveConfigName)
            {
                PressMachineParam.ParasName = name;
                SerializeHelper.Serialize(ConfigPath, PressMachineParam);
                //PressMachineHistoryParams.Serialize(ConfigPath!, PressMachineParam!);
            }



        }

        #region

        private void Read(string name)
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Paramster");
            string filename = dir + $"\\{name}.json";
            CurrentRecipeDetail = SerializeHelper.Deserialize<PressMachineCoreParamsDa>(filename);

            ConfigName = name;

            UpdateConfigList();
        }

        private void Delete(string name)
        {
            string filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "Paramster",
                $"{name}.json");
            if (!File.Exists(filename))
            {
                UpdateConfigList();
                return;
            }

            File.Delete(filename);
            MessageBox.Show(
                "删除成功！",
                "提示",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            this.ConfigName = "";
            UpdateConfigList();
        }

        #endregion

        #endregion


        #region PLC内部参数读出

        [RelayCommand]
        private void ReadPlcContent(string index)
        {
            if (!Config.PlcConnect.GoOn)
            {
                MessageBox.Show("PLC未连接", "提示");
                return;
            }

            //if (index == null) return;
            //PressMachineParamsDa? point = index switch
            //{
            //    "1" => Config.UsePre.PressMachine10!,
            //    "2" => Config.UsePre.PressMachine20!,
            //    "3" => Config.UsePre.PressMachine30!,
            //    "4" => Config.UsePre.PressMachine40!,
            //    _ => null,
            //};

            Task.Run(async () =>
            {
                Growl.InfoGlobal("正在读取PLC");
                await CurrentRecipeDetail.ReadSiemens3();
                MessageBox.Show("提示", "读取成功");
            });



        }

        #endregion


        public static PressMachineHistoryParams? PressMachineParam { get; set; }
        private static string? ConfigPath { get; set; }

        static PressMachineParamsViewModel()
        {
            ConfigPath =
               Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "PressConfig.json");

            if (System.IO.File.Exists(ConfigPath))
            {
                PressMachineParam = SerializeHelper.Deserialize<PressMachineHistoryParams>(ConfigPath);
            }
            else
            {
                PressMachineParam = new PressMachineHistoryParams()
                {
                    ParasName = "Test",
                    ParasWriteIndex = false,
                    XUnit = "mm",
                    YUnit = "KN",
                    XName = "位移",
                    YName = "压力",
                    PositionUnitName = "mm",
                    PressUnitName = "KN",
                    SpeedUnitName = "mm/s",

                };
                SerializeHelper.Serialize(ConfigPath, PressMachineParam);
            }

            

        }

        #region 配方本地使用读取

        public void Init()
        {

            //PressMachineHistoryParams.Deserialize(ConfigPath);
            //UseBase(PressMachineParam.ParasName, false);
        }

        #endregion
    }
}