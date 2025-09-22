using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using PressMachineMainModeules.Config;
using WPF.Admin.Models;

namespace PressMachineMainModeules.Models {
    public partial class AutoPartialCodeContent : BindableBase {
        [ObservableProperty] private string _autoMode;
        [ObservableProperty] private Dictionary<int, string> _mainCode;
        [ObservableProperty] private Dictionary<int, ObservableCollection<string>> _partialCodes;
    }


    /// <summary>
    /// 零件码组
    /// </summary>
    public partial class AutoCodeModel : BindableBase {
        [ObservableProperty] private ObservableCollection<AutoPartialCodeContent> _partialCodes;

        public bool CheckResult(string autoMode, int step) {
            return this.PartialCodes.FirstOrDefault(x => x.AutoMode == autoMode)
                .PartialCodes[step].Count(string.IsNullOrEmpty) == 0;
        }

        public AutoCodeModel() {
            PartialCodes =
                new ObservableCollection<AutoPartialCodeContent>();

            foreach (var item in HomeManager.Instance.HomePositionModels)
            {
                var autoPartialCodeContent = new AutoPartialCodeContent() { AutoMode = item.Desc };
                autoPartialCodeContent.PartialCodes = new Dictionary<int, ObservableCollection<string>>();
                autoPartialCodeContent.MainCode = new Dictionary<int, string>();
                if (AutoCheckCodeModelManager.Instance.AutoCheckCodeModel.SelectStepCheckCodeOpen)
                {
                    foreach (var value in Enumerable.Range(1, item.StepSum + 1))
                    {
                        autoPartialCodeContent.PartialCodes.Add(value, new ObservableCollection<string>());
                        autoPartialCodeContent.MainCode.Add(value, "");
                    }
                }
                else
                {
                    autoPartialCodeContent.PartialCodes.Add(1, new ObservableCollection<string>());
                    autoPartialCodeContent.MainCode.Add(1, "");
                }


                PartialCodes.Add(autoPartialCodeContent);
            }
        }
    }
}