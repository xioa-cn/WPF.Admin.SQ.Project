using System.Collections.ObjectModel;
using System.IO;
using PressMachineMainModeules.Utils;
using PressMachineMainModeules.ViewModels;
using WPF.Admin.Models.Models;
using WPF.Admin.Models.Utils;
using WPF.Admin.Themes.Converter;

namespace PressMachineMainModeules.Models {
    public class AutoCheckCodeModelManager {
        private AutoCheckCodeModel? _autoCheckCodeModel;
        public static AutoCheckCodeParameterModel? AutoCheckCodeParameterModelInstance { get; set; }

        private List<AutoWorkwearRolaCodeEntity>? _autoRolaCodeEntities;

        public List<AutoWorkwearRolaCodeEntity> AutoWorkwearRolaCodeEntities {
            get
            {
                if (this._autoRolaCodeEntities is not null) return _autoRolaCodeEntities;
                if (!AutoCheckCodeModel.OpenCheckMainCode)
                {
                    return _autoRolaCodeEntities ??= new List<AutoWorkwearRolaCodeEntity>();
                }

                _autoRolaCodeEntities = new List<AutoWorkwearRolaCodeEntity>();

                if (System.IO.File.Exists(ApplicationConfigConst
                        .AutoWorkwearBindingParametersFilePath))
                {
                    var list = SerializeHelper.Deserialize<AutoWorkwearBindingParam[]>(
                        ApplicationConfigConst
                            .AutoWorkwearBindingParametersFilePath);

                    list.ToList().ForEach(item =>
                    {
                        _autoRolaCodeEntities.Add(new AutoWorkwearRolaCodeEntity(0) {
                            AutoRolaCodeType = AutoRolaCodeType.Workwear,
                            RolaString = item.WorkwearName + ApplicationConfigConst.AutoModeJoinChar +
                                         item.ParamerterName,
                        });
                    });
                }

                return _autoRolaCodeEntities;
            }
        }

        private List<AutoMainPartialRolaCodeEntity>? _autoMainPartialRolaCodeEntities;

        public List<AutoMainPartialRolaCodeEntity> AutoMainPartialRolaCodeEntities {
            get
            {
                if (this._autoMainPartialRolaCodeEntities is not null) return _autoMainPartialRolaCodeEntities;

                _autoMainPartialRolaCodeEntities = new List<AutoMainPartialRolaCodeEntity>();
                string filename = AutoCheckCodeContentViewModel.Dir +
                                  $"\\{AutoCheckCodeModelManager.HistoryAutoCheckCodeParameterName}.json";
                if (!System.IO.File.Exists(filename))
                {
                    return _autoMainPartialRolaCodeEntities;
                }

                var jsonData =
                    SerializeHelper.Deserialize<ObservableCollection<NowVersionCheckCodeModel>>(filename);

                _autoMainPartialRolaCodeEntities = NowVersionModelToAutoModel.Converter(jsonData.ToList());

                return _autoMainPartialRolaCodeEntities;
            }
        }

        public AutoCheckCodeModel AutoCheckCodeModel {
            get { return _autoCheckCodeModel ??= Create(); }
        }

        public static AutoCheckCodeModelManager Instance { get; } = new AutoCheckCodeModelManager();

        private AutoCheckCodeModel Create() {
            return CheckCodeExcelReader.ReadExcel();
        }

        public static string HistoryAutoCheckCodeParameterPathFile {
            get
            {
                return System.IO.Path.Combine(AutoCheckCodeContentViewModel.DirBase,
                    "HISTORY_AUTO_CHECK_CODE.json");
            }
        }

        public static string? HistoryAutoCheckCodeParameterName {
            get { return HistoryAutoCheckCodeParameter?.Name; }
        }

        public static HistoryAutoCheckCodeParameterNameModel? HistoryAutoCheckCodeParameter {
            get
            {
                if (System.IO.File.Exists(HistoryAutoCheckCodeParameterPathFile))
                {
                    return SerializeHelper.Deserialize<HistoryAutoCheckCodeParameterNameModel>(
                        HistoryAutoCheckCodeParameterPathFile);
                }

                return null;
            }
        }

        public static async Task SaveHistoryAutoCheckCodeParameterNameAsync(string name) {
            await SaveHistoryAutoCheckCodeParameterNameAsync(
                new HistoryAutoCheckCodeParameterNameModel() { Name = name, Time = DateTime.Now });
        }

        public static async Task SaveHistoryAutoCheckCodeParameterNameAsync(
            HistoryAutoCheckCodeParameterNameModel model) {
            if (File.Exists(HistoryAutoCheckCodeParameterPathFile))
            {
                File.Delete(HistoryAutoCheckCodeParameterPathFile);
            }

            await SerializeHelper.SerializeAsync(HistoryAutoCheckCodeParameterPathFile, model);
        }
    }
}