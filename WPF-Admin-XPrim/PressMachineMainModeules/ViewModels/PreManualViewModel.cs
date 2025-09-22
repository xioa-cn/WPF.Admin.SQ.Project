using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using PressMachineMainModeules.Views.Elf;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using CommunityToolkit.Mvvm.Messaging;
using PressMachineMainModeules.Models;
using PressMachineMainModeules.Show;
using WPF.Admin.Models;
using WPF.Admin.Service.Services;
using PressMachineMainModeules.Components;
using PressMachineMainModeules.Config;
using PressMachineMainModeules.Views;
using WPF.Admin.Models.Models;
using Application = System.Windows.Application;

namespace PressMachineMainModeules.ViewModels
{
    public partial class PreManualViewModel : BindableBase
    {
        [ObservableProperty] private string _title = nameof(PreManualViewModel);


        #region IO

        /// <summary>
        /// IO点监控值
        /// </summary>
        [ObservableProperty] private IOPointVal ioPoint = new IOPointVal();
        private void ChangeIoStatus(object recipient, IOPointVal message)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => { this.IoPoint.SetSelf(message); });
        }

        #endregion

        [ObservableProperty] private MaualShowFloatData _MaualShowFloatData = new MaualShowFloatData();

        public PreManualViewModel()
        {
            #region IO监听初始化
            WeakReferenceMessenger.Default.Register<IOPointVal>(this, ChangeIoStatus);
            #endregion

            WeakReferenceMessenger.Default.Register<PressMachineStatusManager>(this, PressMachineStatusMethods);
            WeakReferenceMessenger.Default.Register<MaualShowFloatData>(this, PressMachineManualShowFloatDataMethods);
            ReadParamsView();
        }

        private void PressMachineManualShowFloatDataMethods(object recipient, MaualShowFloatData message)
        {
            MaualShowFloatData.Setself(message);
        }

        private void PressMachineStatusMethods(object recipient, PressMachineStatusManager message)
        {
            switch (message.Num)
            {
                case 1:
                    this.PressMachine01Status.Setself(message.Status);
                    break;
                case 2:
                    this.PressMachine02Status.Setself(message.Status);
                    break;
                case 3:
                    this.PressMachine03Status.Setself(message.Status);
                    break;
            }
        }


        #region Manual页面参数集

        public InvoMaualModel ManualParameter01 { get; set; } = new InvoMaualModel();
        public InvoMaualModel ManualParameter02 { get; set; } = new InvoMaualModel();
        public InvoMaualModel ManualParameter03 { get; set; } = new InvoMaualModel();
        public ManualWriteWindow WriteWindow { get; set; } = new ManualWriteWindow();

        [RelayCommand]
        private void WriteParams1(string writeType)
        {
            WriteParams(writeType, ManualParameter01);
        }

        [RelayCommand]
        private void WriteParams2(string writeType)
        {
            WriteParams(writeType, ManualParameter02, 1);
        }

        [RelayCommand]
        private void WriteParams3(string writeType)
        {
            WriteParams(writeType, ManualParameter03, 2);
        }

        [RelayCommand]
        private void ReadParamsView()
        {
            ManualParameter01.Setself( ReadParams(0));
            ManualParameter02.Setself(ReadParams(1));
            ManualParameter03.Setself(ReadParams(2));
        }

        private void WriteParams(string writeType, InvoMaualModel manualParameter, int writeNum = 0)
        {
            string point = string.Empty;
            switch (writeType)
            {
                case "手动速度":
                    point = "MB" + $"{2000 + writeNum * 200}";
                    Write(point, manualParameter.手动速度, writeType);
                    break;
                case "手动保护压力":
                    point = "MB" + $"{2004 + writeNum * 200}";
                    Write(point, manualParameter.手动保护压力, writeType);
                    break;
                case "设定频率":
                    point = "MB" + $"{2008 + writeNum * 200}";
                    Write(point, manualParameter.设定频率, writeType);
                    break;
                case "设定速度":
                    point = "MB" + $"{2012 + writeNum * 200}";
                    Write(point, manualParameter.设定速度, writeType);
                    break;
                case "每千克对应扭矩":
                    point = "MB" + $"{2016 + writeNum * 200}";
                    Write(point, manualParameter.每千克对应扭矩, writeType);
                    break;
                case "压力浮动范围":
                    point = "MB" + $"{2020 + writeNum * 200}";
                    Write(point, manualParameter.压力浮动范围, writeType);
                    break;
                case "设定正向压力":
                    point = "MB" + $"{2024 + writeNum * 200}";
                    Write(point, manualParameter.设定正向压力, writeType);
                    break;
                case "正向压力补偿":
                    point = "MB" + $"{2028 + writeNum * 200}";
                    Write(point, manualParameter.正向压力补偿, writeType);
                    break;
                case "设定负向压力":
                    point = "MB" + $"{2032 + writeNum * 200}";
                    Write(point, manualParameter.设定负向压力, writeType);
                    break;
                case "负向压力补偿":
                    point = "MB" + $"{2036 + writeNum * 200}";
                    Write(point, manualParameter.负向压力补偿, writeType);
                    break;
            }
        }
        private InvoMaualModel ReadParams(int writeNum = 0)
        {
            InvoMaualModel result = new InvoMaualModel();

            if (PlcConnect.GoOn)
            {
                try
                {
                    result.手动速度 = PlcConnect.Plc?.ReadFloat("MB" + $"{2000 + writeNum * 200}").Content ?? 0;
                    result.手动保护压力 = PlcConnect.Plc?.ReadFloat("MB" + $"{2004 + writeNum * 200}").Content ?? 0;
                    result.设定频率 = PlcConnect.Plc?.ReadFloat("MB" + $"{2008 + writeNum * 200}").Content ?? 0;
                    result.设定速度 = PlcConnect.Plc?.ReadFloat("MB" + $"{2012 + writeNum * 200}").Content ?? 0;
                    result.每千克对应扭矩 = PlcConnect.Plc?.ReadFloat("MB" + $"{2016 + writeNum * 200}").Content ?? 0;
                    result.压力浮动范围 = PlcConnect.Plc?.ReadFloat("MB" + $"{2020 + writeNum * 200}").Content ?? 0;
                    result.设定正向压力 = PlcConnect.Plc?.ReadFloat("MB" + $"{2024 + writeNum * 200}").Content ?? 0;
                    result.正向压力补偿 = PlcConnect.Plc?.ReadFloat("MB" + $"{2028 + writeNum * 200}").Content ?? 0;
                    result.设定负向压力 = PlcConnect.Plc?.ReadFloat("MB" + $"{2032 + writeNum * 200}").Content ?? 0;
                    result.负向压力补偿 = PlcConnect.Plc?.ReadFloat("MB" + $"{2036 + writeNum * 200}").Content ?? 0;
                }
                catch (Exception ex)
                {
                    Growl.ErrorGlobal($"读取异常:{ex.Message}");
                }
            }
            else
            {
                Growl.WarningGlobal($"Plc Connect Warning~ 执行的操作为 {writeNum}");
            }


            return result;
        }


        private void Write(string point, float value, string writetype)
        {
            if (PlcConnect.GoOn)
            {
                try
                {
                    var result = PlcConnect.Plc?.Write(point, value);
                    if (!result.IsSuccess)
                    {
                        throw new Exception($"写入失败~{result.Message}");
                    }

                    Growl.SuccessGlobal($"值写入成功:{value}");

                }
                catch (Exception ex)
                {
                    Growl.ErrorGlobal($"写入异常:{ex.Message}");
                }

            }
            else
            {
                Growl.WarningGlobal($"Plc Connect Warning~ 执行的操作为 {writetype}:{point}-{value}");
            }
        }

        [RelayCommand]
        private void OpenManualParamsWriteWindow()
        {
            WriteWindow.DataContext = this;
            WriteWindow.Show();
        }

        #endregion


        #region 读取



        #endregion


        #region 状态窗
        [ObservableProperty] private bool _statusWindowShow = false;

        private PressMachineManualStatusView manualStatusView = new PressMachineManualStatusView();


        [RelayCommand]
        private void ShowStatusView()
        {
            if (!manualStatusView.IsShow)
            {
                manualStatusView = new PressMachineManualStatusView();
            }

            manualStatusView.DataContext = this;
            manualStatusView.Show();
            manualStatusView.IsShow = true;
            StatusWindowShow = true;
        }

        #endregion


        #region 软件精灵

        /// <summary>
        /// 精灵界面内容
        /// </summary>
        public ElfContent[] ElfContents => ElfShow.ElfContents;

        private WindowScreenElf? windowScreenElf;
        private IntPtr windowHandle; // 存储窗口句柄

        [RelayCommand]
        private void OpenElf()
        {
            // 检查窗口是否已经创建并且仍然存在
            if (windowScreenElf != null)
            {
                if (!IsWindowValid())
                {
                    windowScreenElf = null; // 窗口无效，清除引用
                    windowHandle = IntPtr.Zero;
                }
                else if (windowScreenElf.IsShow)
                {
                    Growl.InfoGlobal($"精灵已经打开了哦~！快去使用吧！！！");
                    return;
                }
            }

            windowScreenElf = new WindowScreenElf()
            {
                DataContext = this,
            };

            // 存储窗口句柄
            windowScreenElf.SourceInitialized += (s, e) =>
            {
                if (s is System.Windows.Window window)
                {
                    windowHandle = new WindowInteropHelper(window).Handle;
                }
            };

            windowScreenElf.Closed += WindowScreenElf_Closed;
            windowScreenElf.Closing += WindowScreenElf_Closing;
            Application.Current.Exit += Application_Exit;

            windowScreenElf.Show();
        }

        private bool IsWindowValid()
        {
            if (windowScreenElf == null || windowHandle == IntPtr.Zero)
                return false;

            try
            {
                // 检查窗口是否仍然存在
                if (!IsWindow(windowHandle))
                    return false;

                // 检查窗口是否可见
                if (!windowScreenElf.IsVisible)
                    return false;

                // 尝试访问窗口属性
                var _ = windowScreenElf.ActualWidth;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void WindowScreenElf_Closed(object? sender, EventArgs e)
        {
            windowScreenElf = null;
            windowHandle = IntPtr.Zero;
        }

        private void WindowScreenElf_Closing(object? sender, CancelEventArgs e)
        {
            if (sender is WindowScreenElf window)
            {
                window.Closed -= WindowScreenElf_Closed;
                window.Closing -= WindowScreenElf_Closing;
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (windowScreenElf != null)
            {
                windowScreenElf.Close();
                windowScreenElf = null;
                windowHandle = IntPtr.Zero;
            }
        }

        // Win32 API
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindow(IntPtr hWnd);

        #endregion


        #region 测试报警

        [RelayCommand]
        private void AddAlarm()
        {
            AlarmToUIModels.CreateAddAlarm(["我是报警信息"]).Send();
        }


        [RelayCommand]
        private void RemoveAlarm()
        {
            AlarmToUIModels.CreateRemoveAlarm(["我是报警信息"]).Send();
        }

        #endregion
    }
}