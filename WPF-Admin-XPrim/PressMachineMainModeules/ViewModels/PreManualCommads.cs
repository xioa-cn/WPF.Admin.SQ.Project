using CommunityToolkit.Mvvm.Input;
using PressMachineMainModeules.Models;
using PressMachineMainModeules.Utils;
using WPF.Admin.Models;

namespace PressMachineMainModeules.ViewModels
{
    public partial class PreManualViewModel : BindableBase
    {
        #region 自动启动

        public ElfContent 自动启动True { get; set; } = new ElfContent
        {
            Content = "自动启动",
            Down = "DB1.0.0-BOOL-TRUE",
        };
        public ElfContent 自动启动False { get; set; } = new ElfContent
        {
            Content = "自动启动",
            Down = "DB1.0.0-BOOL-False",
        };

        #endregion

        #region 自动停止
        
        public ElfContent 自动停止True { get; set; } = new ElfContent
        {
            Content = "自动停止",
            Down = "DB1.0.1-BOOL-TRUE",
        };
        public ElfContent 自动停止False { get; set; } = new ElfContent
        {
            Content = "自动停止",
            Down = "DB1.0.1-BOOL-False",
        };

        #endregion

        #region 报警复位

        public ElfContent 报警复位True { get; set; } = new ElfContent
        {
            Content = "报警复位",
            Down = "DB1.0.2-BOOL-TRUE",
        };
        public ElfContent 报警复位False { get; set; } = new ElfContent
        {
            Content = "报警复位",
            Down = "DB1.0.2-BOOL-False",
        };

        #endregion

        #region 总原点回归

        public ElfContent 总原点回归True { get; set; } = new ElfContent
        {
            Content = "总原点回归",
            Down = "DB1.0.3-BOOL-TRUE",
        };
        public ElfContent 总原点回归False { get; set; } = new ElfContent
        {
            Content = "总原点回归",
            Down = "DB1.0.3-BOOL-False",
        };

        #endregion

        #region 压机原点回归

        public ElfContent 压机原点回归True { get; set; } = new ElfContent
        {
            Content = "压机原点回归",
            Down = "DB1.0.4-BOOL-TRUE",
        };
        public ElfContent 压机原点回归False { get; set; } = new ElfContent
        {
            Content = "压机原点回归",
            Down = "DB1.0.4-BOOL-False",
        };

        #endregion

        #region X轴原点回归

        public ElfContent X轴原点回归True { get; set; } = new ElfContent
        {
            Content = "X轴原点回归",
            Down = "DB1.0.5-BOOL-TRUE",
        };
        public ElfContent X轴原点回归False { get; set; } = new ElfContent
        {
            Content = "X轴原点回归",
            Down = "DB1.0.5-BOOL-False",
        };

        #endregion

        #region 高低速

        public ElfContent 高速 { get; set; } = new ElfContent
        {
            Content = "高速",
            Down = "DB1.0.6-BOOL-TRUE",
        };
        public ElfContent 低速 { get; set; } = new ElfContent
        {
            Content = "低速",
            Down = "DB1.0.6-BOOL-False",
        };

        #endregion

        #region 压机点动上升

        public ElfContent 压机电动上升True { get; set; } = new ElfContent
        {
            Content = "压机电动上升",
            Down = "DB1.0.7-BOOL-TRUE",
        };
        public ElfContent 压机电动上升False { get; set; } = new ElfContent
        {
            Content = "压机电动上升",
            Down = "DB1.0.7-BOOL-False",
        };

        #endregion

        #region 压机点动下降

        public ElfContent 压机点动下降True { get; set; } = new ElfContent
        {
            Content = "压机点动下降",
            Down = "DB1.1.0-BOOL-TRUE",
        };
        public ElfContent 压机点动下降False { get; set; } = new ElfContent
        {
            Content = "压机点动下降",
            Down = "DB1.1.0-BOOL-False",
        };

        #endregion

        #region X轴点动单次

        public ElfContent X轴点动单次True { get; set; } = new ElfContent
        {
            Content = "X轴点动单次",
            Down = "DB1.1.1-BOOL-TRUE",
        };
        public ElfContent X轴点动单次False { get; set; } = new ElfContent
        {
            Content = "X轴点动单次",
            Down = "DB1.1.1-BOOL-False",
        };

        #endregion

        #region X轴点动左移

        public ElfContent X轴点动左移True { get; set; } = new ElfContent
        {
            Content = "X轴点动左移",
            Down = "DB1.1.2-BOOL-TRUE",
        };
        public ElfContent X轴点动左移False { get; set; } = new ElfContent
        {
            Content = "X轴点动左移",
            Down = "DB1.1.2-BOOL-False",
        };

        #endregion

        #region X轴点动右移

        public ElfContent X轴点动右移True { get; set; } = new ElfContent
        {
            Content = "X轴点动右移",
            Down = "DB1.1.3-BOOL-TRUE",
        };
        public ElfContent X轴点动右移False { get; set; } = new ElfContent
        {
            Content = "X轴点动右移",
            Down = "DB1.1.3-BOOL-False",
        };

        #endregion

        #region X轴待机位

        public ElfContent X轴待机位True { get; set; } = new ElfContent
        {
            Content = "X轴待机位",
            Down = "DB1.1.4-BOOL-TRUE",
        };
        public ElfContent X轴待机位False { get; set; } = new ElfContent
        {
            Content = "X轴待机位",
            Down = "DB1.1.4-BOOL-False",
        };

        #endregion

        #region X轴压装位一

        public ElfContent X轴压装位一True { get; set; } = new ElfContent
        {
            Content = "X轴压装位一",
            Down = "DB1.1.5-BOOL-TRUE",
        };
        public ElfContent X轴压装位一False { get; set; } = new ElfContent
        {
            Content = "X轴压装位一",
            Down = "DB1.1.5-BOOL-False",
        };

        #endregion

        #region X轴压装位二

        public ElfContent X轴压装位二True { get; set; } = new ElfContent
        {
            Content = "X轴压装位二",
            Down = "DB1.1.6-BOOL-TRUE",
        };
        public ElfContent X轴压装位二False { get; set; } = new ElfContent
        {
            Content = "X轴压装位二",
            Down = "DB1.1.6-BOOL-False",
        };

        #endregion

        #region X轴压装位三

        public ElfContent X轴压装位三True { get; set; } = new ElfContent
        {
            Content = "X轴压装位三",
            Down = "DB1.1.7-BOOL-TRUE",
        };
        public ElfContent X轴压装位三False { get; set; } = new ElfContent
        {
            Content = "X轴压装位三",
            Down = "DB1.1.7-BOOL-False",
        };

        #endregion

        #region X轴压装位四

        public ElfContent X轴压装位四True { get; set; } = new ElfContent
        {
            Content = "X轴压装位四",
            Down = "DB1.2.0-BOOL-TRUE",
        };
        public ElfContent X轴压装位四False { get; set; } = new ElfContent
        {
            Content = "X轴压装位四",
            Down = "DB1.2.0-BOOL-False",
        };

        #endregion

        #region 产量清零

        public ElfContent 产量清零True { get; set; } = new ElfContent
        {
            Content = "产量清零",
            Down = "DB1.2.1-BOOL-TRUE",
        };
        public ElfContent 产量清零False { get; set; } = new ElfContent
        {
            Content = "产量清零",
            Down = "DB1.2.1-BOOL-False",
        };

        #endregion


        #region 电缸移动

        public ElfContent 电缸下降True { get; set; } = new ElfContent
        {
            Content = "电缸下降True",
            Down = "DB1.1.0-BOOL-TRUE",
        };
        public ElfContent 电缸下降False { get; set; } = new ElfContent
        {
            Content = "电缸下降False",
            Down = "DB1.1.0-BOOL-False",
        };

        public ElfContent 电缸上升True { get; set; } = new ElfContent
        {
            Content = "电缸上升True",
            Down = "DB1.0.7-BOOL-TRUE",
        };
        public ElfContent 电缸上升False { get; set; } = new ElfContent
        {
            Content = "电缸上升False",
            Down = "DB1.0.7-BOOL-False",
        };

        #endregion


        [RelayCommand]
        private void StatusWrite(ElfContent elfContent)
        {
            WriteTools.Instance.Write(elfContent);
        }

    }
}
