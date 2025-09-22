using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

namespace WPF.Admin.Themes.I18n
{
    [MarkupExtensionReturnType(typeof(string))]
    public class LocalizeExtension : MarkupExtension
    {
        private string _key;
        private DependencyObject _targetObject;
        private DependencyProperty _targetProperty;

        public LocalizeExtension(string key)
        {
            _key = key;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // 获取目标对象和属性
            var target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            
            // 处理模板场景：当目标是模板时，延迟到实例化时再订阅事件
            if (target?.TargetObject is DependencyObject targetObject &&
                target.TargetProperty is DependencyProperty targetProperty)
            {
                _targetObject = targetObject;
                _targetProperty = targetProperty;

                // 订阅语言变化事件（使用弱引用包装，避免内存泄漏）
                WeakReference<DependencyObject> weakTarget = new WeakReference<DependencyObject>(targetObject);
                Action updateAction = () =>
                {
                    if (weakTarget.TryGetTarget(out var obj) && _targetProperty != null)
                    {
                        obj.SetValue(_targetProperty, GetLocalizedValue());
                    }
                };

                // 关键：确保事件订阅不会持有目标对象的强引用
                I18nManager.Instance.OnLanguageChanged += updateAction;

                // 注册清理逻辑：当目标对象被销毁时，移除事件订阅
                var cleanup = new CleanupHandler(weakTarget, updateAction);
                DependencyPropertyDescriptor.FromProperty(FrameworkElement.DataContextProperty, typeof(FrameworkElement))
                    ?.AddValueChanged(targetObject, cleanup.OnTargetDestroyed);
            }

            return GetLocalizedValue();
        }

        private string GetLocalizedValue()
        {
            return I18nManager.Instance.GetString(_key) ?? $"[{_key}]";
        }

        // 用于清理事件订阅的辅助类
        public class CleanupHandler
        {
            private readonly WeakReference<DependencyObject> _weakTarget;
            private readonly Action _updateAction;

            public CleanupHandler(WeakReference<DependencyObject> weakTarget, Action updateAction)
            {
                _weakTarget = weakTarget;
                _updateAction = updateAction;
            }

            public void OnTargetDestroyed(object sender, EventArgs e)
            {
                if (!_weakTarget.TryGetTarget(out _))
                {
                    // 目标对象已销毁，移除事件订阅
                    I18nManager.Instance.OnLanguageChanged -= _updateAction;
                }
            }
        }
    }
}
    