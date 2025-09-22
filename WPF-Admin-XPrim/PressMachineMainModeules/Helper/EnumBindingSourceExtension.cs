using System.Windows.Markup;
using System.ComponentModel;

namespace PressMachineMainModeules.Helper
{
    public class EnumBindingSourceExtension : MarkupExtension
    {
        private Type _enumType;

        public EnumBindingSourceExtension(Type enumType)
        {
            if (enumType == null)
                throw new ArgumentNullException(nameof(enumType));

            EnumType = enumType;
        }

        public Type EnumType
        {
            get { return _enumType; }
            private set
            {
                if (value != _enumType)
                {
                    if (null != value)
                    {
                        Type enumType = Nullable.GetUnderlyingType(value) ?? value;
                        if (!enumType.IsEnum)
                            throw new ArgumentException("Type must be an Enum.");
                    }

                    _enumType = value;
                }
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var enumType = Nullable.GetUnderlyingType(EnumType) ?? EnumType;
            var enumValues = Enum.GetValues(enumType);

            // 将枚举值转换为 EnumDescriptionItem 对象数组
            return enumValues
                .Cast<object>()
                .Select(enumValue => new EnumDescriptionItem
                {
                    Value = enumValue,           // 存储原始枚举值
                    Description = GetDescription(enumValue)  // 存储中文描述
                })
                .ToArray();
        }

        private string GetDescription(object enumValue)
        {
            var descriptionAttribute = enumValue.GetType()
                .GetField(enumValue.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .FirstOrDefault() as DescriptionAttribute;

            return descriptionAttribute?.Description ?? enumValue.ToString();
        }
    }

    public class EnumDescriptionItem
    {
        public object Value { get; set; }
        public string Description { get; set; }
    }
}
