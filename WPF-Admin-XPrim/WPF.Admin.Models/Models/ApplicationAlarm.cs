using CommunityToolkit.Mvvm.Messaging;

namespace WPF.Admin.Models.Models;

public class ApplicationAlarm
{
    public bool Alarm { get; set; }

    public ApplicationAlarm()
    {
    }

    public ApplicationAlarm(bool alarm)
    {
        this.Alarm = alarm;
    }

    private static bool oldState = false;

    public static void ApplicationAlarmChange(bool alarm)
    {
        if (ApplicationAuthTaskFactory.AuthFlag)
        {
            throw new Exception("授权失败，无法实现报警状态");
        }
        if (oldState == alarm) return;

        WeakReferenceMessenger.Default.Send(new ApplicationAlarm(alarm));
        oldState = alarm;
    }

    public static void AlarmStatus()
    {
        ApplicationAlarmChange(true);
    }

    public static void NoAlarmStatus()
    {
        ApplicationAlarmChange(false);
    }
}