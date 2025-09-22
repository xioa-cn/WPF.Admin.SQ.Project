using System.Windows;

namespace WPFAdmin {
    public partial class App {
        public static void Exit() {
            if (Process is null)
            {
                return;
            }
          
            Process.Kill();
            Process.Dispose();
        }
    }
}