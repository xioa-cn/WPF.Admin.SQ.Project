using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HslCommunication;
using HslCommunication.Profinet.Turck;
using PressMachineMainModeules.Config;
using PressMachineMainModeules.Models;
using System.Collections.ObjectModel;
using System.Globalization;
using PressMachineMainModeules.Utils;
using WPF.Admin.Models;
using WPF.Admin.Service.Logger;
using WPF.Admin.Service.Services;

namespace PressMachineMainModeules.ViewModels
{
    public partial class PreManual2ViewModel : BindableBase
    {
        [ObservableProperty] private ObservableCollection<IOPointPositionModel>? _IOPointPositionModels;
        [ObservableProperty] private ObservableCollection<ManualPointPositionModel>? _ManualButtonModels;
        [ObservableProperty] private ObservableCollection<ManualParametersModel>? _ManualParametersModels;
        [ObservableProperty] private ObservableCollection<ManualValueModel>? _ManualValueModels;
        [ObservableProperty] private ManualWeakContextModel _Content = new ManualWeakContextModel();
        public bool ViewIsLoaded { get; set; } = false;

        public PreManual2ViewModel()
        {
            if (Common.OpenIO)
            {
                Task.Run(() =>
                {
                    // 初始化手动界面IO
                    IOManager.ManualIOInstance.Initialized(ConfigPlcs.ConfigPath, "ManualIO");
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        IOPointPositionModels = IOManager.ManualIOInstance.Instance;
                    });

                });
            }

            Task.Run(() =>
            {
                // 初始化手动界面按钮
                ManualButtonManager.Initialized(ConfigPlcs.ConfigPath, "ManualBtn");
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    ManualButtonModels = ManualButtonManager.Instance;
                });
            });

            Task.Run(() =>
            {
                ManualParametersManager.Instance.Initialized(ConfigPlcs.ConfigPath, "ManualParameters");
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    ManualParametersModels = ManualParametersManager.Instance.ManualParameters;
                });
            });
            Task.Run(() =>
            {
                ManualValueManager.Instance.Initialized(ConfigPlcs.ConfigPath, "ManualValue");
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    ManualValueModels = ManualValueManager.Instance.ManualValue;
                });
            });

            Task.Factory.StartNew(ManualValueRefresh, TaskCreationOptions.LongRunning);
        }

        private void ManualValueRefresh()
        {
            while (true)
            {
                try
                {
                    if (ManualValueModels is not null && ViewIsLoaded)
                    {
                        foreach (var item in ManualValueModels)
                        {
                           var value = item.Read(ConfigPlcs.Instance[item.PlcName], item.getFullPosition);
                            // var value = ConfigPlcs.Instance[item.PlcName]?.ReadFloat(item.getFullPosition);

                            if (value.IsSuccess)
                            {
                            }
                            else
                            {
                                var message = $"ReadFloat Error: {value?.Message} \nPosition: {item.getFullPosition} \nPlcName: {item.PlcName}";
                                throw new Exception(message);
                            }
                        }
                    }
                    if (IOPointPositionModels is not null && ViewIsLoaded)
                    {
                        foreach (var item in IOPointPositionModels)
                        {
                            var value = ConfigPlcs.Instance[item.PlcName]?.ReadBool(item.Point);

                            if (value is not null && value.IsSuccess)
                            {
                                item.State = value.Content;
                            }
                            else
                            {
                                var message = $"ReadFloat Error: {value?.Message} \nPosition: {item.Point} \nPlcName: {item.PlcName}";
                                throw new Exception(message);
                            }
                        }
                    }
                    
                    Thread.Sleep(100);
            
                }
                catch (Exception ex)
                {
                    XLogGlobal.Logger?.LogError(ex.Message, ex);
                    Growl.WarningGlobal($"ManualValueRefresh Error: {ex.Message}");
                    Thread.Sleep(5000);
                }
                Thread.Sleep(500);
            }
        }

        [RelayCommand]
        private void StartWrite(ManualPointPositionModel dto)
        {
            if (dto == null)
            {
                return;
            }
            Write(dto);
        }


        [RelayCommand]
        private void EnterWrite(ManualPointPositionModel dto)
        {
            if (dto == null)
            {
                return;
            }
            if (dto.WriteState != "2")
            {
                return;
            }
            Write(dto, true);
        }

        private void Write(ManualPointPositionModel dto, bool isEnter = false)
        {
            var device = ConfigPlcs.Instance[dto.PlcName];
            if (device is null)
            {
                Growl.ErrorGlobal($"device is null {nameof(PreManual2ViewModel)}:84");
                return;
            }
            string? position = dto.getFullPosition;

            OperateResult? result = null;
            var value = false;
            if (isEnter)
            {
                value = false;
            }
            else if (dto.WriteState == "1")
            {
                value = dto.WValue.ToLower() == "true";
            }
            else if (dto.WriteState == "3")
            {
                var tempvalue = device.ReadBool(position);
                if (tempvalue.IsSuccess)
                {
                    value = !(tempvalue.Content);
                }
                else
                {
                    var message = $"tempvalue is error \n {result?.Message} {nameof(PreManual2ViewModel)}:118";
                    Growl.ErrorGlobal(message);
                    XLogGlobal.Logger?.LogError(message);
                    return;
                }
            }
            else
            {
                value = true;
            }

            result = ConfigPlcs.Instance[dto.PlcName].Write(position, value);
            if (result is null || !result.IsSuccess)
            {
                var message = $"result is error \n {result?.Message} {nameof(PreManual2ViewModel)}:131";
                Growl.ErrorGlobal(message);
                XLogGlobal.Logger?.LogError(message);
                return;
            }
            //Growl.SuccessGlobal($"Write Success \nDb : {position} \nValue : {value}");
        }

        [RelayCommand]
        public void WriteParameter(ManualParametersModel dto)
        {
            var device = ConfigPlcs.Instance[dto.PlcName];
            if (device is null)
            {
                Growl.ErrorGlobal($"device is null {nameof(PreManual2ViewModel)}:84");
                return;
            }
            string position = dto.getFullPosition;
            OperateResult result =  dto.Write(device, position);
            //OperateResult result = null;
            // if (dto.Type.ToLower() is "float")
            // {
            //     result = device.Write(position, float.Parse(dto.Value.ToString(CultureInfo.InvariantCulture) ?? "0"));
            // }
            // else if (dto.Type.ToLower() is "double")
            // {
            //     result = device.Write(position, double.Parse(dto.Value.ToString(CultureInfo.InvariantCulture) ?? "0"));
            // }
            // else if (dto.Type.ToLower() is "int")
            // {
            //     result = device.Write(position, int.Parse(dto.Value.ToString(CultureInfo.InvariantCulture) ?? "0"));
            // }
            // else if (dto.Type.ToLower() is "ushort")
            // {
            //     result = device.Write(position, ushort.Parse(dto.Value.ToString(CultureInfo.InvariantCulture) ?? "0"));
            // }
            // else if (dto.Type.ToLower() is "short")
            // {
            //     result = device.Write(position, short.Parse(dto.Value.ToString(CultureInfo.InvariantCulture) ?? "0"));
            // }
            // else if (dto.Type.ToLower() is "int32")
            // {
            //     result = device.Write(position, (Int32)int.Parse(dto.Value.ToString(CultureInfo.InvariantCulture) ?? "0"));
            // }
            // else
            // {
            //     var message = $"Unsupported type: {dto.Type} for position {position} in {nameof(PreManual2ViewModel)}:194";
            //     Growl.ErrorGlobal(message);
            //     XLogGlobal.Logger?.LogError(message);
            //     return;
            // }

            if (!result.IsSuccess)
            {
                var message = $"result is error \n {result?.Message} {nameof(PreManual2ViewModel)}:195";
                Growl.ErrorGlobal(message);
                XLogGlobal.Logger?.LogError(message);
                return;
            }
            else
            {
                Growl.SuccessGlobal($"写入成功 Value : {dto.Value}");
            }
        }
    }
}
