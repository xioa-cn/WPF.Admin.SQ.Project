using WPF.Admin.Models.Models;
using WPF.Admin.Models.Utils;

namespace WPF.Admin.Service.Utils {
    public static class AppBaseConfigHelper {
        public static void InitializedAppBaseConfig() {
            if (System.IO.File.Exists(ApplicationConfigConst.BaseConfigFile))
            {
                ApplicationConfigConst.InstanceApplicationGlobalName = 
                    System.Text.Json.JsonSerializer.Deserialize<ApplicationGlobalName>(System.IO.File.ReadAllText(ApplicationConfigConst.BaseConfigFile));
            }
        }
    }
}