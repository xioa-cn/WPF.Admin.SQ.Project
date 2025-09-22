using System.Windows;
using WPF.Admin.Service.Logger;
using WPF.Admin.Service.Logger.AlarmLog;
using WPFAdmin.Config;

namespace WPFAdmin {
    public partial class App {
        private void GlobalExceptionHandle() {
            // 设置全局异常处理
            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                XLogGlobal.Logger?.LogFatal("Unhandled Exception", args.ExceptionObject as Exception);
                if (args.ExceptionObject is Exception exception)
                {
                    MessageBox.Show(exception.Message.Contains("授权失败") ? "授权失败，请联系管理员" : exception.Message, "错误",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            Current.DispatcherUnhandledException += (s, args) =>
            {
                XLogGlobal.Logger?.LogError("Dispatcher Unhandled Exception", args.Exception);
                MessageBox.Show(args.Exception.Message.Contains("授权失败") ? "授权失败，请联系管理员" : args.Exception.Message, "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                //args.Handled = true;
            };

            if (Configs.Default is not null && Configs.Default.OpenAlarmLog)
            {
                OpenAlarmLog.Instance.Initialized();
            }
        }
    }
}