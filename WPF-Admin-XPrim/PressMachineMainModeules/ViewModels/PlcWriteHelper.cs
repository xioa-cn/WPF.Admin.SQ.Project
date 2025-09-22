using HslCommunication.Profinet.Siemens;
using PressMachineMainModeules.Models;

namespace PressMachineMainModeules.ViewModels;

public static partial class PlcViewModel {

    private static void WritePressXParams(string point,int start, PressMachineParamsXDa dto, SiemensS7Net plc)
    {
        var retWrite =
            plc?.Write($"{point}.{start}", dto.待机速度);
        start += 4;
        retWrite =
            plc?.Write($"{point}.{start}", dto.待机位置);
        start += 4;
        retWrite =
            plc?.Write($"{point}.{start}", dto.第一速度);
        start += 4;
        retWrite =
            plc?.Write($"{point}.{start}", dto.第二速度);
        start += 4;
        retWrite =
            plc?.Write($"{point}.{start}", dto.第三速度);
        start += 4;
        retWrite =
            plc?.Write($"{point}.{start}", dto.第四速度);
        start += 4;
        retWrite =
            plc?.Write($"{point}.{start}", dto.第一位置);
        start += 4;
        retWrite =
            plc?.Write($"{point}.{start}", dto.第二位置);
        start += 4;
        retWrite =
            plc?.Write($"{point}.{start}", dto.第三位置);
        start += 4;
        retWrite =
            plc?.Write($"{point}.{start}", dto.第四位置);
    }

    private static void WritePressMachineParams(string point, int start, PressMachineParamsDa dto
        , SiemensS7Net plc
    ) {
        var retWrite =
            plc?.Write($"{point}.{start}", dto.第一位置);
        start += 4;
        retWrite = plc.Write($"{point}.{start}", dto.第二位置);
        start += 4;
        retWrite = plc?.Write($"{point}.{start}", dto.第三位置);
        start += 4;
        retWrite = plc?.Write($"{point}.{start}", dto.第四位置);
        start += 4;
        retWrite = plc?.Write($"{point}.{start}", dto.第一速度);
        start += 4;
        retWrite = plc?.Write($"{point}.{start}", dto.第二速度);
        start += 4;
        retWrite = plc?.Write($"{point}.{start}", dto.第三速度);
        start += 4;
        retWrite = plc?.Write($"{point}.{start}", dto.第四速度);
        start += 4;
        //retWrite = plc?.Write($"{point}.{start}", dto.位置容差);
        //start += 4;
        retWrite = plc?.Write($"{point}.{start}", dto.保护压力);
        start += 4;
        retWrite = plc?.Write($"{point}.{start}", dto.保压时间);
    }

    private static void WritePressMachineWayParams(string point, int start, PressMachineSlipwayDa dto
        , SiemensS7Net plc1) {
        var retWrite = plc1.Write($"{point}.{start}", dto.待机速度);
        start += 4;
        retWrite = plc1.Write($"{point}.{start}", dto.待机位置);
        start += 4;
        retWrite = plc1.Write($"{point}.{start}", dto.第一速度);
        start += 4;
        retWrite = plc1.Write($"{point}.{start}", dto.第一位置);
        start += 4;
        retWrite = plc1.Write($"{point}.{start}", dto.第二速度);
        start += 4;
        retWrite = plc1.Write($"{point}.{start}", dto.第二位置);
        start += 4;
        retWrite = plc1.Write($"{point}.{start}", dto.第三速度);
        start += 4;
        retWrite = plc1.Write($"{point}.{start}", dto.第三位置);
    }

    private static void WritePressMachineSideswayParams(string point, int start, PressMachineSideswayDa dto,
        SiemensS7Net plc1) {
        var retWrite = plc1.Write($"{point}.{start}", dto.待机速度);
        start += 4;
        retWrite = plc1.Write($"{point}.{start}", dto.待机位置);
        start += 4;
        retWrite = plc1.Write($"{point}.{start}", dto.第一速度);
        start += 4;
        retWrite = plc1.Write($"{point}.{start}", dto.第一位置);
        start += 4;
        retWrite = plc1.Write($"{point}.{start}", dto.第二速度);
        start += 4;
        retWrite = plc1.Write($"{point}.{start}", dto.第二位置);
        start += 4;
        retWrite = plc1.Write($"{point}.{start}", dto.第三速度);
        start += 4;
        retWrite = plc1.Write($"{point}.{start}", dto.第三位置);
    }
}