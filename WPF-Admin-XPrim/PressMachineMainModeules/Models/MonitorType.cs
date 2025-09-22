using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressMachineMainModeules.Models
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum MonitorType
    {
        [Description("下进右出")]
        BottomInRightOut,
        [Description("下进下出")]
        BottomInBottomOut,
        [Description("左进右出")]
        LeftInRightOut,
        [Description("左进上出")]
        LeftInTopOut,
        [Description("下进不出")]
        BottomInNoOut,
        [Description("左进右不出")]
        LeftInRightNoOut,
    }
}
