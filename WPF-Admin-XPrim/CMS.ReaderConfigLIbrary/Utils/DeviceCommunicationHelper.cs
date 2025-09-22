using HslCommunication;
using HslCommunication.Core.Device;

namespace CMS.ReaderConfigLIbrary.Utils {
    public static class DeviceCommunicationHelper {
        public static OperateResult WriteHelper(this DeviceCommunication plc, string address, object value, Type type) {
            if (type == typeof(short))
            {
                var st = short.Parse((string)value);
                return plc.Write(address: address, value: st);
            }
            else if (type == typeof(ushort))
            {
                var ust = ushort.Parse((string)value);
                return plc.Write(address: address, value: ust);
            }

            else if (type == typeof(int))
            {
                var it = int.Parse((string)value);
                return plc.Write(address: address, value: it);
            }

            else if (type == typeof(uint))
            {
                var uit = uint.Parse((string)value);
                return plc.Write(address: address, value: uit);
            }

            else if (type == typeof(long))
            {
                var lt = long.Parse((string)value);
                return plc.Write(address: address, value: lt);
            }

            else if (type == typeof(ulong))
            {
                var ult = ulong.Parse((string)value);
                return plc.Write(address: address, value: ult);
            }

            else if (type == typeof(float))
            {
                var ft = float.Parse((string)value);
                return plc.Write(address: address, value: ft);
            }

            else if (type == typeof(double))
            {
                var dt = double.Parse((string)value);
                return plc.Write(address: address, value: dt);
            }

            else if (type == typeof(bool) )
            {
                var b = bool.Parse((string)value);
                return plc.Write(address: address, value: b);
            }

            else if (type == typeof(byte))
            {
                var bt = byte.Parse((string)value);
                return plc.Write(address: address, value: bt);
            }

            else if (type == typeof(sbyte) )
            {
                var sbt = sbyte.Parse((string)value);
                return plc.Write(address: address, value: sbt);
            }

            else if (type == typeof(string))
            {
                var str = (string)value;
                return plc.Write(address: address, value: str);
            }

            else if (type == typeof(byte[]))
            {
                var bts = (byte[])value;
                return plc.Write(address: address, value: bts);
            }


            throw new Exception("未实现");
        }
    }
}