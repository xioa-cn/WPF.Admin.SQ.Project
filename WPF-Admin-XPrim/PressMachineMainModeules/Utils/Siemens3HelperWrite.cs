using HslCommunication.Core.Device;
using PressMachineMainModeules.Config;
using PressMachineMainModeules.Models;

namespace PressMachineMainModeules.Utils
{
    public static partial class Siemens3Helper
    {
        /// <summary>
        /// 写共有参数
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static async Task<bool> WriteCommon(PressMachineCoreParamsDa dto)
        {
            if (PlcConnect.GoOnS7_02 && PlcConnect.PlcS702 is not null)
            {
                var retPoint = 0;
                var ret = await PlcConnect.PlcS702.WriteAsync($"DB3.{retPoint}", dto.左待机位置);
                if (!ret.IsSuccess)
                {
                    throw new Exception($"左待机位置写入失败:{ret.Message}");
                }
                retPoint += 4;
                ret = await PlcConnect.PlcS702.WriteAsync($"DB3.{retPoint}", dto.左待机速度);
                retPoint += 4;
                ret = await PlcConnect.PlcS702.WriteAsync($"DB3.{retPoint}", dto.右待机位置);
                retPoint += 4;
                ret = await PlcConnect.PlcS702.WriteAsync($"DB3.{retPoint}", dto.右待机速度);
            }
            else
            {
                return false;
            }

            if (PlcConnect.GoOnS7_03 && PlcConnect.PlcS703 is not null)
            {
                var retPoint = 0;
                var ret = await PlcConnect.PlcS703.WriteAsync($"DB3.{retPoint}", dto.Press3待机位置);
                if (!ret.IsSuccess)
                {
                    throw new Exception($"左待机位置写入失败:{ret.Message}");
                }
                retPoint += 4;
                ret = await PlcConnect.PlcS703.WriteAsync($"DB3.{retPoint}", dto.Press3待机速度);
                retPoint += 4;
                ret = await PlcConnect.PlcS703.WriteAsync($"DB3.{retPoint}", dto.Press4待机位置);
                retPoint += 4;
                ret = await PlcConnect.PlcS703.WriteAsync($"DB3.{retPoint}", dto.Press4待机速度);
            }
            else
            {
                return false;
            }



            return true;
        }


        /// <summary>
        /// 写电缸参数
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        private static async Task<bool> WriteRoboCylinder(PressMachineCoreParamsDa dto)
        {
            if (PlcConnect.GoOnS7_02 && PlcConnect.PlcS702 is not null)
            {
                var ret = await WriteRoboCylinder(dto.PlcParams01, PlcConnect.PlcS702, 100, "DB3");
                if (!ret)
                {
                    return false;
                }
                ret = await WriteRoboCylinder(dto.PlcParams02, PlcConnect.PlcS702, 200, "DB3");
                if (!ret)
                {
                    return false;
                }
                ret = await WriteRoboCylinder(dto.PlcParams03, PlcConnect.PlcS702, 300, "DB3");
                if (!ret)
                {
                    return false;
                }
                ret = await WriteRoboCylinder(dto.PlcParams04, PlcConnect.PlcS702, 400, "DB3");
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
                var ret = await WriteRoboCylinder(dto.Press3Params01, PlcConnect.PlcS703, 100, "DB3");
                if (!ret)
                {
                    return false;
                }
                ret = await WriteRoboCylinder(dto.Press3Params02, PlcConnect.PlcS703, 200, "DB3");
                if (!ret)
                {
                    return false;
                }
                ret = await WriteRoboCylinder(dto.Press3Params03, PlcConnect.PlcS703, 300, "DB3");
                if (!ret)
                {
                    return false;
                }
                ret = await WriteRoboCylinder(dto.Press3Params04, PlcConnect.PlcS703, 400, "DB3");
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

        private static async Task<bool> WriteRoboCylinder(PressMachineParamsDa dto,
            DeviceCommunication plc,
            int point,
            string db)
        {
            var ret = await plc.WriteAsync($"{db}.{point}", dto.预压位置);
            if (ret != null && !ret.IsSuccess)
            {
                throw new Exception($"参数写入失败:{ret.Message}");
            }
            point += 4;
            ret = await plc.WriteAsync($"{db}.{point}", dto.预压速度);
            point += 4;
            ret = await plc.WriteAsync($"{db}.{point}", dto.第一位置);
            point += 4;
            ret = await plc.WriteAsync($"{db}.{point}", dto.第一速度);
            point += 4;
            ret = await plc.WriteAsync($"{db}.{point}", dto.第二位置);
            point += 4;
            ret = await plc.WriteAsync($"{db}.{point}", dto.第二速度);
            point += 4;
            ret = await plc.WriteAsync($"{db}.{point}", dto.第三位置);
            point += 4;
            ret = await plc.WriteAsync($"{db}.{point}", dto.第三速度);
            point += 4;
            ret = await plc.WriteAsync($"{db}.{point}", dto.第四位置);
            point += 4;
            ret = await plc.WriteAsync($"{db}.{point}", dto.第四速度);
            point += 4;
            ret = await plc.WriteAsync($"{db}.{point}", dto.位置容差);
            point += 4;
            ret = await plc.WriteAsync($"{db}.{point}", dto.保护压力);
            point += 4;
            ret = await plc.WriteAsync($"{db}.{point}", dto.保压时间);
            return true;
        }

        /// <summary>
        /// 写滑台参数
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        private static async Task<bool> WriteSlidingTable(PressMachineCoreParamsDa dto)
        {
            if (PlcConnect.GoOnS7_01 && PlcConnect.PlcS701 is not null)
            {
                var ret = await WriteSlidingTable(dto.SlipwayDa, PlcConnect.PlcS701, 0, "DB3");
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

        private static async Task<bool> WriteSlidingTable(PressMachineSlipwayDa dto,
           DeviceCommunication plc,
           int point,
           string db)
        {
            var ret = await plc.WriteAsync($"{db}.{point}", dto.待机速度);
            if (ret != null && !ret.IsSuccess)
            {
                throw new Exception($"参数写入失败:{ret.Message}");
            }
            point += 4;
            ret = await plc.WriteAsync($"{db}.{point}", dto.待机位置);
            point += 4;
            ret = await plc.WriteAsync($"{db}.{point}", dto.第一速度);
            point += 4;
            ret = await plc.WriteAsync($"{db}.{point}", dto.第一位置);
            point += 4;
            ret = await plc.WriteAsync($"{db}.{point}", dto.第二速度);
            point += 4;
            ret = await plc.WriteAsync($"{db}.{point}", dto.第二位置);
            point += 4;
            ret = await plc.WriteAsync($"{db}.{point}", dto.第三速度);
            point += 4;
            ret = await plc.WriteAsync($"{db}.{point}", dto.第三位置);
            point += 4;
            ret = await plc.WriteAsync($"{db}.{point}", dto.第四速度);
            point += 4;
            ret = await plc.WriteAsync($"{db}.{point}", dto.第四位置);

            return true;
        }


        /// <summary>
        /// 写横移参数
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        private static async Task<bool> WriteSidesway(PressMachineCoreParamsDa dto)
        {
            if (PlcConnect.GoOnS7_01 && PlcConnect.PlcS701 is not null)
            {
                var ret = await WriteSidesway(dto.SideswayDaLeft, PlcConnect.PlcS701, 100, "DB3");
                if (!ret)
                {
                    return false;
                }
                ret = await WriteSidesway(dto.SideswayDaRight, PlcConnect.PlcS701, 200, "DB3");
                if (!ret)
                {
                    return false;
                }
                ret = await WriteSidesway(dto.SideswayDaCenter, PlcConnect.PlcS701, 300, "DB3");
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

        private static async Task<bool> WriteSidesway(PressMachineSideswayDa dto,
            DeviceCommunication plc,
            int point,
            string db)
        {
            var ret = await plc.WriteAsync($"{db}.{point}", dto.待机速度);
            if (ret != null && !ret.IsSuccess)
            {
                throw new Exception($"参数写入失败:{ret.Message}");
            }
            point += 4;
            ret = await plc.WriteAsync($"{db}.{point}", dto.待机位置);
            point += 4;
            ret = await plc.WriteAsync($"{db}.{point}", dto.第一速度);
            point += 4;
            ret = await plc.WriteAsync($"{db}.{point}", dto.第一位置);
            point += 4;
            ret = await plc.WriteAsync($"{db}.{point}", dto.第二速度);
            point += 4;
            ret = await plc.WriteAsync($"{db}.{point}", dto.第二位置);
            point += 4;
            ret = await plc.WriteAsync($"{db}.{point}", dto.第三速度);
            point += 4;
            ret = await plc.WriteAsync($"{db}.{point}", dto.第三位置);
            point += 4;
            ret = await plc.WriteAsync($"{db}.{point}", dto.第四速度);
            point += 4;
            ret = await plc.WriteAsync($"{db}.{point}", dto.第四位置);

            return true;           
        }
    }
}
