using HandyControl.Controls;
using PressMachineMainModeules.Models;

namespace PressMachineMainModeules.Utils
{
    public static partial class Siemens3Helper
    {
        public static async Task WriteSiemens3(this PressMachineCoreParamsDa dto)
        {
            try
            {
                var ret = await WriteCommon(dto);
                if (!ret)
                {
                    Growl.ErrorGlobal("写入失败Common");
                    return;
                }
                ret = await WriteRoboCylinder(dto);
                if (!ret)
                {
                    Growl.ErrorGlobal("写入失败RoboCylinder");
                    return;
                }
                ret = await WriteSlidingTable(dto);
                if (!ret)
                {
                    Growl.ErrorGlobal("写入失败SlidingTable");
                    return;
                }
                ret = await WriteSidesway(dto);
                if (!ret)
                {
                    Growl.ErrorGlobal("写入失败Sidesway");
                    return;
                }
            }
            catch (Exception ex)
            {
                Growl.ErrorGlobal($"写入失败:{ex.Message}");
            }

        }


        public static async Task ReadSiemens3(this PressMachineCoreParamsDa dto)
        {
            try
            {
                var ret = await dto.ReadCommon();
                if (!ret)
                {
                    Growl.ErrorGlobal("读取失败Common");
                    return;
                }
                ret = await dto.ReadRoboCylinder();
                if (!ret)
                {
                    Growl.ErrorGlobal("读取失败RoboCylinder");
                    return;
                }
                ret = await dto.ReadSlidingTable();
                if (!ret)
                {
                    Growl.ErrorGlobal("读取失败SlidingTable");
                    return;
                }
                ret = await dto.ReadSidesway();
                if (!ret)
                {
                    Growl.ErrorGlobal("读取失败Sidesway");
                    return;
                }
            }
            catch (Exception ex)
            {
                Growl.ErrorGlobal($"写入失败:{ex.Message}");
            }
        }
    }
}
