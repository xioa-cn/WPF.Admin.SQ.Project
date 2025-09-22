using CMS.ReaderConfigLIbrary.Models;
using CMS.ReaderConfigLIbrary.Services;

namespace CMS.ReaderConfigLIbrary.Utils {
    public static class PlcSettingToPlcsModel {
        // private static AnalysisPlcServices _analysisPlcServices;
        //
        // public static AnalysisPlcServices AnalysisPlcServices {
        //     get => _analysisPlcServices ??= new AnalysisPlcServices();
        // }

        public static bool TryConverterPlcs(this PlcViewSettingModel? plcViewSettingModel) {
            try
            {
                AnalysisPlcServices analysisPlcServices = new AnalysisPlcServices();
                if (plcViewSettingModel is null) return false;


                if (plcViewSettingModel.Contents == null)
                {
                    return true;
                }

                foreach (var plcViewSettingContent in plcViewSettingModel.Contents)
                {
                    NormalPlcs.DeviceCommunications.TryAdd(plcViewSettingContent.Key,
                        analysisPlcServices.AnalysisLocalPlcServices(plcViewSettingContent));
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}