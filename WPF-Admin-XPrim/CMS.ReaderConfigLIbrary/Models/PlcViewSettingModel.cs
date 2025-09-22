using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using WPF.Admin.Models;

namespace CMS.ReaderConfigLIbrary.Models {
    public enum PlcType {
        Melsec_MC_Binary,
        Siemens_S7_S1200,
        Siemens_S7_S1500,
        Modbus_TCP,
        Modbus_RTU,
        Inovance_TcpNet,
        Inovance_TcpNet_AM,
        Omron_FinsTcp,
    }

    public partial class PlcViewSettingContent : BindableBase {
        [ObservableProperty] private string _key;
        [ObservableProperty] private string _ip;
        [ObservableProperty] private string _desc;
        [ObservableProperty] private PlcType _type;

        // PLC类型相关配置
        [ObservableProperty] private int _port = 502; // 端口号，默认502
        [ObservableProperty] private byte _station = 1; // 站号，默认1
        [ObservableProperty] private int _connectTimeOut = 2000; // 连接超时时间(ms)
        [ObservableProperty] private int _receiveTimeOut = 5000; // 接收超时时间(ms)
        [ObservableProperty] private bool _dataFormat = true; // 大小端设置，true为大端
        [ObservableProperty] private string _serialPort = "COM1"; // 串口号
        [ObservableProperty] private int _baudRate = 9600; // 波特率
        [ObservableProperty] private int _dataBits = 8; // 数据位
        [ObservableProperty] private int _stopBits = 1; // 停止位
        [ObservableProperty] private int _parity = 0; // 校验位，0-None, 1-Odd, 2-Even
        [ObservableProperty] private bool _havingHeartbeat; // 是否启用心跳
        [ObservableProperty] private int _heartbeatInterval = 500; // 心跳间隔(ms)
        [ObservableProperty] private string _heartbeatValue; // 心跳值
        [ObservableProperty] private string _heartbeatPoint; // 心跳地址
        [ObservableProperty] private HeartbeatType _heartbeatType = HeartbeatType.Short; // 心跳类型
    }

    public class PlcViewSettingModel {
        public ObservableCollection<PlcViewSettingContent>? Contents { get; set; }

        /// <summary>
        /// 校验PLC配置
        /// </summary>
        /// <param name="msg">结果信息</param>
        /// <returns></returns>
        public bool Verification(out string msg) {
            msg = string.Empty;
            if (Contents == null || Contents.Count == 0) return true;

            var duplicateKeys = Contents.GroupBy(x => x.Key)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateKeys.Count == 0)
            {
                return true;
            }

            msg = $"发现重复的Key值: {string.Join(", ", duplicateKeys)}";
            return false;
        }
    }
}