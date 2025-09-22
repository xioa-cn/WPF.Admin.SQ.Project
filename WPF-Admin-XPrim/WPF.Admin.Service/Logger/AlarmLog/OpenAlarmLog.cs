using CommunityToolkit.Mvvm.Messaging;
using WPF.Admin.Models.Models;
using WPF.Admin.Models.Utils;

namespace WPF.Admin.Service.Logger.AlarmLog {
    public class OpenAlarmLog {
        private bool _isInitialized = false;

        private bool IsInitialized {
            get => _isInitialized;
            set
            {
                if (_isInitialized)
                {
                    return;
                }

                _isInitialized = value;
            }
        }

        private OpenAlarmLog() {
        }

        private static OpenAlarmLog? _alarmLog;

        public static OpenAlarmLog Instance {
            get { return _alarmLog ??= new OpenAlarmLog(); }
        }

        public void Initialized() {
            if (IsInitialized)
                return;
            WeakReferenceMessenger.Default.Register<AlarmToUIModels>(this, AlarmsFunc);
            IsInitialized = false;
        }

        private List<string> alarmList { get; set; } = new List<string>();

        private void AlarmsFunc(object recipient, AlarmToUIModels message) {
            switch (message.ToUIType)
            {
                case ToUIEnum.Add: {
                    foreach (var item in message.Alarms)
                    {
                        if (alarmList.Contains(item))
                        {
                            continue;
                        }
                        else
                        {
                            alarmList.Add(item);
                            {
                                using var db = AlarmDbInstance.CreateNormal();
                                db.AlarmLogs.Add(AlarmLogHelper.DeviceLog(item));
                                db.SaveChanges();
                            }
                        }
                    }

                    break;
                }
                case ToUIEnum.Remove: {
                    foreach (var item in message.Alarms)
                    {
                        alarmList.Remove(item);
                    }

                    break;
                }
                case ToUIEnum.Normal: {
                    if (message.Alarms is null || message.Alarms.Count == 0)
                    {
                        alarmList.Clear();
                        return;
                    }

                    foreach (var item in message.Alarms.Where(item => !alarmList.Contains(item)))
                    {
                        alarmList.Add(item);
                        {
                            using var db = AlarmDbInstance.CreateNormal();
                            db.AlarmLogs.Add(AlarmLogHelper.DeviceLog(item));
                            db.SaveChanges();
                        }
                    }

                    foreach (var item in alarmList.Where(item => !message.Alarms.Contains(item)).ToList())
                    {
                        alarmList.Remove(item);
                    }


                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}