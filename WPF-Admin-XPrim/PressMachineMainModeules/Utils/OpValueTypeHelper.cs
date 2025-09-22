using PressMachineMainModeules.Models;

namespace PressMachineMainModeules.Utils {
    public class OpValueTypeHelper {
        public static OpValueType GetValueType(string type) {
            return type.ToLower() switch {
                "int" => OpValueType._int,
                "uint" => OpValueType._uint,
                "ushort" => OpValueType._ushort,
                "short" => OpValueType._short,
                "float" => OpValueType._float,
                "double" => OpValueType._double,
                "bool" => OpValueType._bool,
                "byte" => OpValueType._byte,
                "string" => OpValueType._string,
                "long" => OpValueType._long,
                "ulong" => OpValueType._ulong,
                _ => OpValueType._float
            };
        }
    }
}