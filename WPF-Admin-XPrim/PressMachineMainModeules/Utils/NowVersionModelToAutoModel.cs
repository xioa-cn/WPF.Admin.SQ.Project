using PressMachineMainModeules.Models;
using WPF.Admin.Models.Models;

namespace PressMachineMainModeules.Utils {
    public static class NowVersionModelToAutoModel {
        public static List<AutoMainPartialRolaCodeEntity> Converter(List<NowVersionCheckCodeModel> dto) {
            var autoMainPartialRolaCodeEntities = new List<AutoMainPartialRolaCodeEntity>();
            
            foreach (var item in dto)
            {
                if (item.CheckRolaValueSetps != null)
                {
                    foreach (var keyRola in item.CheckRolaValueSetps)
                    {
                        var value = new AutoMainPartialRolaCodeEntity {
                            AutoModeName = item.AutoModeName,
                            Step = keyRola.Step,
                            MainPartialRolas = keyRola.CheckRolaValues?.Select(e =>
                                new MainPartialRola {
                                    RolaString = e.RolaString,
                                    AutoRolaCodeType = e.AutoRolaCodeType
                                }).ToList() ?? [],
                        };
                        if (!string.IsNullOrEmpty(keyRola.MainCodeRola))
                        {
                            value.MainPartialRolas.Add(new MainPartialRola {
                                RolaString = keyRola.MainCodeRola,
                                AutoRolaCodeType = AutoRolaCodeType.Main,
                            });
                        }

                        autoMainPartialRolaCodeEntities.Add(value);
                    }
                }
            }

            return autoMainPartialRolaCodeEntities;
        }
    }
}