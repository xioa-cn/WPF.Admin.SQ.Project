using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using WPF.Admin.Models.Models;
using WPF.Admin.Themes.I18n;

namespace SystemModules.Converter
{
    // 清理逻辑辅助类：避免强引用导致内存泄漏
    internal class CleanupHandler
    {
        private readonly WeakReference<DependencyObject> _weakTarget;
        private readonly Action _updateAction;

        public CleanupHandler(WeakReference<DependencyObject> weakTarget, Action updateAction)
        {
            _weakTarget = weakTarget;
            _updateAction = updateAction;
        }

        // 目标对象销毁时，移除语言变更事件订阅
        public void OnTargetDestroyed(object sender, EventArgs e)
        {
            if (_weakTarget.TryGetTarget(out var target))
            {
                // 移除事件订阅，避免内存泄漏
                I18nManager.Instance.OnLanguageChanged -= _updateAction;
                
                // 解绑ValueChanged事件，防止重复触发
                if (sender is DependencyObject obj)
                {
                    DependencyPropertyDescriptor.FromProperty(FrameworkElement.DataContextProperty, obj.GetType())
                        ?.RemoveValueChanged(obj, OnTargetDestroyed);
                }
            }
        }
    }

    public class LocalizeAuthConverter : MarkupExtension, IValueConverter, INotifyPropertyChanged
    {
        // 单例实例：确保所有绑定共享同一个转换器（避免重复订阅事件）
        private static LocalizeAuthConverter _instance;

        // 存储绑定目标信息（弱引用，避免内存泄漏）
        private readonly WeakReference<DependencyObject> _weakTargetObject;
        private readonly DependencyProperty _targetProperty;


        private LocalizeAuthConverter(DependencyObject targetObject, DependencyProperty targetProperty)
        {
            // 用弱引用存储目标对象，防止转换器强引用UI元素导致内存泄漏
            _weakTargetObject = new WeakReference<DependencyObject>(targetObject);
            _targetProperty = targetProperty;

            // 订阅语言变更事件（仅单例订阅一次，避免重复订阅）
            I18nManager.Instance.OnLanguageChanged += OnLanguageChanged;
        }

        // 语言变更时：触发绑定目标更新
        private void OnLanguageChanged()
        {
            // 1. 触发PropertyChanged：通知绑定系统转换器本身已变更
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Converter)));

            // 2. 强制更新绑定目标（双重保障，确保UI刷新）
            if (_weakTargetObject.TryGetTarget(out var targetObject))
            {
                // 获取当前绑定的数据源（LoginAuth值）
                var binding = BindingOperations.GetBinding(targetObject, _targetProperty);
                if (binding != null)
                {
                    // 重新计算本地化值并更新目标属性
                    var dataContext = targetObject.GetValue(FrameworkElement.DataContextProperty);
                    var authValue = binding.Path.Path != null 
                        ? binding.Path.Path.Split('.').Aggregate(dataContext, (obj, prop) => obj?.GetType().GetProperty(prop)?.GetValue(obj)) 
                        : null;

                    // 强制设置目标属性值，触发UI刷新
                    if (authValue is LoginAuth auth)
                    {
                        var localizedValue = GetLocalizedValue(auth);
                        targetObject.SetValue(_targetProperty, localizedValue);
                    }
                }
            }
        }

        // 核心：根据LoginAuth枚举获取本地化文本（修复原代码中硬编码中文的问题）
        private object GetLocalizedValue(LoginAuth auth)
        {
            return auth switch
            {
                LoginAuth.Admin => SystemModules.t("Admin"),
                LoginAuth.Engineer => SystemModules.t("Engineer"),
                LoginAuth.Employee => SystemModules.t("Employee"),
                LoginAuth.FUser => SystemModules.t("FrontendUser"), // 改为本地化Key（需在语言文件中配置）
                LoginAuth.HUser => SystemModules.t("BackendUser"),  // 改为本地化Key（需在语言文件中配置）
                _ => SystemModules.t("NoPermission")               // 改为本地化Key（需在语言文件中配置）
            };
        }

        #region IValueConverter 实现
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LoginAuth auth)
            {
                return GetLocalizedValue(auth); // 复用本地化逻辑，避免代码重复
            }
            return DependencyProperty.UnsetValue; // 无效值时返回默认，避免绑定错误
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("不需要反向转换（枚举→文本）");
        }
        #endregion

        #region MarkupExtension 实现（关键：处理绑定目标并创建单例）
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // 1. 获取绑定目标信息（目标对象+目标属性，如TextBlock.Text）
            var valueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (valueTarget == null || !(valueTarget.TargetObject is DependencyObject targetObject) || !(valueTarget.TargetProperty is DependencyProperty targetProperty))
            {
                // 若目标是模板（如DataTemplate），返回自身（模板实例化时会重新调用）
                return this;
            }

            // 2. 创建单例：确保全局仅一个转换器实例，避免重复订阅语言事件
            if (_instance == null)
            {
                _instance = new LocalizeAuthConverter(targetObject, targetProperty);
            }
            else
            {
                // 若单例已存在，更新单例的目标信息（适配多绑定场景）
                _instance._weakTargetObject.SetTarget(targetObject);
                //_instance._targetProperty = targetProperty;
            }

            // 3. 注册清理逻辑：目标对象销毁时移除事件订阅，防止内存泄漏
            var cleanupHandler = new CleanupHandler(_instance._weakTargetObject, _instance.OnLanguageChanged);
            DependencyPropertyDescriptor.FromProperty(FrameworkElement.DataContextProperty, targetObject.GetType())
                ?.AddValueChanged(targetObject, cleanupHandler.OnTargetDestroyed);

            return _instance;
        }
        #endregion

        #region INotifyPropertyChanged 实现
        // 用于触发绑定更新的"占位"属性（绑定系统会监听此属性变更）
        public IValueConverter Converter => this;

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}