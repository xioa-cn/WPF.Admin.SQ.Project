using HslCommunication;
using WPF.Admin.Service.Logger;

namespace PressMachineMainModeules.Utils {
    public static class PlcHelper {
        public static void ResultIfErrorLogger(this OperateResult result) {
            if (result.IsSuccess)
            {
                return;
            }

            XLogGlobal.Logger?.LogError(result.Message);
        }
    }
}