using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressMachineMainModeules.Models
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum AlarmReaction
    {
        [Description("停止")]
        Stop,
        [Description("返回原点")]
        ReturnOrigin,
        [Description("继续")]
        Continue
    }
}
