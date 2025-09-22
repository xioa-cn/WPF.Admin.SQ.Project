using HslCommunication.Profinet.Inovance;
using HslCommunication.Profinet.Siemens;
using System.Net.NetworkInformation;
using WPF.Admin.Models.Models;
using WPF.Admin.Service.Logger;

namespace PressMachineMainModeules.Config
{
    public class PlcConnect
    {
        private static InovanceTcpNet? _Plc;
        public static InovanceTcpNet? Plc
        {
            get
            {
                if (ApplicationAuthTaskFactory.AuthFlag)
                    return null;
                return _Plc;
            }
            set { _Plc = value; }
        }

        private static SiemensS7Net? _PlcS701;
        private static SiemensS7Net? _PlcS702;
        private static SiemensS7Net? _PlcS703;


        public static SiemensS7Net? PlcS701
        {
            get
            {
                if (ApplicationAuthTaskFactory.AuthFlag)
                    return null;
                return _PlcS701;
            }
            set { _PlcS701 = value; }
        } //西门子PLC连接对象
        public static SiemensS7Net? PlcS702
        {
            get
            {
                if (ApplicationAuthTaskFactory.AuthFlag)
                    return null;
                return _PlcS702;
            }
            set { _PlcS702 = value; }
        } //西门子PLC连接对象
        public static SiemensS7Net? PlcS703
        {
            get
            {
                if (ApplicationAuthTaskFactory.AuthFlag)
                    return null;
                return _PlcS703;
            }
            set { _PlcS703 = value; }
        } //西门子PLC连接对象

        public static bool GoOn
        {
            get => GoOnS7_01 && GoOnS7_02 && GoOnS7_03;
            set
            {
                
            }
        } // 连接成功
        public static bool GoOnS7_01
        {
            get;
            set;
        }
        public static bool GoOnS7_02 { get; set; }
        public static bool GoOnS7_03 { get; set; }
        public static bool Used { get; set; } = true; //通讯启用
        public static bool LineFromPCToPLC()
        {
            if (Plc is not null)
            {
                Plc.Dispose();
                Plc = null;
            }
            Plc = new InovanceTcpNet()
            {
                IpAddress = Common.Ip,
                Port = Common.Port,
                ConnectTimeOut = 2000
            };
            Plc.Station = 1;
            Plc.AddressStartWithZero = true;
            Plc.IsStringReverse = false;
            Plc.Series = HslCommunication.Profinet.Inovance.InovanceSeries.AM;
            Plc.DataFormat = HslCommunication.Core.DataFormat.CDAB;
            Plc.ConnectClose();
            var ret = Plc.ConnectServer();
            GoOn = ret.IsSuccess;
            return ret.IsSuccess;
        }

        /// <summary>
        /// 创建临时连接
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static SiemensS7Net? CreatePlcConnect(string ip)
        {
            var plc = new SiemensS7Net(SiemensPLCS.S1200, ip)
            {
                ConnectTimeOut = 2000
            };
            plc.ConnectClose();
            var ret = plc.ConnectServer();
            if (ret.IsSuccess)
                return plc;
            return null;
        }


        public static string? Code01 { get; set; } = string.Empty;
        public static string? Code02 { get; set; } = string.Empty;
        public static string? Code03 { get; set; } = string.Empty;
        public static string? Code04 { get; set; } = string.Empty;

        public static bool LineFromPCToPLC_S7(int connectId = 1)
        {
            if (connectId < 1 || connectId > 3)
            {
                XLogGlobal.Logger?.LogError($"PLC连接ID错误：{connectId}");
                return false;
            }
            var plc_S7Net = connectId switch
            {
                1 => PlcS701,
                2 => PlcS702,
                3 => PlcS703,
                _ => null
            };

            plc_S7Net?.Dispose();

            var ip = connectId switch
            {
                1 => Common.Ip01,
                2 => Common.Ip02,
                3 => Common.Ip03,
                _ => Common.Ip01
            };
            plc_S7Net = new SiemensS7Net(SiemensPLCS.S1200, ip)
            {
                ConnectTimeOut = 2000,
                ReceiveTimeOut = 2000,
            };
            plc_S7Net.ConnectClose();
            var ret = plc_S7Net.ConnectServer();

            switch (connectId)
            {
                case 1:
                    GoOnS7_01 = ret.IsSuccess;
                    break;
                case 2:
                    GoOnS7_02 = ret.IsSuccess;
                    break;
                case 3:
                    GoOnS7_03 = ret.IsSuccess;
                    break;
            }
            if (!ret.IsSuccess)
            {
                XLogGlobal.Logger?.LogError($"PLC {ip}连接异常：{ret.Message}");
            }
            return ret.IsSuccess;
        }


        public static bool PingPlcIp(int connectId)
        {
            var ip = connectId switch
            {
                1 => Common.Ip01,
                2 => Common.Ip02,
                3 => Common.Ip03,
                _ => Common.Ip01
            };
            if (ip is null) return false;
            Ping pingSend = new Ping();
            PingReply reply = pingSend.Send(ip, 1000);
            if (reply.Status == IPStatus.Success)
                return true;
            return false;
        }
    }
}
