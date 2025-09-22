using CommunityToolkit.Mvvm.ComponentModel;
using PressMachineMainModeules.Config;
using System.Collections.ObjectModel;
using System.ComponentModel;
using HandyControl.Controls;
using PressMachineMainModeules.Utils;
using WPF.Admin.Models;
using WPF.Admin.Models.Background;
using WPF.Admin.Service.Services;

namespace PressMachineMainModeules.Models
{
    public partial class AutoParameterContentModel : ParameterBase
    {
        public string Unit { get; set; }
        public string Db { get; set; }
        public string Point { get; set; }
        public string PlcName { get; set; }

        public string getFullPosition
        {
            get
            {
                if (string.IsNullOrEmpty(Point))
                {
                    return $"{Db}";
                }

                return $"{Db}.{Point}";
            }
        }
    }

    public partial class AutoParametersInstance : BindableBase
    {
        public string ParameterName { get; set; }
        public string Type { get; set; }

        public ObservableCollection<AutoParameterContentModel> Content { get; set; } =
            new ObservableCollection<AutoParameterContentModel>();

        [ObservableProperty] public CurvePara? _CurvePara;
        [ObservableProperty] public ObservableCollection<MonitorRec>? _MonitorRecs;
        [ObservableProperty] public EnvelopePosintModel? _EnvelopePosintModel;

        public void CheckAutoMode()
        {
            if (Type == "1")
            {
                CurvePara = new CurvePara();
                MonitorRecs = new ObservableCollection<MonitorRec>();
                EnvelopePosintModel = new EnvelopePosintModel();
            }
            else
            {
                CurvePara = null;
                EnvelopePosintModel = null;
                MonitorRecs = null;
            }
        }
    }

    public class AutoParameterContent
    {
        public string AutoMode { get; set; }
        public string Key { get; set; }
        public string Type { get; set; }
        public string Step { get; set; }
        public AutoParametersInstance Instance { get; set; }
    }

    public class AutoParameterWeakRef
    {
        public class AutoParameterWeakContent
        {
            public CurvePara? CurvePara { get; set; }
            public EnvelopePosintModel? EnvelopePosintModel { get; set; }
            public ObservableCollection<MonitorRec>? MonitorRecs { get; set; }
        }

        public class AutoParameterWeakDic
        {
            [Description("参数类型")] public string Type { get; set; }

            [Description("AutoMode映射")] public string AutoMode { get; set; }

            [Description("对应步骤")] public string Step { get; set; }

            public AutoParameterWeakContent AutoParameterWeakContent { get; set; }
        }

        public class AutoParameterWeakTp
        {
            [Description("参数包名称")] public string ParameterName { get; set; }

            public AutoParameterWeakDic AutoParameterWeakDic { get; set; }
        }

        // key Automode映射
        public Dictionary<string, List<AutoParameterWeakTp>> Parameters { get; set; }
    }


    public partial class AutoParameterSaveEntity
    {
        public partial class SaveContent : BindableBase
        {
            public string Key { get; set; }
            [ObservableProperty] public CurvePara? _CurvePara;
            [ObservableProperty] public EnvelopePosintModel? _EnvelopePosintModel;
            [ObservableProperty] public ObservableCollection<MonitorRec>? _MonitorRecs;

            public Dictionary<string, float> Value { get; set; } = new Dictionary<string, float>();
        }

        public Dictionary<string, SaveContent> Entities { get; set; } = new Dictionary<string, SaveContent>();
    }

    public class AutoParameterModel
    {
        //public Dictionary<string, AutoParametersInstance> ParametersContext { get; set; } = new Dictionary<string, AutoParametersInstance>();
        public ObservableCollection<AutoParameterContent> AutoParameterContents { get; set; } =
            new ObservableCollection<AutoParameterContent>();


        public bool TryGetValue(string key, out AutoParameterContent value)
        {
            var temp = AutoParameterContents?.FirstOrDefault(x => x.Key == key);
            if (temp != null)
            {
                value = temp;
                return true;
            }

            value = null;
            return false;
        }

        public AutoParameterContent this[string key]
        {
            get
            {
                if (this.TryGetValue(key, out var value))
                {
                    return value;
                }

                var nValue = new AutoParameterContent { Key = key, Instance = new AutoParametersInstance() };
                AutoParameterContents.Add(nValue);
                return nValue;
            }
            set
            {
                if (this.TryGetValue(key, out var oldValue))
                {
                    AutoParameterContents.Remove(oldValue);
                }

                AutoParameterContents.Add(value);
            }
        }
    }


    public static class AutoParameterModelHelper
    {
        public static AutoParameterWeakRef ToAutoParameterWeakRefEntity(this AutoParameterModel autoParameterModel,
            string parameterName)
        {
            var autoParameterWeakRef = new AutoParameterWeakRef()
            {
                Parameters = new Dictionary<string, List<AutoParameterWeakRef.AutoParameterWeakTp>>()
            };
            var tempAutoParameterContents = autoParameterModel.AutoParameterContents.Where(e => e.Type == "1").ToList();

            tempAutoParameterContents.ForEach(item =>
            {
                if (!autoParameterWeakRef.Parameters.TryGetValue(item.AutoMode, out var value))
                {
                    autoParameterWeakRef.Parameters[item.AutoMode] =
                        [];
                }

                autoParameterWeakRef.Parameters[item.AutoMode].Add(new AutoParameterWeakRef.AutoParameterWeakTp()
                {
                    ParameterName = parameterName,
                    AutoParameterWeakDic = new AutoParameterWeakRef.AutoParameterWeakDic
                    {
                        Type = item.Type,
                        AutoMode = item.AutoMode,
                        Step = item.Step,
                        AutoParameterWeakContent = new AutoParameterWeakRef.AutoParameterWeakContent
                        {
                            CurvePara = item.Instance.CurvePara,
                            MonitorRecs = item.Instance.MonitorRecs,
                            EnvelopePosintModel = item.Instance.EnvelopePosintModel
                        }
                    }
                });
            });

            return autoParameterWeakRef;
        }

        public static AutoParameterSaveEntity ToSaveEntity(this AutoParameterModel autoParameterModel)
        {
            var result = new AutoParameterSaveEntity();
            foreach (var item in autoParameterModel.AutoParameterContents)
            {
                var saveEntity = new AutoParameterSaveEntity.SaveContent
                {
                    Key = item.Key,
                    CurvePara = item.Instance.CurvePara,
                    EnvelopePosintModel = item.Instance.EnvelopePosintModel,
                    MonitorRecs = item.Instance.MonitorRecs,
                    Value = item.Instance.Content.ToDictionary(a => a.Desc, b => (float)b.Value)
                };

                result.Entities[item.Key] = saveEntity;
            }

            return result;
        }

        public static void GetSaveEntity(this AutoParameterModel autoParameterModel, AutoParameterSaveEntity dto,
            bool isOnUi = true)
        {
            foreach (var item in dto.Entities)
            {
                if (!autoParameterModel.TryGetValue(item.Key, out var autoParameterContent))
                {
                    continue;
                }

                autoParameterContent.Instance.CurvePara = item.Value.CurvePara;
                autoParameterContent.Instance.MonitorRecs = item.Value.MonitorRecs;
                // autoParameterContent.Instance.EnvelopePosintModel = item.Value.EnvelopePosintModel;
                if (isOnUi)
                {
                    autoParameterContent.Instance.EnvelopePosintModel?.Posints.Clear();
                    QueuedHostedService.TaskQueue.QueueBackgroundWorkItem(async (token) =>
                    {
                        // foreach (var envelope in item.Value.EnvelopePosintModel.Posints)
                        // {
                        //     autoParameterContent.Instance.EnvelopePosintModel?.Posints.Add(envelope);
                        // }

                        if (item.Value.EnvelopePosintModel is null) return;
                        var newItems = item.Value.EnvelopePosintModel.Posints.ToList();
                        int batchSize = 10;
                        for (int i = 0; i < item.Value.EnvelopePosintModel.Posints.Count; i += batchSize)
                        {
                            var batch = newItems.Skip(i).Take(batchSize).ToList();

                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                foreach (var envelope in batch)
                                    autoParameterContent.Instance.EnvelopePosintModel?.Posints.Add(envelope);
                            });

                            await Task.Delay(100,token);
                        }
                    });
                }
                else
                {
                    autoParameterContent.Instance.EnvelopePosintModel = item.Value.EnvelopePosintModel;
                }
               


                foreach (var value in item.Value.Value)
                {
                    var find = autoParameterContent.Instance.Content.FirstOrDefault(e => e.Desc == value.Key);
                    if (find is not null)
                    {
                        find.Value = value.Value;
                    }
                }
            }
        }

        public static async Task<bool> WriteToPlcAsync(this AutoParameterModel autoParameterModel)
        {
            var tasks = new List<Task<bool>>();
            foreach (var item in autoParameterModel.AutoParameterContents)
            {
                var test = Task.Run<bool>(() =>
                {
                    foreach (var item1 in item.Instance.Content)
                    {
                        if (item1.Desc.Contains("#"))
                        {
                            continue;
                        }

                        var result = item1.Write(ConfigPlcs.Instance[item1.PlcName], item1.getFullPosition);
                        if (result.IsSuccess)
                        {
                            continue;
                        }

                        Growl.WarningGlobal($"WRITE-{item1.getFullPosition}:{result.Message}");
                        return false;
                    }

                    return true;
                });
                tasks.Add(test);
            }

            var res = await Task.WhenAll(tasks);
            return res.All(item => item);
        }


        public static async Task<bool> ReadToPlcAsync(this AutoParameterModel autoParameterModel)
        {
            var tasks = new List<Task<bool>>();
            foreach (var item in autoParameterModel.AutoParameterContents)
            {
                var test = Task.Run<bool>(() =>
                {
                    foreach (var item1 in item.Instance.Content)
                    {
                        if (item1.Desc.Contains("#"))
                        {
                            continue;
                        }

                        var result = item1.Read(ConfigPlcs.Instance[item1.PlcName], item1.getFullPosition);
                        if (result.IsSuccess)
                        {
                            continue;
                        }

                        Growl.WarningGlobal($"READ-{item1.getFullPosition}:{result.Message}");
                        return false;
                    }

                    return true;
                });
                tasks.Add(test);
            }

            var res = await Task.WhenAll(tasks);
            return res.All(item => item);
        }
    }
}