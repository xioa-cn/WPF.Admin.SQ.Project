using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PressMachineMainModeules.Models;


public class EnumDescriptionTypeConverter : EnumConverter
{
    public EnumDescriptionTypeConverter(Type type) : base(type)
    {
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (destinationType == typeof(string))
        {
            if (null != value)
            {
                FieldInfo fi = value.GetType().GetField(value.ToString());

                if (null != fi)
                {
                    var attributes =
                        (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                    return ((attributes.Length > 0) && (!string.IsNullOrEmpty(attributes[0].Description)))
                        ? attributes[0].Description
                        : value.ToString();
                }
            }

            return string.Empty;
        }
        return base.ConvertTo(context, culture, value, destinationType);
    }
}

[Serializable]
[TypeConverter(typeof(EnumDescriptionTypeConverter))]
public enum Result
{
    [Description("合格")]
    Ok = 1,
    [Description("不合格")]
    Ng = 2,
    [Description("无")]
    None = 0
}

