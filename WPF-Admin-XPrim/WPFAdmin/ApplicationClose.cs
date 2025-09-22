using WPF.Admin.Service.Services.Garnets;

namespace WPFAdmin;

public partial class App {
    public static void DisposeAppResources() {
        // 清理资源
        //TrackingManager.Instance.Dispose();
        GarnetService.ShutdownGarnet();
        App.DisposeNotifyIcon();
        App.Exit();
    }

    public static void DisposeNotifyIconResources() {
        App.DisposeNotifyIcon();
        App.NotifyViewModel = null; // 释放NotifyViewModel 
    }
}