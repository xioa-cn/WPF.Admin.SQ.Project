using WPF.Admin.Models;
using WPF.Admin.Models.Models;

namespace PressMachineMainModeules.Models {
    public partial class AutoWorkwearRolaCodeEntity : BindableBase {
        public int Step { get; set; }
        public string RolaString { get; set; }
        public AutoRolaCodeType AutoRolaCodeType { get; set; }

        public AutoWorkwearRolaCodeEntity(int step) {
            this.Step = step;
        }
    }
}