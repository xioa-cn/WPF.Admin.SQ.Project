using HslCommunication.Core.Device;
using PressMachineMainModeules.Config;
using PressMachineMainModeules.Models;

namespace PressMachineMainModeules.Utils
{
    public static partial class Siemens3Helper
    {
        /// <summary>
        /// 读公共参数
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        private static async Task<bool> ReadCommon(this PressMachineCoreParamsDa dto)
        {
            if (PlcConnect.GoOnS7_02 && PlcConnect.PlcS702 is not null)
            {
                var retPoint = 0;
                dto.左待机位置 = (await PlcConnect.PlcS702.ReadFloatAsync($"DB3.{retPoint}")).Content;
                retPoint += 4;
                dto.左待机速度 = (await PlcConnect.PlcS702.ReadFloatAsync($"DB3.{retPoint}")).Content;
                retPoint += 4;
                dto.右待机位置 = (await PlcConnect.PlcS702.ReadFloatAsync($"DB3.{retPoint}")).Content;
                retPoint += 4;
                dto.右待机速度 = (await PlcConnect.PlcS702.ReadFloatAsync($"DB3.{retPoint}")).Content;
            }
            else
            {
                return false;
            }

            if (PlcConnect.GoOnS7_03 && PlcConnect.PlcS703 is not null)
            {
                var retPoint = 0;
                dto.Press3待机位置 = (await PlcConnect.PlcS703.ReadFloatAsync($"DB3.{retPoint}")).Content;

                retPoint += 4;
                dto.Press3待机速度 = (await PlcConnect.PlcS703.ReadFloatAsync($"DB3.{retPoint}")).Content;
                retPoint += 4;
                dto.Press4待机位置 = (await PlcConnect.PlcS703.ReadFloatAsync($"DB3.{retPoint}")).Content;
                retPoint += 4;
                dto.Press4待机速度 = (await PlcConnect.PlcS703.ReadFloatAsync($"DB3.{retPoint}")).Content;
            }
            else
            {
                return false;
            }



            return true;
        }

        /// <summary>
        /// 读电缸参数
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        private static async Task<bool> ReadRoboCylinder(this PressMachineCoreParamsDa dto)
        {
            if (PlcConnect.GoOnS7_02 && PlcConnect.PlcS702 is not null)
            {
                var ret = await dto.PlcParams01.ReadRoboCylinder(PlcConnect.PlcS702, 100, "DB3");
                if (!ret)
                {
                    return false;
                }
                ret = await dto.PlcParams02.ReadRoboCylinder(PlcConnect.PlcS702, 200, "DB3");
                if (!ret)
                {
                    return false;
                }
                ret = await dto.PlcParams03.ReadRoboCylinder(PlcConnect.PlcS702, 300, "DB3");
                if (!ret)
                {
                    return false;
                }
                ret = await dto.PlcParams04.ReadRoboCylinder(PlcConnect.PlcS702, 400, "DB3");
                if (!ret)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            if (PlcConnect.GoOnS7_03 && PlcConnect.PlcS703 is not null)
            {
                var ret = await dto.Press3Params01.ReadRoboCylinder(PlcConnect.PlcS703, 100, "DB3");
                if (!ret)
                {
                    return false;
                }
                ret = await dto.Press3Params02.ReadRoboCylinder(PlcConnect.PlcS703, 200, "DB3");
                if (!ret)
                {
                    return false;
                }
                ret = await dto.Press3Params03.ReadRoboCylinder(PlcConnect.PlcS703, 300, "DB3");
                if (!ret)
                {
                    return false;
                }
                ret = await dto.Press3Params04.ReadRoboCylinder(PlcConnect.PlcS703, 400, "DB3");
                if (!ret)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        private static async Task<bool> ReadRoboCylinder(this PressMachineParamsDa dto,
            DeviceCommunication plc,
            int point,
            string db)
        {
            dto.预压位置 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;

            point += 4;
            dto.预压速度 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            point += 4;
            dto.第一位置 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            point += 4;
            dto.第一速度 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            point += 4;
            dto.第二位置 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            point += 4;
            dto.第二速度 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            point += 4;
            dto.第三位置 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            point += 4;
            dto.第三速度 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            point += 4;
            dto.第四位置 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            point += 4;
            dto.第四速度 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            point += 4;
            dto.位置容差 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            point += 4;
            dto.保护压力 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            point += 4;
            dto.保压时间 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            return true;
        }

        /// <summary>
        /// 读滑台参数
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        private static async Task<bool> ReadSlidingTable(this PressMachineCoreParamsDa dto)
        {
            if (PlcConnect.GoOnS7_02 && PlcConnect.PlcS702 is not null)
            {
                var ret = await dto.SlipwayDa.ReadSlidingTable(PlcConnect.PlcS702, 0, "DB3");
                if (!ret)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        private static async Task<bool> ReadSlidingTable(this PressMachineSlipwayDa dto,
            DeviceCommunication plc,
           int point,
           string db)
        {
            dto.待机速度 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            point += 4;
            dto.待机位置 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            point += 4;
            dto.第一速度 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            point += 4;
            dto.第一位置 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            point += 4;
            dto.第二速度 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            point += 4;
            dto.第二位置 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            point += 4;
            dto.第三速度 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            point += 4;
            dto.第三位置 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            point += 4;
            dto.第四速度 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            point += 4;
            dto.第四位置 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            return true;
        }


        /// <summary>
        /// 读横移参数
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        private static async Task<bool> ReadSidesway(this PressMachineCoreParamsDa dto)
        {
            if (PlcConnect.GoOnS7_01 && PlcConnect.PlcS701 is not null)
            {
                var ret = await dto.SideswayDaLeft.ReadSidesway( PlcConnect.PlcS701, 100, "DB3");
                if (!ret)
                {
                    return false;
                }
                ret = await dto.SideswayDaRight.ReadSidesway( PlcConnect.PlcS701, 200, "DB3");
                if (!ret)
                {
                    return false;
                }
                ret = await dto.SideswayDaCenter.ReadSidesway( PlcConnect.PlcS701, 300, "DB3");
                if (!ret)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        private static async Task<bool> ReadSidesway(this PressMachineSideswayDa dto,
          DeviceCommunication plc,
          int point,
          string db)
        {
            dto.待机速度 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            point += 4;
            dto.待机位置 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            point += 4;
            dto.第一速度 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            point += 4;
            dto.第一位置 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            point += 4;
            dto.第二速度 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            point += 4;
            dto.第二位置 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            point += 4;
            dto.第三速度 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            point += 4;
            dto.第三位置 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            point += 4;
            dto.第四速度 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            point += 4;
            dto.第四位置 = (await plc.ReadFloatAsync($"{db}.{point}")).Content;
            return true;
            return true;
        }

    }
}
