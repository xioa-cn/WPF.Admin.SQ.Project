using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PressMachineMainModeules.Models;
using PressMachineMainModeules.Utils;
using WPF.Admin.Models;

namespace PressMachineMainModeules.ViewModels
{
    public partial class PreManualViewModel : BindableBase
    {

        #region 报警复位

        public ElfContent Invo报警复位True { get; set; } = new ElfContent
        {
            Content = "报警复位",
            Down = "M0.0-BOOL-TRUE",
        };
        public ElfContent Invo报警复位False { get; set; } = new ElfContent
        {
            Content = "报警复位",
            Down = "M0.0-BOOL-False",
        };

        #endregion

        #region 电缸一

        #region 电缸一启用屏蔽

        public ElfContent 电缸一启用屏蔽True { get; set; } = new ElfContent
        {
            Content = "电缸一启用屏蔽",
            Down = "MX1000.0-BOOL-TRUE",
        };
        public ElfContent 电缸一启用屏蔽False { get; set; } = new ElfContent
        {
            Content = "电缸一启用屏蔽",
            Down = "MX1000.0-BOOL-False",
        };

        #endregion

        #region 电缸一手自动切换

        public ElfContent 电缸一手自动切换True { get; set; } = new ElfContent
        {
            Content = "电缸一手自动切换",
            Down = "MX10.0-BOOL-TRUE",
        };
        public ElfContent 电缸一手自动切换False { get; set; } = new ElfContent
        {
            Content = "电缸一手自动切换",
            Down = "MX10.0-BOOL-False",
        };

        #endregion

        #region 电缸一手动正转1

        public ElfContent 电缸一手动正转1True { get; set; } = new ElfContent
        {
            Content = "电缸一手动正转1",
            Down = "MX10.1-BOOL-TRUE",
        };
        public ElfContent 电缸一手动正转1False { get; set; } = new ElfContent
        {
            Content = "电缸一手动正转1",
            Down = "MX10.1-BOOL-False",
        };

        #endregion

        #region 电缸一手动反转1

        public ElfContent 电缸一手动反转1True { get; set; } = new ElfContent
        {
            Content = "电缸一手动反转1",
            Down = "MX10.2-BOOL-TRUE",
        };
        public ElfContent 电缸一手动反转1False { get; set; } = new ElfContent
        {
            Content = "电缸一手动反转1",
            Down = "MX10.2-BOOL-False",
        };

        #endregion

        #region 电缸一自动启动1

        public ElfContent 电缸一自动启动1True { get; set; } = new ElfContent
        {
            Content = "电缸一自动启动1",
            Down = "MX10.3-BOOL-TRUE",
        };
        public ElfContent 电缸一自动启动1False { get; set; } = new ElfContent
        {
            Content = "电缸一自动启动1",
            Down = "MX10.3-BOOL-False",
        };

        #endregion

        #region 电缸一自动停止1

        public ElfContent 电缸一自动停止1True { get; set; } = new ElfContent
        {
            Content = "电缸一自动停止1",
            Down = "MX10.4-BOOL-TRUE",
        };
        public ElfContent 电缸一自动停止1False { get; set; } = new ElfContent
        {
            Content = "电缸一自动停止1",
            Down = "MX10.4-BOOL-False",
        };

        #endregion

        #region 电缸一压力测试1

        public ElfContent 电缸一压力测试1True { get; set; } = new ElfContent
        {
            Content = "电缸一压力测试1",
            Down = "MX10.5-BOOL-TRUE",
        };
        public ElfContent 电缸一压力测试1False { get; set; } = new ElfContent
        {
            Content = "电缸一压力测试1",
            Down = "MX10.5-BOOL-False",
        };

        #endregion

        #endregion

        #region 电缸二

        #region 电缸二启用屏蔽

        public ElfContent 电缸二启用屏蔽True { get; set; } = new ElfContent
        {
            Content = "电缸二启用屏蔽",
            Down = "MX1000.1-BOOL-TRUE",
        };
        public ElfContent 电缸二启用屏蔽False { get; set; } = new ElfContent
        {
            Content = "电缸二启用屏蔽",
            Down = "MX1000.1-BOOL-False",
        };

        #endregion

        #region 电缸二手自动切换

        public ElfContent 电缸二手自动切换True { get; set; } = new ElfContent
        {
            Content = "电缸二手自动切换",
            Down = "MX20.0-BOOL-TRUE",
        };
        public ElfContent 电缸二手自动切换False { get; set; } = new ElfContent
        {
            Content = "电缸二手自动切换",
            Down = "MX20.0-BOOL-False",
        };

        #endregion

        #region 电缸二手动正转1

        public ElfContent 电缸二手动正转1True { get; set; } = new ElfContent
        {
            Content = "电缸二手动正转1",
            Down = "MX20.1-BOOL-TRUE",
        };
        public ElfContent 电缸二手动正转1False { get; set; } = new ElfContent
        {
            Content = "电缸二手动正转1",
            Down = "MX20.1-BOOL-False",
        };

        #endregion

        #region 电缸二手动反转1

        public ElfContent 电缸二手动反转1True { get; set; } = new ElfContent
        {
            Content = "电缸二手动反转1",
            Down = "MX20.2-BOOL-TRUE",
        };
        public ElfContent 电缸二手动反转1False { get; set; } = new ElfContent
        {
            Content = "电缸二手动反转1",
            Down = "MX20.2-BOOL-False",
        };

        #endregion

        #region 电缸二自动启动1

        public ElfContent 电缸二自动启动1True { get; set; } = new ElfContent
        {
            Content = "电缸二自动启动1",
            Down = "MX20.3-BOOL-TRUE",
        };
        public ElfContent 电缸二自动启动1False { get; set; } = new ElfContent
        {
            Content = "电缸二自动启动1",
            Down = "MX20.3-BOOL-False",
        };

        #endregion

        #region 电缸二自动停止1

        public ElfContent 电缸二自动停止1True { get; set; } = new ElfContent
        {
            Content = "电缸二自动停止1",
            Down = "MX20.4-BOOL-TRUE",
        };
        public ElfContent 电缸二自动停止1False { get; set; } = new ElfContent
        {
            Content = "电缸二自动停止1",
            Down = "MX20.4-BOOL-False",
        };

        #endregion

        #region 电缸二压力测试1

        public ElfContent 电缸二压力测试1True { get; set; } = new ElfContent
        {
            Content = "电缸二压力测试1",
            Down = "MX20.5-BOOL-TRUE",
        };
        public ElfContent 电缸二压力测试1False { get; set; } = new ElfContent
        {
            Content = "电缸二压力测试1",
            Down = "MX20.5-BOOL-False",
        };

        #endregion

        #endregion

        #region 电缸三

        #region 电缸三启用屏蔽

        public ElfContent 电缸三启用屏蔽True { get; set; } = new ElfContent
        {
            Content = "电缸三启用屏蔽",
            Down = "MX1000.2-BOOL-TRUE",
        };
        public ElfContent 电缸三启用屏蔽False { get; set; } = new ElfContent
        {
            Content = "电缸三启用屏蔽",
            Down = "MX1000.2-BOOL-False",
        };

        #endregion

        #region 电缸三手自动切换

        public ElfContent 电缸三手自动切换True { get; set; } = new ElfContent
        {
            Content = "电缸三手自动切换",
            Down = "MX30.0-BOOL-TRUE",
        };
        public ElfContent 电缸三手自动切换False { get; set; } = new ElfContent
        {
            Content = "电缸三手自动切换",
            Down = "MX30.0-BOOL-False",
        };

        #endregion

        #region 电缸三手动正转1

        public ElfContent 电缸三手动正转1True { get; set; } = new ElfContent
        {
            Content = "电缸三手动正转1",
            Down = "MX30.1-BOOL-TRUE",
        };
        public ElfContent 电缸三手动正转1False { get; set; } = new ElfContent
        {
            Content = "电缸三手动正转1",
            Down = "MX30.1-BOOL-False",
        };

        #endregion

        #region 电缸三手动反转1

        public ElfContent 电缸三手动反转1True { get; set; } = new ElfContent
        {
            Content = "电缸三手动反转1",
            Down = "MX30.2-BOOL-TRUE",
        };
        public ElfContent 电缸三手动反转1False { get; set; } = new ElfContent
        {
            Content = "电缸三手动反转1",
            Down = "MX30.2-BOOL-False",
        };

        #endregion

        #region 电缸三自动启动1

        public ElfContent 电缸三自动启动1True { get; set; } = new ElfContent
        {
            Content = "电缸三自动启动1",
            Down = "MX30.3-BOOL-TRUE",
        };
        public ElfContent 电缸三自动启动1False { get; set; } = new ElfContent
        {
            Content = "电缸三自动启动1",
            Down = "MX30.3-BOOL-False",
        };

        #endregion

        #region 电缸三自动停止1

        public ElfContent 电缸三自动停止1True { get; set; } = new ElfContent
        {
            Content = "电缸三自动停止1",
            Down = "MX30.4-BOOL-TRUE",
        };
        public ElfContent 电缸三自动停止1False { get; set; } = new ElfContent
        {
            Content = "电缸三自动停止1",
            Down = "MX30.4-BOOL-False",
        };

        #endregion

        #region 电缸三压力测试1

        public ElfContent 电缸三压力测试1True { get; set; } = new ElfContent
        {
            Content = "电缸三压力测试1",
            Down = "MX30.5-BOOL-TRUE",
        };
        public ElfContent 电缸三压力测试1False { get; set; } = new ElfContent
        {
            Content = "电缸三压力测试1",
            Down = "MX30.5-BOOL-False",
        };

        #endregion

        #endregion


        [ObservableProperty] private PressMachineStatus _pressMachine01Status = new();
        [ObservableProperty] private PressMachineStatus _pressMachine02Status = new();
        [ObservableProperty] private PressMachineStatus _pressMachine03Status = new();

        [ObservableProperty] private float _valueWrite = 0;

        [RelayCommand]
        private void ValueAdd()
        {
            this.ValueWrite++;
        }
        [RelayCommand]
        private void ValueCutDown()
        {
            this.ValueWrite--;
        }

    }
}
