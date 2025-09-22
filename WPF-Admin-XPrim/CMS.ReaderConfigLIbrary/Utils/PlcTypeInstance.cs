using CMS.ReaderConfigLIbrary.Models;
using HslCommunication.ModBus;
using HslCommunication.Profinet.Inovance;
using HslCommunication.Profinet.Melsec;
using HslCommunication.Profinet.Omron;
using HslCommunication.Profinet.Siemens;

namespace CMS.ReaderConfigLIbrary.Utils {
    public static class PlcTypeInstance {
        public static MelsecMcNet GetPlcMelsecMcNetInstance(this PlcViewSettingContent plcViewSettingContent) {
            HslCommunication.Profinet.Melsec.MelsecMcNet plc = new HslCommunication.Profinet.Melsec.MelsecMcNet();
            plc.NetworkNumber = 0;
            plc.NetworkStationNumber = 0;
            plc.TargetIOStation = 1023;
            plc.EnableWriteBitToWordRegister = false;
            plc.ByteTransform.IsStringReverseByteWord = false;
            plc.CommunicationPipe =
                new HslCommunication.Core.Pipe.PipeTcpNet(plcViewSettingContent.Ip, plcViewSettingContent.Port) {
                    ConnectTimeOut = 5000,
                    ReceiveTimeOut = 10000,
                };
            return plc;
        }

        public static SiemensS7Net GetPlcSiemensS7Instance(this PlcViewSettingContent plcViewSettingContent,
            SiemensPLCS siemensPlcs) {
            HslCommunication.Profinet.Siemens.SiemensS7Net plc =
                new HslCommunication.Profinet.Siemens.SiemensS7Net(siemensPlcs);
            plc.Rack = 0;
            plc.Slot = 0;
            plc.CommunicationPipe =
                new HslCommunication.Core.Pipe.PipeTcpNet(plcViewSettingContent.Ip, plcViewSettingContent.Port) {
                    ConnectTimeOut = 5000,
                    ReceiveTimeOut = 10000,
                };
            return plc;
        }

        public static ModbusTcpNet GetPlcModbusTcpInstance(this PlcViewSettingContent plcViewSettingContent) {
            HslCommunication.ModBus.ModbusTcpNet modbus = new HslCommunication.ModBus.ModbusTcpNet();
            modbus.Station = 1;
            modbus.AddressStartWithZero = true;
            modbus.IsCheckMessageId = true;
            modbus.IsStringReverse = false;
            modbus.DataFormat = HslCommunication.Core.DataFormat.CDAB;
            modbus.BroadcastStation = -1;
            modbus.CommunicationPipe =
                new HslCommunication.Core.Pipe.PipeTcpNet(plcViewSettingContent.Ip, plcViewSettingContent.Port) {
                    ConnectTimeOut = 5000,
                    ReceiveTimeOut = 10000,
                };
            return modbus;
        }

        public static ModbusRtu GetPlcModbusRtuInstance(this PlcViewSettingContent plcViewSettingContent) {
            HslCommunication.ModBus.ModbusRtu modbus = new HslCommunication.ModBus.ModbusRtu();
            modbus.AddressStartWithZero = true;
            modbus.IsStringReverse = false;
            modbus.DataFormat = HslCommunication.Core.DataFormat.CDAB;
            modbus.Station = 1;
            modbus.Crc16CheckEnable = true;
            modbus.IsClearCacheBeforeRead = false;
            modbus.StationCheckMacth = true;
            modbus.BroadcastStation = -1;
            HslCommunication.Core.Pipe.PipeSerialPort pipe = new HslCommunication.Core.Pipe.PipeSerialPort();

            var connectString =
                $"{plcViewSettingContent.SerialPort}-{plcViewSettingContent.BaudRate}-{plcViewSettingContent.DataBits}-{plcViewSettingContent.Parity}-{plcViewSettingContent.StopBits}";

            pipe.SerialPortInni(connectString);
            pipe.RtsEnable = false;
            pipe.DtrEnable = false;
            pipe.SleepTime = 20;
            modbus.CommunicationPipe = pipe;

            return modbus;
        }

        public static InovanceTcpNet GetPlcInovanceTcpNetInstance(this PlcViewSettingContent plcViewSettingContent) {
            HslCommunication.Profinet.Inovance.InovanceTcpNet plc =
                new HslCommunication.Profinet.Inovance.InovanceTcpNet();
            plc.Station = 1;
            plc.AddressStartWithZero = true;
            plc.IsStringReverse = false;
            plc.Series = HslCommunication.Profinet.Inovance.InovanceSeries.H5U;
            plc.DataFormat = HslCommunication.Core.DataFormat.CDAB;
            plc.CommunicationPipe =
                new HslCommunication.Core.Pipe.PipeTcpNet(plcViewSettingContent.Ip, plcViewSettingContent.Port) {
                    ConnectTimeOut = 5000,
                    ReceiveTimeOut = 10000,
                };
            return plc;
        }
        
        public static InovanceTcpNet GetPlcInovanceTcpNetAMInstance(this PlcViewSettingContent plcViewSettingContent) {
            HslCommunication.Profinet.Inovance.InovanceTcpNet plc =
                new HslCommunication.Profinet.Inovance.InovanceTcpNet();
            plc.Station = 1;
            plc.AddressStartWithZero = true;
            plc.IsStringReverse = false;
            plc.Series = HslCommunication.Profinet.Inovance.InovanceSeries.AM;
            plc.DataFormat = HslCommunication.Core.DataFormat.CDAB;
            plc.CommunicationPipe =
                new HslCommunication.Core.Pipe.PipeTcpNet(plcViewSettingContent.Ip, plcViewSettingContent.Port) {
                    ConnectTimeOut = 5000,
                    ReceiveTimeOut = 10000,
                };
            return plc;
        }

        public static OmronFinsNet GetPlcOmronFinsTcpInstance(this PlcViewSettingContent plcViewSettingContent) {
            HslCommunication.Profinet.Omron.OmronFinsNet plc = new HslCommunication.Profinet.Omron.OmronFinsNet();
            plc.PlcType = HslCommunication.Profinet.Omron.OmronPlcType.CSCJ;
            plc.DA2 = 0;
            plc.ReceiveUntilEmpty = false;
            plc.ByteTransform.DataFormat = HslCommunication.Core.DataFormat.CDAB;
            plc.ByteTransform.IsStringReverseByteWord = true;
            plc.CommunicationPipe =
                new HslCommunication.Core.Pipe.PipeTcpNet(plcViewSettingContent.Ip, plcViewSettingContent.Port) {
                    ConnectTimeOut = 5000, 
                    ReceiveTimeOut = 10000, 
                };
            return plc;
        }
    }
}