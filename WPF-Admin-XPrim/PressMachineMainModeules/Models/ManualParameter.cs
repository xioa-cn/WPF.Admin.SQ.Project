using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF.Admin.Models;

namespace PressMachineMainModeules.Models
{
    public partial class ManualParameter : BindableBase
    {
        /// <summary>
        /// 写入手动保护压力值
        /// </summary>
        [ObservableProperty] private float _WriteManualProtectionPressure;
        /// <summary>
        /// 写入手动速度值
        /// </summary>
        [ObservableProperty] private float _WriteManualSPeed;
        /// <summary>
        /// X轴手动速度
        /// </summary>
        [ObservableProperty] private float _WriteManualXSPeed;
    }
}
