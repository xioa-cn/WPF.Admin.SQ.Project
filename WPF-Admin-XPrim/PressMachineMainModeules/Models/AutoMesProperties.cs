using PressMachineMainModeules.Config;
using PressMachineMainModeules.Utils;
using WPF.Admin.Themes.Converter;

namespace PressMachineMainModeules.Models {
    public class AutoMesProperties {
        public string Code { get; set; }
        public string Recipe { get; set; }
        public string ResultString { get; set; }
        public int ResultInt { get; set; }
        public float MaxPre { get; set; }
        public float FinalPre { get; set; }
        public float MaxPos { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinalTime { get; set; }

        public double Timing {
            get { return (FinalTime - StartTime).TotalSeconds; }
        }

        public int Step { get; set; }
        public string AutoModeID { get; set; }
        public string FilePath { get; set; }

        public string? LoginUser {
            get { return LoginAuthHelper.LoginUser?.UserName; }
        }

        public object? this[string property] {
            get
            {
                var propertyNameList = property.Split('-');
                if (propertyNameList.Length == 3)
                {
                    if (propertyNameList[2].ToLower().Contains("string"))
                    {
                        var length = int.Parse(propertyNameList[2].Replace("string", ""));
                        var type = OpValueTypeHelper.GetValueType("string");
                        return ParameterBaseReadHelper.Read(propertyNameList[1],
                            ConfigPlcs.Instance[propertyNameList[0]], type, (ushort)length);
                    }
                    else
                    {
                        var type = OpValueTypeHelper.GetValueType(propertyNameList[2]);
                        return ParameterBaseReadHelper.Read(propertyNameList[1],
                            ConfigPlcs.Instance[propertyNameList[0]],
                            type);
                    }
                }

                return this.GetType()?.GetProperty(property)?.GetValue(this, null) ?? "{NULLABLE}";
            }
        }

        public string Analysis(string sendString) {
            var result = "";
            var tempSendStringList = sendString.Split("%");
            for (int i = 1; i < tempSendStringList.Length + 1; i++)
            {
                if (tempSendStringList[i - 1].Contains("{") && tempSendStringList[i - 1].Contains("}"))
                {
                    result += this[tempSendStringList[i - 1].Replace("{", "").Replace("}", "")] ??
                              tempSendStringList[i - 1];
                }
                else
                {
                    result += tempSendStringList[i - 1];
                }
            }

            return result;
        }
    }
}