﻿using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace WPF.Admin.Themes.I18n
{
    public static class I18nLocalizeAttach
    {
        #region 附加属性：AutoLocalize（标记需要本地化的属性）

        /// <summary>
        /// 标记控件的某个属性需要自动本地化（值为“本地化键名的绑定路径”）
        /// </summary>
        public static readonly DependencyProperty AutoLocalizeProperty =
            DependencyProperty.RegisterAttached(
                "AutoLocalize",
                typeof(string),
                typeof(I18nLocalizeAttach),
                new PropertyMetadata(null, OnAutoLocalizeChanged));

        public static string GetAutoLocalize(DependencyObject obj) =>
            (string)obj.GetValue(AutoLocalizeProperty);

        public static void SetAutoLocalize(DependencyObject obj, string value) =>
            obj.SetValue(AutoLocalizeProperty, value);

        #endregion

        #region 核心逻辑：属性变化时初始化本地化绑定

        private static void OnAutoLocalizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // 目标必须是 FrameworkElement（如 TextBlock、Button），且绑定路径不为空
            if (d is not FrameworkElement targetElement || string.IsNullOrEmpty((string)e.NewValue))
            {
                return;
            }

            // 获取“需要本地化的属性”（如 TextBlock.Text、Button.Content）
            // 原理：通过附加属性的“所有者类型+属性名”反向推导（例：LocalizeHelper.AutoLocalize → 对应 TextBlock.Text）
            var target = GetTargetDependencyProperty(d, e);
            if (target == null)
            {
                return;
            }
            if (target is DependencyProperty targetProperty)
            {
                // 构建“键名绑定”（从目标控件的指定路径获取本地化键名）
                var keyBinding = new Binding((string)e.NewValue)
                {
                    Source = targetElement, // 绑定源为当前控件（支持 RelativeSource）
                    Mode = BindingMode.OneWay // 仅需要单向绑定（键名变化 → 本地化文本更新）
                };

                // 构建“本地化绑定”（键名 → LocalizeConverter → 本地化文本）
                var localizeBinding = new Binding()
                {
                    Source = keyBinding, // 绑定源为“键名绑定”的结果
                    Converter = new LocalizeConverter(), // 核心：通过转换器获取本地化文本
                    Mode = BindingMode.OneWay
                };

                // 将本地化绑定应用到目标属性
                BindingOperations.SetBinding(targetElement, targetProperty, localizeBinding);

                // 监听语言切换事件（一键刷新）
                I18nManager.Instance.OnLanguageChanged += () =>
                {
                    // 语言变化时，强制刷新当前绑定（更新UI文本）
                    var bindingExpr = BindingOperations.GetBindingExpression(targetElement, targetProperty);
                    bindingExpr?.UpdateTarget();
                };
            }
        }

        #endregion

        #region 辅助方法：推导需要本地化的目标属性（如 TextBlock.Text）

        /// <summary>
        /// 从附加属性的使用场景，推导“需要本地化的属性”
        /// </summary>
        private static object GetTargetDependencyProperty(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            // 获取附加属性的“所有者类型”（如 TextBlock、Button）
            var targetType = d.GetType();

            // 优先匹配“默认文本属性”（如 TextBlock.Text、Button.Content）
            var defaultTextProperty = DependencyPropertyDescriptor.FromName(
                                          "Text", targetType, targetType)?.DependencyProperty ??
                                      DependencyPropertyDescriptor.FromName(
                                          "Content", targetType, targetType)?.DependencyProperty ??
                                      DependencyPropertyDescriptor.FromName(
                                          "Header", targetType, targetType)?.DependencyProperty;
            ;

            if (defaultTextProperty != null)
            {
                return defaultTextProperty;
            }

            // 若没有默认文本属性，可通过自定义逻辑扩展
            throw new InvalidOperationException($"控件 {targetType.Name} 没有默认的文本属性（Text/Content/Header），无法自动本地化");
        }

        #endregion
    }

    #region 配套转换器：LocalizeConverter（键名 → 本地化文本）

    /// <summary>
    /// 本地化转换器：将“本地化键名”转换为“本地化文本”（复用 I18nManager 逻辑）
    /// </summary>
    public class LocalizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string key;
            if (value is Binding binding)
            {
                key = binding.Path.Path;
            }
            else if (value is string str)
            {
                key = str;
                if (string.IsNullOrEmpty(key))
                {
                    return "[Invalid Key]";
                }
            }
            else
            {
                return "[Invalid Key]";
            }

            // 调用 I18nManager 获取本地化文本
            return I18nManager.Instance.GetString(key);
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("本地化无需反向转换");
        }
    }

    #endregion
}