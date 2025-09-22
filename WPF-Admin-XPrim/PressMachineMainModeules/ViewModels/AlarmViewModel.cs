using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using PressMachineMainModeules.Models;
using PressMachineMainModeules.Views;
using System.Collections.ObjectModel;
using System.Windows;
using WPF.Admin.Models;
using WPF.Admin.Models.Models;

namespace PressMachineMainModeules.ViewModels
{
    public partial class AlarmViewModel : BindableBase
    {
        private AlarmMessageWindow? AlarmMessageWindow;
        public AlarmViewModel()
        {

            WeakReferenceMessenger.Default.Register<AlarmToUIModels>(this, AlarmsFunc);
        }

        public void Initialized()
        {
            AlarmMessageWindow = new AlarmMessageWindow()
            {
                DataContext = this,
            };
        }
        public ObservableCollection<string> Alarms { get; set; } = new ObservableCollection<string>();

        [ObservableProperty] private Visibility? _visibilityAlarm = Visibility.Hidden;

        private void AlarmsFunc(object recipient, AlarmToUIModels message)
        {
            if (message.ToUIType == ToUIEnum.Remove)
            {
                foreach (var item in message.Alarms)
                {
                    Alarms.Remove(item);
                }
            }
            else if (message.ToUIType == ToUIEnum.Add)
            {
                AlarmMessageWindow?.Show();
                foreach (var item in message.Alarms)
                {
                    if (!Alarms.Contains(item))
                    {
                        AddAlarm(item);
                    }
                }
            }
            else if (message.ToUIType == ToUIEnum.Normal)
            {
                Alarms.Clear();
                foreach (var item in message.Alarms)
                {
                    if (!Alarms.Contains(item))
                    {
                        AddAlarm(item);
                    }
                }
            }
            IsNeedShow();
        }



        private void AddAlarm(string alarm)
        {

            this.Alarms.Add(alarm);
            //// 提取报警信息的关键部分（例如去除时间戳）
            //string alarmKey = ExtractAlarmKey(alarm);

            //// 检查是否已存在相同的关键信息
            //if (!Alarms.Any(existingAlarm => ExtractAlarmKey(existingAlarm) == alarmKey))
            //{
            //    this.Alarms.Add(alarm);
            //}
        }

        private string ExtractAlarmKey(string alarm)
        {
            // 这里可以实现自定义的报警信息处理逻辑
            // 例如：移除时间戳，只保留实际报警内容
            // 或者提取特定的报警代码等
            return alarm.Split([DateTime.Now.ToString("yyyy-MM-dd")], StringSplitOptions.None).Last().Trim();
        }

        private void IsNeedShow()
        {
            if (Alarms.Count > 0)
            {
                AlarmMessageWindow?.Show();
                ApplicationAlarm.AlarmStatus();
                this.VisibilityAlarm = Visibility.Visible;
            }
            else
            {
                ApplicationAlarm.NoAlarmStatus();
                this.VisibilityAlarm = Visibility.Hidden;
            }
        }
    }
}
