using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using WPF.Admin.Models;
using WPF.Admin.Models.Models;

namespace PressMachineMainModeules.Models {
    public partial class CheckRolaValueSetp : BindableBase {
        [ObservableProperty] private string autoModeName;
        public int Step { get; set; }
        [ObservableProperty] private string _tokenKey;
        [ObservableProperty] private bool _open;
        [ObservableProperty] private ObservableCollection<CheckRolaValue>? checkRolaValues;
        [ObservableProperty] private string mainCodeRola;
    }

    /// <summary>
    /// 目标版本条码检测模型
    /// </summary>
    public partial class NowVersionCheckCodeModel : BindableBase {
        [ObservableProperty] private string autoModeName;

        private int _stepSum;

        public int StepSum {
            get => _stepSum;
            set
            {
                if (CheckRolaValueSetps is null)
                {
                    if (AutoCheckCodeModelManager.Instance.AutoCheckCodeModel.SelectStepCheckCodeOpen)
                    {
                        CheckRolaValueSetps = new CheckRolaValueSetp[value];
                        Enumerable.Range(0, value).ToList().ForEach(i =>
                            CheckRolaValueSetps[i] = new CheckRolaValueSetp {
                                TokenKey = $"步序号-{i + 1}", Step = i + 1,
                                AutoModeName = this.AutoModeName
                            }
                        );
                    }
                    else
                    {
                        CheckRolaValueSetps = new CheckRolaValueSetp[1];
                        CheckRolaValueSetps[0] = new CheckRolaValueSetp
                            { TokenKey = $"步序号-1", Step = 1, AutoModeName = this.AutoModeName };
                    }
                }

                _stepSum = value;
            }
        }

        [ObservableProperty] private bool _openStep;
        [ObservableProperty] private CheckRolaValueSetp[]? _checkRolaValueSetps;
    }

    public class NowVersionCheckCodeManager {
        private ObservableCollection<NowVersionCheckCodeModel>? _nowVersionCheckCodeModels;

        public ObservableCollection<NowVersionCheckCodeModel> NowVersionCheckCodeModels {
            get { return _nowVersionCheckCodeModels ?? GetNowVersionCheckCodeModel(); }
        }

        public static NowVersionCheckCodeManager Instance { get; } = new NowVersionCheckCodeManager();

        public ObservableCollection<NowVersionCheckCodeModel> GetNowVersionCheckCodeModel() {
            var result = new ObservableCollection<NowVersionCheckCodeModel>();
            foreach (var item in HomeManager.Instance.HomePositionModels)
            {
                var value = new NowVersionCheckCodeModel {
                    AutoModeName = item.Desc,
                    OpenStep = AutoCheckCodeModelManager.Instance.AutoCheckCodeModel.SelectStepCheckCodeOpen,
                };
                value.StepSum = item.StepSum;
                result.Add(value);
            }


            return result;
        }
    }
}