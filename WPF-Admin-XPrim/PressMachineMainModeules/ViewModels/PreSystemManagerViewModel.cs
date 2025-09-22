using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PressMachineMainModeules.Config;
using PressMachineMainModeules.Models;
using WPF.Admin.Models;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Windows;
using WPF.Admin.Themes.Helper;
using MessageBox = System.Windows.MessageBox;
using HandyControl.Controls;
using System.Drawing;
using WPF.Admin.Service.Services;
using PressMachineMainModeules.Utils;
using WPF.Admin.Themes.I18n;
using WPF.Admin.Themes.W_Dialogs;

namespace PressMachineMainModeules.ViewModels
{
    public partial class PreSystemManagerViewModel : BindableBase
    {
        [ObservableProperty] private SystemConfigModel _systemConfigModel = Common.SysConfigModel;

        public PreSystemManagerViewModel()
        {
            (t, _) = CSharpI18n.UseI18n();
        }
        

        [RelayCommand]
        private void Save()
        {

            try
            {
                var ips = this.SystemConfigModel.Ip.Split('.');
                if (ips.Length != 4)
                    throw new Exception(t!("Msg.PreSystem.IP_Error"));
                foreach (var item in ips)
                {
                    if (!int.TryParse(item, out var systemIp))
                    {
                        throw new Exception(t!("Msg.PreSystem.IP_Error"));
                    }

                    if (systemIp < 0 || systemIp > 255)
                    {
                        throw new Exception(t!("Msg.PreSystem.IP_Error"));
                    }
                }

                var comport = this.SystemConfigModel.Com.Replace("COM", "");
                if (!int.TryParse(comport, out var systemCom))
                {
                    throw new Exception(t!("Msg.PreSystem.Com_Error"));
                }


            }
            catch (Exception e)
            {
                SnackbarHelper.Show(e.Message);
                return;
            }

            // Task.Run(() =>
            // {
            //     WriteStatus(this.SystemConfigModel);
            // });

            this.SystemConfigModel.Save();
            SnackbarHelper.Show(t!("Msg.PreSystem.SaveSuccess"));
        }


        public static void WriteStatus(SystemConfigModel systemConfigModel)
        {
            if (PlcConnect.GoOn)
            {
                try
                {
                    var result = PlcConnect.Plc?.Write("DB1.2.2", systemConfigModel.DoorStatus);
                    if (!result.IsSuccess)
                    {
                        throw new Exception($"写入失败~{result.Message}");
                    }
                    result = PlcConnect.Plc?.Write("DB1.2.3", systemConfigModel.LightCurtainStatus);
                    result = PlcConnect.Plc?.Write("DB1.2.4", systemConfigModel.BuzzerStatus);
                    result = PlcConnect.Plc?.Write("DB1.2.5", systemConfigModel.CodeStatus);
                    result = PlcConnect.Plc?.Write("DB1.2.6", systemConfigModel.PCResultStatus);
                    result = PlcConnect.Plc?.Write("DB1.2.7", systemConfigModel.NGConStatus);
                    result = PlcConnect.Plc?.Write("DB1.3.0", systemConfigModel.InterStatus);
                    result = PlcConnect.Plc?.Write("DB2.0.0", systemConfigModel.压机传感器补偿);
                    result = PlcConnect.Plc?.Write("DB7.0.0", systemConfigModel.安全互锁屏蔽);
                    if (!result.IsSuccess)
                    {
                        throw new Exception($"写入失败~{result.Message}");
                    }
                }
                catch (Exception ex)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        Growl.ErrorGlobal($"SystemConfig 写入异常:{ex.Message}");
                    });

                }

            }
            else
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    Growl.WarningGlobal($"Plc Connect Warning~ 执行的操作为 SystemConfig");
                });
            }
        }

        [RelayCommand]
        private void BrowseFolder()
        {
            try
            {
                var dialog = new FolderBrowserDialog
                {
                    Description = t!("Msg.PreSystem.SelectDataFile"),
                    ShowNewFolderButton = true,  // 允许创建新文件夹
                    UseDescriptionForTitle = true // 使用Description作为标题
                };

                // 如果已有路径，则设置为初始路径
                if (!string.IsNullOrEmpty(SystemConfigModel.DataFolder) && Directory.Exists(
                        SystemConfigModel.DataFolder))
                {
                    dialog.SelectedPath = SystemConfigModel.DataFolder;
                }

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    SystemConfigModel.DataFolder = dialog.SelectedPath;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"选择文件夹出错: {ex.Message}");
                Growl.ErrorGlobal(t!("Msg.PreSystem.SelectFileError")
                    );
            }
        }

        [RelayCommand]
        private async Task InitializedPressMachineDb()
        {
            var result = await AdminDialogHelper.ShowTextDialog(t!("Msg.PreSystem.DeleteDb"), 
                WPF.Admin.Models.Models.HcDialogMessageToken.DialogPressMachineSystemToken, MessageBoxButton.YesNo);
            if(result == MessageBoxResult.Yes)
            {
                await PressMachineDataHelper.InitializedPressMachineDb();
                Growl.SuccessGlobal(t!("Msg.PreSystem.DeleteDbSuccess"));
            }
            
        }
    }
}