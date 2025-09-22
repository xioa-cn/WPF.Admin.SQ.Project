using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Data;

namespace PressMachineMainModeules.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class FloatRuleAttribute : ValidationAttribute
    {
        public float Min { get; }
        public float Max { get; }

        public FloatRuleAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // 如果值为空，返回成功（因为通常会配合Required特性使用）
            if (value == null)
            {
                return ValidationResult.Success;
            }

            // 尝试转换为float
            if (!float.TryParse(value.ToString(), out float floatValue))
            {
                return new ValidationResult($"请输入有效的数字");
            }

            // 检查范围
            if (floatValue < Min || floatValue > Max)
            {
                return new ValidationResult(ErrorMessage ?? $"数值必须在 {Min} 到 {Max} 之间");
            }

            return ValidationResult.Success;
        }
    }

    public class FloatValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float floatValue)
            {
                return floatValue.ToString();
            }
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                if (string.IsNullOrWhiteSpace(stringValue) || 
                    stringValue == "." || 
                    stringValue == "-" ||
                    stringValue == "。")
                {
                    return null;
                }

                if (float.TryParse(stringValue, out float result))
                {
                    return result;
                }
                return null;
            }
            return null;
        }
    }
}
