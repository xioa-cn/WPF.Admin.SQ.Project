using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using PressMachineMainModeules.Config;
using PressMachineMainModeules.Models;
using System.Diagnostics;
using WPF.Admin.Models;

namespace PressMachineMainModeules.Utils
{
    public partial class WriteTools : BindableBase
    {
        public static WriteTools Instance { get; set; } = new WriteTools();

        private WriteTools()
        {
            
        }
        /// <summary>
        /// 写指令
        /// </summary>
        /// <param name="commandstr">DB1-BOOL-False</param>      
        public void Write(ElfContent content)
        {

            var command = content.Down.Split('-');
            if (command.Length == 3)
            {
                bool result = false;

                try
                {
                    switch (command[1])
                    {
                        case "BOOL":
                            {
                                result = Write(command[0], bool.Parse(command[2]));
                                break;
                            }
                        case "SHORT":
                            {
                                result = Write(command[0], short.Parse(command[2]));
                                break;
                            }
                        case "FLOAT":
                            {
                                result = Write(command[0], float.Parse(command[2]));
                                break;
                            }
                        case "INT16":
                            {
                                result = Write(command[0], Int16.Parse(command[2]));
                                break;
                            }
                        case "INT32":
                            {
                                result = Write(command[0], Int32.Parse(command[2]));
                                break;
                            }
                        case "INT64":
                            {
                                result = Write(command[0], Int64.Parse(command[2]));
                                break;
                            }
                        case "DOUBLE":
                            {
                                result = Write(command[0], double.Parse(command[2]));
                                break;
                            }
                        case "BYTE":
                            {
                                result = Write(command[0], byte.Parse(command[2]));
                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    Growl.ErrorGlobal($"指令异常-{content.Content}-{content.Down} - {ex.Message}");
                }

                Debug.WriteLine(result);
            }
            else
            {
                Growl.ErrorGlobal($"指令异常-{content.Down}");
            }
        }

        #region 写入方法

        private bool Write(string pos, bool value)
        {
            if (PlcConnect.GoOn)
            {
                var res = PlcConnect.Plc.Write(pos, value);
                return res.IsSuccess;
            }
            else
            {
                Growl.ErrorGlobal($"Write PLC Connect Error~ command[0]-{pos} command[2]-{value}");
                return false;
            }
        }

        private bool Write(string pos, short value)
        {
            if (PlcConnect.GoOn)
            {
                var res = PlcConnect.Plc.Write(pos, value);
                return res.IsSuccess;
            }
            else
            {
                Growl.ErrorGlobal($"Write PLC Connect Error~ command[0]-{pos} command[2]-{value}");
                return false;
            }
        }

        private bool Write(string pos, float value)
        {
            if (PlcConnect.GoOn)
            {
                var res = PlcConnect.Plc.Write(pos, value);
                return res.IsSuccess;
            }
            else
            {
                Growl.ErrorGlobal($"Write PLC Connect Error~ command[0]-{pos} command[2]-{value}");
                return false;
            }
        }

        private bool Write(string pos, double value)
        {
            if (PlcConnect.GoOn)
            {
                var res = PlcConnect.Plc.Write(pos, value);
                return res.IsSuccess;
            }
            else
            {
                Growl.ErrorGlobal($"Write PLC Connect Error~ command[0]-{pos} command[2]-{value}");
                return false;
            }
        }

        #endregion
    }
}
