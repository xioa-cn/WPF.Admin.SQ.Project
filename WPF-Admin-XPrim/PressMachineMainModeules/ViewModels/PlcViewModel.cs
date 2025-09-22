using PressMachineMainModeules.Config;
using PressMachineMainModeules.Models;
using WPF.Admin.Service.Services;
using WPF.Admin.Themes.Helper;

namespace PressMachineMainModeules.ViewModels;

public  static partial class PlcViewModel {
    public static void Read(this PressMachineCoreParamsDa dto) {
        dto.PlcParams01.保压时间 = 10;
    }

    public static void Write(this PressMachineCoreParamsDa dto) {
        Task task = Task.Run(() =>
        {
            var plc = PlcConnect.CreatePlcConnect("192.168.0.10");
            if (plc is null)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => { SnackbarHelper.Show("PLC连接失败！参数初始化写入异常", 2000); });
                return;
            }

            var retWrite = plc.Write("DB3.0", dto.左待机位置);
            if (retWrite is null || !retWrite.IsSuccess)
            {
                SnackbarHelper.Show("参数写失败请检查配置！", 2000);
                return;
            }

            retWrite = plc.Write("DB3.4", dto.左待机速度);
            //retWrite = plc.Write("DB2.200", dto.右待机位置);
            //retWrite = plc.Write("DB2.204", dto.右待机速度);

            // retWrite = PlcConnect.Plc?.Write("DB2.8", dto.PlcParams01.第一位置);
            // retWrite = PlcConnect.Plc?.Write("DB2.12", dto.PlcParams01.第二位置);
            // retWrite = PlcConnect.Plc?.Write("DB2.16", dto.PlcParams01.第三位置);
            // retWrite = PlcConnect.Plc?.Write("DB2.20", dto.PlcParams01.第四位置);
            // retWrite = PlcConnect.Plc?.Write("DB2.24", dto.PlcParams01.第一速度);
            // retWrite = PlcConnect.Plc?.Write("DB2.28", dto.PlcParams01.第二速度);
            // retWrite = PlcConnect.Plc?.Write("DB2.32", dto.PlcParams01.第三速度);
            // retWrite = PlcConnect.Plc?.Write("DB2.36", dto.PlcParams01.第四速度);
            // retWrite = PlcConnect.Plc?.Write("DB2.40", dto.PlcParams01.位置容差);
            // retWrite = PlcConnect.Plc?.Write("DB2.44", dto.PlcParams01.保护压力);
            // retWrite = PlcConnect.Plc?.Write("DB2.48", dto.PlcParams01.保压时间);
            WritePressMachineParams("DB3", 20, dto.PlcParams01, plc);
            WritePressMachineParams("DB3", 100, dto.PlcParams03, plc);
            WritePressMachineParams("DB3", 180, dto.PlcParams02, plc);
            WritePressMachineParams("DB3", 260, dto.PlcParams04, plc);
            WritePressXParams("DB3", 340, dto.PlcParamsX, plc);
            plc.ConnectClose();
            plc.Dispose();
        });

        //Task task2 = Task.Run(() =>
        //{
        //    var plc1 = PlcConnect.CreatePlcConnect("192.168.0.10");
        //    if (plc1 is null)
        //    {
        //        DispatcherHelper.CheckBeginInvokeOnUI(() => { SnackbarHelper.Show("PLC连接失败！参数初始化写入异常", 2000); });

        //        return;
        //    }

        //    // retWrite = plc1.Write("DB2.100", dto.SlipwayDa.待机速度);
        //    // retWrite = plc1.Write("DB2.104", dto.SlipwayDa.待机位置);
        //    // retWrite = plc1.Write("DB2.108", dto.SlipwayDa.第一速度);
        //    // retWrite = plc1.Write("DB2.112", dto.SlipwayDa.第一位置);
        //    // retWrite = plc1.Write("DB2.116", dto.SlipwayDa.第二速度);
        //    // retWrite = plc1.Write("DB2.120", dto.SlipwayDa.第二位置);
        //    // retWrite = plc1.Write("DB2.124", dto.SlipwayDa.第三速度);
        //    // retWrite = plc1.Write("DB2.128", dto.SlipwayDa.第三位置);
        //    WritePressMachineWayParams("DB2", 100, dto.SlipwayDa, plc1);

        //    WritePressMachineSideswayParams("DB2", 132, dto.SideswayDaLeft, plc1);

        //    WritePressMachineSideswayParams("DB2", 164, dto.SideswayDaRight, plc1);
        //});
        
        Task.WaitAll(task);
        
        SnackbarHelper.Show("参数写入成功", 2000);
    }

  
}