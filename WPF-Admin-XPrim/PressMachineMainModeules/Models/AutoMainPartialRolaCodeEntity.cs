using WPF.Admin.Models.Models;

namespace PressMachineMainModeules.Models {

    public class MainPartialRola {
        public string RolaString { get; set; }
        public AutoRolaCodeType AutoRolaCodeType { get; set; }
    }
    
    public class AutoMainPartialRolaCodeEntity {
        public string AutoModeName { get; set; }
        public int Step { get; set; }
        public List<MainPartialRola> MainPartialRolas { get; set; }
    }
}