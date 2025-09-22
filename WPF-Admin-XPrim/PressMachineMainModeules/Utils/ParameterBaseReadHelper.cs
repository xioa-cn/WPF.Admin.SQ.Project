using HandyControl.Controls;
using HslCommunication;
using HslCommunication.Core.Device;
using PressMachineMainModeules.Models;
using WPF.Admin.Service.Logger;

namespace PressMachineMainModeules.Utils
{
    public static class ParameterBaseReadHelper
    {
        public static OperateResult Read(this ParameterBase parameterBase, DeviceCommunication deviceCommunication,
            string dbPoint)
        {
            switch (parameterBase.ValueType)
            {
                case OpValueType._bool:
                {
                    var result = deviceCommunication.ReadBool(dbPoint);
                    if (result.IsSuccess)
                    {
                        parameterBase.RealReadValue(result.Content ? 1 : 0);
                    }

                    return result;
                }
                case OpValueType._byte:
                {
                    var result = deviceCommunication.Read(dbPoint, 1);
                    if (result.IsSuccess)
                    {
                        parameterBase.RealReadValue(result.Content[0] == (byte)1 ? 1 : 0);
                    }

                    return result;
                }
                case OpValueType._short:
                {
                    var result = deviceCommunication.ReadInt16(dbPoint);
                    if (result.IsSuccess)
                    {
                        parameterBase.RealReadValue(result.Content);
                    }

                    return result;
                }
                case OpValueType._ushort:
                {
                    var result = deviceCommunication.ReadUInt16(dbPoint);
                    if (result.IsSuccess)
                    {
                        parameterBase.RealReadValue(result.Content);
                    }

                    return result;
                }
                case OpValueType._int:
                {
                    var result = deviceCommunication.ReadInt32(dbPoint);
                    if (result.IsSuccess)
                    {
                        parameterBase.RealReadValue(result.Content);
                    }

                    return result;
                }
                case OpValueType._uint:
                {
                    var result = deviceCommunication.ReadUInt32(dbPoint);
                    if (result.IsSuccess)
                    {
                        parameterBase.RealReadValue(result.Content);
                    }

                    return result;
                }
                case OpValueType._long:
                {
                    var result = deviceCommunication.ReadInt64(dbPoint);
                    if (result.IsSuccess)
                    {
                        parameterBase.RealReadValue(result.Content);
                    }

                    return result;
                }
                case OpValueType._ulong:
                {
                    var result = deviceCommunication.ReadUInt64(dbPoint);
                    if (result.IsSuccess)
                    {
                        parameterBase.RealReadValue(result.Content);
                    }

                    return result;
                }
                case OpValueType._float:
                {
                    var result = deviceCommunication.ReadFloat(dbPoint);
                    if (result.IsSuccess)
                    {
                        parameterBase.RealReadValue(result.Content);
                    }

                    return result;
                }
                case OpValueType._double:
                {
                    var result = deviceCommunication.ReadDouble(dbPoint);
                    if (result.IsSuccess)
                    {
                        parameterBase.RealReadValue((float)result.Content);
                    }

                    return result;
                }
                case OpValueType._string:
                default:
                {
                    XLogGlobal.Logger?.LogError(
                        $"class {nameof(ParameterBaseReadHelper)} Not Support {parameterBase.ValueType}");
                    Growl.ErrorGlobal($"class:ParameterBaseReadHelper Methods:Read" +
                                      $"\n 类型错误：暂不支持类型{parameterBase.ValueType} {dbPoint}");
                    break;
                }
            }

            throw new ArgumentException();
        }

        public static object? Read(string dbPoint, DeviceCommunication deviceCommunication, OpValueType valueType,
            ushort stringLength = 1)
        {
            switch (valueType)
            {
                case OpValueType._string:
                {
                    var reuslt = deviceCommunication.ReadString(dbPoint, stringLength);
                    if (reuslt.IsSuccess)
                    {
                        return reuslt.Content;
                    }

                    return null;
                }
                case OpValueType._short:
                {
                    var result = deviceCommunication.ReadInt16(dbPoint);
                    if (result.IsSuccess)
                    {
                        return (int)result.Content;
                    }

                    return null;
                }
                case OpValueType._ushort:
                {
                    var result = deviceCommunication.ReadUInt16(dbPoint);
                    if (result.IsSuccess)
                    {
                        return (int)result.Content;
                    }

                    return null;
                }
                case OpValueType._int:
                {
                    var result = deviceCommunication.ReadInt32(dbPoint);
                    if (result.IsSuccess)
                    {
                        return (int)result.Content;
                    }

                    return null;
                }
                case OpValueType._uint:
                {
                    var result = deviceCommunication.ReadUInt32(dbPoint);
                    if (result.IsSuccess)
                    {
                        return (int)result.Content;
                    }

                    return null;
                }
                case OpValueType._float:
                {
                    var result = deviceCommunication.ReadFloat(dbPoint);
                    if (result.IsSuccess)
                    {
                        return (float)result.Content;
                    }

                    return null;
                }
                case OpValueType._double:
                {
                    var result = deviceCommunication.ReadDouble(dbPoint);
                    if (result.IsSuccess)
                    {
                        return (double)result.Content;
                    }

                    return null;
                }
                case OpValueType._bool:
                {
                    var result = deviceCommunication.ReadBool(dbPoint);
                    if (result.IsSuccess)
                        return (bool)result.Content;
                    return null;
                }
                case OpValueType._long:
                {
                    var result = deviceCommunication.ReadInt64(dbPoint);
                    if (result.IsSuccess)
                    {
                        return (long)result.Content;
                    }

                    return null;
                }
                case OpValueType._ulong:
                {
                    var result = deviceCommunication.ReadInt64(dbPoint);
                    if (result.IsSuccess)
                    {
                        return (long)result.Content;
                    }

                    return null;
                }
                case OpValueType._byte:
                {
                    var result = deviceCommunication.Read(dbPoint, 1);
                    if (result.IsSuccess)
                    {
                        return result.Content;
                    }

                    return null;
                }
                default:
                {
                    XLogGlobal.Logger?.LogError(
                        $"class {nameof(ParameterBaseReadHelper)} Not Support {valueType}");
                    Growl.ErrorGlobal($"class:ParameterBaseReadHelper Methods:Read" +
                                      $"\n 类型错误：暂不支持类型{valueType} {dbPoint}");
                    break;
                }
            }

            return null;
        }
    }
}