using WPF.Admin.Models.Utils;

namespace WPF.Admin.Service.Utils {
    public static class PressMachineModuleHelper {
        public static bool PressMachineExcelConfigExist() {
            return System.IO.File.Exists(ApplicationConfigConst.PressMachineConfigExcel);
        }
    }
}