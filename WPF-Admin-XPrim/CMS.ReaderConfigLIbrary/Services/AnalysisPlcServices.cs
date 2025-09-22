using CMS.ReaderConfigLIbrary.Models;
using CMS.ReaderConfigLIbrary.Utils;
using HslCommunication.Profinet.Siemens;

namespace CMS.ReaderConfigLIbrary.Services {
    public class AnalysisPlcServices {
        public AnalysisPlcServices() {
        }

        public LocalDeviceCommunication AnalysisLocalPlcServices(PlcViewSettingContent analysisPlc) {
            return analysisPlc.Type switch {
                PlcType.Melsec_MC_Binary => LocalDeviceCommunication.Create(analysisPlc.GetPlcMelsecMcNetInstance(),
                    analysisPlc.HavingHeartbeat, analysisPlc.HeartbeatInterval, analysisPlc.HeartbeatPoint,
                    analysisPlc.HeartbeatType, analysisPlc.HeartbeatValue),
                PlcType.Siemens_S7_S1500 => LocalDeviceCommunication.Create(
                    analysisPlc.GetPlcSiemensS7Instance(SiemensPLCS.S1500),
                    analysisPlc.HavingHeartbeat, analysisPlc.HeartbeatInterval, analysisPlc.HeartbeatPoint,
                    analysisPlc.HeartbeatType, analysisPlc.HeartbeatValue),
                PlcType.Siemens_S7_S1200 => LocalDeviceCommunication.Create(
                    analysisPlc.GetPlcSiemensS7Instance(SiemensPLCS.S1200),
                    analysisPlc.HavingHeartbeat, analysisPlc.HeartbeatInterval, analysisPlc.HeartbeatPoint,
                    analysisPlc.HeartbeatType, analysisPlc.HeartbeatValue),
                PlcType.Modbus_TCP => LocalDeviceCommunication.Create(analysisPlc.GetPlcModbusTcpInstance(),
                    analysisPlc.HavingHeartbeat, analysisPlc.HeartbeatInterval, analysisPlc.HeartbeatPoint,
                    analysisPlc.HeartbeatType, analysisPlc.HeartbeatValue),
                PlcType.Modbus_RTU => LocalDeviceCommunication.Create(analysisPlc.GetPlcModbusRtuInstance(),
                    analysisPlc.HavingHeartbeat, analysisPlc.HeartbeatInterval, analysisPlc.HeartbeatPoint,
                    analysisPlc.HeartbeatType, analysisPlc.HeartbeatValue),
                PlcType.Inovance_TcpNet => LocalDeviceCommunication.Create(analysisPlc.GetPlcInovanceTcpNetInstance(),
                    analysisPlc.HavingHeartbeat, analysisPlc.HeartbeatInterval, analysisPlc.HeartbeatPoint,
                    analysisPlc.HeartbeatType, analysisPlc.HeartbeatValue),
                PlcType.Inovance_TcpNet_AM => LocalDeviceCommunication.Create(analysisPlc.GetPlcInovanceTcpNetAMInstance(),
                    analysisPlc.HavingHeartbeat, analysisPlc.HeartbeatInterval, analysisPlc.HeartbeatPoint,
                    analysisPlc.HeartbeatType, analysisPlc.HeartbeatValue),
                PlcType.Omron_FinsTcp => LocalDeviceCommunication.Create(analysisPlc.GetPlcOmronFinsTcpInstance(),
                    analysisPlc.HavingHeartbeat, analysisPlc.HeartbeatInterval, analysisPlc.HeartbeatPoint,
                    analysisPlc.HeartbeatType, analysisPlc.HeartbeatValue),
                _ => throw new NotImplementedException()
            };
        }
    }
}