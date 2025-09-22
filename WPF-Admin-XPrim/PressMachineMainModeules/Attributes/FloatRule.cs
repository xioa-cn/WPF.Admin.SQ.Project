using System.Globalization;
using System.Windows.Controls;
using PressMachineMainModeules.Config;
using WPF.Admin.Themes.I18n;

namespace PressMachineMainModeules.Attributes
{
    public class FloatRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return ValidationResult.ValidResult;

            if (!float.TryParse(value.ToString(), out float floatValue))
                return new ValidationResult(false, Common.t("Msg.Rule.ValidFigure"));

            if (floatValue < Min || floatValue > Max)
                return new ValidationResult(false, $"{Common.t("Msg.Rule.ParameterRange")}{Min}~{Max}");

            return ValidationResult.ValidResult;
        }

        //public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        //{
        //    string vs = (string)value;

        //    if (string.IsNullOrEmpty(vs))
        //    {
        //        return new ValidationResult(false, ErrorContent);
        //    }

        //    var result = float.TryParse(vs, out float v);
        //    if (!result)
        //    {
        //        return new ValidationResult(false, ErrorContent);
        //    }

        //    if (v < Min || v > Max)
        //    {
        //        return new ValidationResult(false, ErrorContent);
        //    }
        //    else
        //    {
        //        return ValidationResult.ValidResult;
        //    }
        //}

        /// <summary>
        /// 最小值
        /// </summary>
        public float Min { get; set; }

        /// <summary>
        /// 最大值
        /// </summary>
        public float Max { get; set; }

        /// <summary>
        /// 错误提示内容
        /// </summary>
        public string ErrorContent { get; set; }
    }
}
