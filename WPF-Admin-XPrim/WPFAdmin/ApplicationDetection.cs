using System.Text;
using System.Windows;
using WPF.Admin.Models.Models;
using WPF.Admin.Service.Services;
using WPF.SharedMemory.Model;
using WPF.SharedMemory.Services;
using MessageBox = HandyControl.Controls.MessageBox;

namespace WPFAdmin;

public partial class App {
    private SharedMemoryPubSub? _sharedMemoryPubSub;

    private const string AppOpen = "OPEN";
    private bool Detection {
        get
        {
            _sharedMemoryPubSub ??= new SharedMemoryPubSub(nameof(WPFAdmin));
            string? mName = System.Diagnostics.Process.GetCurrentProcess().MainModule?.ModuleName;
            string? pName = System.IO.Path.GetFileNameWithoutExtension(mName);
            if (System.Diagnostics.Process.GetProcessesByName(pName).Length > 1)
            {
                _sharedMemoryPubSub.Publish(MessageTopics.STATUS_UPDATE,
                    Encoding.UTF8.GetBytes(AppOpen)
                );
                return true;
            }
            else
            {
                _sharedMemoryPubSub.Subscribe(MessageTopics.STATUS_UPDATE, OnApplicationOpen);
                return false;
            }
        }
        
    }
    private void OnApplicationOpen((int MessageId, int TopicId, byte[] Data) obj)
    {
        // 如果消息的主题ID不是STATUS_UPDATE，则返回
        if (obj.TopicId != MessageTopics.STATUS_UPDATE)
            return;
        // 将消息的数据转换为字符串
        var ms = Encoding.UTF8.GetString(obj.Data).TrimEnd('\0');
        // 如果字符串等于AppOpen，则调用App.MainShow()方法
        if (ms != AppOpen)
        {
            return;
        }

        if (ApplicationAuthTaskFactory.AuthFlag)
        {
            throw new Exception("授权失败，无法自动显示主界面");
        }
        // 在UI线程上调用App.MainShow()方法
        DispatcherHelper.CheckBeginInvokeOnUI(App.MainShow);
    }
    private void Detect()
    {
        if (!Detection)
        {
            return;
        }

        Application.Current.Shutdown();
        Environment.Exit(0);
        return;
    }
}