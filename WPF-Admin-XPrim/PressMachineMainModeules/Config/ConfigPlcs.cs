using CMS.ReaderConfigLIbrary.Models;
using CMS.ReaderConfigLIbrary.Services;
using HslCommunication.Core.Device;
using PressMachineMainModeules.Utils;
using System.Collections.Concurrent;
using WPF.Admin.Models.Models;
using WPF.Admin.Models.Utils;

namespace PressMachineMainModeules.Config
{
    public class Plc
    {
        public LocalDeviceCommunication? Device { get; set; }
        public string Name { get; set; }
    }
    public class ConfigPlcs
    {
        public ConcurrentDictionary<string, Plc>? Plcs { get; set; }

        public DeviceCommunication this[string name]
        {
            get
            {
                if (ApplicationAuthTaskFactory.AuthFlag)
                {
                    return null;
                }

                if (string.IsNullOrEmpty(name))
                {
                    return null;
                }
                
                var find = Plcs?[name];
                if (find is null)
                {
                    return null;
                    throw new Exception($"PLC with name '{name}' not found.");
                }
                if (find.Device is null)
                {
                    throw new Exception($"PLC with name '{name}' has no device configured.");
                }
                return find.Device.Plc;
            }
        }
        private static string? _configPath;
        public static string ConfigPath
        {
            get
            {
                return _configPath ??= ApplicationConfigConst.PressMachineConfigExcel;
            }
        }
        public static ConfigPlcs Instance { get; } = new ConfigPlcs();


        public ConfigPlcs Initialized()
        {
            if (this.Plcs is not null)
            {
                return this;
            }
            this.Plcs = new ConcurrentDictionary<string, Plc>();
            var plcModels = ConfigPlcsExcelReader.ReadExcel(ConfigPath,"Plcs");
            AnalysisPlcServices analysisPlcServices = new AnalysisPlcServices();
            foreach (var item in plcModels)
            {
                var ldevice = analysisPlcServices.AnalysisLocalPlcServices(item);
                this.Plcs.TryAdd(item.Key, new Plc
                {
                    Device = ldevice,
                    Name = item.Key,
                });
            }
            return this;
        }

        public List<ConnectServerMessage> ConnectServer()
        {
            List<ConnectServerMessage> connectServerMessage = new List<ConnectServerMessage>();
            if (this.Plcs is not null)
            {
                foreach (var item in Plcs.Values)
                {
                    var result = item.Device?.ConnectServer();
                    if (result is not null && result.DeviceCommunicationState != DeviceCommunicationState.Connect)
                    {
                        connectServerMessage.Add(result);
                    }
                }
            }
            return connectServerMessage;
        }

        public void StartHeartbeat()
        {
            if (this.Plcs is not null)
                foreach (var plc in Plcs.Values)
                {
                    if (plc.Device is not null)
                    {
                        plc.Device.StartHeartbeat();
                    }
                }
        }
    }
}
