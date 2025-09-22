using HslCommunication;
using HslCommunication.Core.Device;
using PressMachineMainModeules.Models;

namespace PressMachineMainModeules.Utils {
    public static class ParameterBaseWriteHelper {
        public static OperateResult Write(this ParameterBase parameterBase, DeviceCommunication deviceCommunication,
            string dbPoint) {
            return parameterBase.ValueType switch {
                OpValueType._bool => deviceCommunication.Write(dbPoint, (int)parameterBase.RealWriteValue() == 1),
                OpValueType._byte => deviceCommunication.Write(dbPoint, (byte)parameterBase.RealWriteValue()),
                OpValueType._int => deviceCommunication.Write(dbPoint, (int)parameterBase.RealWriteValue()),
                OpValueType._uint => deviceCommunication.Write(dbPoint, (uint)parameterBase.RealWriteValue()),
                OpValueType._ushort => deviceCommunication.Write(dbPoint, (ushort)parameterBase.RealWriteValue()),
                OpValueType._short => deviceCommunication.Write(dbPoint, (short)parameterBase.RealWriteValue()),
                OpValueType._float => deviceCommunication.Write(dbPoint, (float)parameterBase.RealWriteValue()),
                OpValueType._double => deviceCommunication.Write(dbPoint, (double)parameterBase.RealWriteValue()),
                OpValueType._long => deviceCommunication.Write(dbPoint, (long)parameterBase.RealWriteValue()),
                OpValueType._ulong => deviceCommunication.Write(dbPoint, (ulong)parameterBase.RealWriteValue()),
                OpValueType._string => deviceCommunication.Write(dbPoint, parameterBase.RealWriteValue().ToString()),
                _ => throw new ArgumentException()
            };
        }
    }
}