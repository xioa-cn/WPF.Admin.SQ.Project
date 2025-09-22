using System.Text;
using System.Windows.Threading;
using WPF.Admin.Models.Models;
using WPF.Admin.Service.Logger;

namespace WPF.Admin.Service.Services {
    public static class DispatcherHelper {
        public static Dispatcher? UIDispatcher { get; private set; }

        public static void CheckBeginInvokeOnUI(Action action) {
            if (ApplicationAuthTaskFactory.AuthFlag)
            {
                XLogGlobal.Logger?.LogError("授权失败，无法捕捉UI");
                return;
            }

            if (action != null)
            {
                CheckDispatcher();
                if (UIDispatcher!.CheckAccess())
                {
                    action();
                }
                else
                {
                    UIDispatcher.BeginInvoke(action);
                }
            }
        }

        private static void CheckDispatcher() {
            if (ApplicationAuthTaskFactory.AuthFlag)
            {
                throw new Exception("授权失败，无法捕捉UI");
            }

            if (UIDispatcher == null)
            {
                StringBuilder stringBuilder = new("The DispatcherHelper is not initialized.");
                stringBuilder.AppendLine();
                stringBuilder.Append("Call DispatcherHelper.Initialize() in the static App constructor.");
                throw new InvalidOperationException(stringBuilder.ToString());
            }
        }

        public static DispatcherOperation RunAsync(Action action) {
            if (ApplicationAuthTaskFactory.AuthFlag)
            {
                throw new Exception("授权失败，无法捕捉UI");
            }

            CheckDispatcher();
            return UIDispatcher!.BeginInvoke(action);
        }

        public static void Initialize() {
            if (ApplicationAuthTaskFactory.AuthFlag)
            {
                throw new Exception("授权失败，无法捕捉UI");
            }

            if (UIDispatcher == null || !UIDispatcher.Thread.IsAlive)
            {
                UIDispatcher = Dispatcher.CurrentDispatcher;
            }
        }

        public static void Reset() {
            UIDispatcher = null;
        }
    }
}