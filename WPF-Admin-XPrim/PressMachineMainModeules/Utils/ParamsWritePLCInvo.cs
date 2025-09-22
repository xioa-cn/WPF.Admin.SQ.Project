using PressMachineMainModeules.Config;
using PressMachineMainModeules.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressMachineMainModeules.Utils
{
    internal class ParamsWritePLCInvo
    {
        public static void Write(PressMachineParamsDa point, PressMachineCoreParamsDa content)
        {
            //if (!PlcConnect.GoOn) return;
            //var ret = PlcConnect.Plc?.Write(point.待机位置, content.PosAwaitPos);
            //if (!ret!.IsSuccess)
            //{
            //    return;
            //}
            //ret = PlcConnect.Plc?.Write(point.待机速度, content.PosAwaitSpeed);

            //ret = PlcConnect.Plc?.Write(point.第一位置, content.PosFirstPos);
            //ret = PlcConnect.Plc?.Write(point.第一速度, content.PosFirstSpeed);

            //ret = PlcConnect.Plc?.Write(point.第二位置, content.PosSecondPos);
            //ret = PlcConnect.Plc?.Write(point.第二速度, content.PosSecondSpeed);

            //ret = PlcConnect.Plc?.Write(point.第三位置, content.PosThirdPos);
            //ret = PlcConnect.Plc?.Write(point.第三速度, content.PosThirdSpeed);

            //ret = PlcConnect.Plc?.Write(point.保护压力, content.PosProtectPress);
            //ret = PlcConnect.Plc?.Write(point.保压时间, content.PosProtectTime);
        }
    }
}
