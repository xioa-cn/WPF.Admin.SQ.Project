using System.Collections.Concurrent;
using System.IO;
using System.Text.Json;
using CMS.ReaderConfigLIbrary.Models;
using CMS.ReaderConfigLIbrary.Services;
using CMS.ReaderConfigLIbrary.Utils;

namespace CMS.ReaderConfigLIbrary {
    public class NormalPlcs {
        private static string? _plcConfigPath;

        public static string PlcConfigPath {
            get
            {
                if (_plcConfigPath != null)
                {
                    return _plcConfigPath;
                }

                _plcConfigPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CMSConfig",
                    "plcSettings.json");
                if (!System.IO.File.Exists(_plcConfigPath))
                {
                    throw new FileNotFoundException(nameof(PlcConfigPath));
                }

                return _plcConfigPath;
            }
            set { _plcConfigPath ??= value; }
        }

        public static ConcurrentDictionary<string, LocalDeviceCommunication> DeviceCommunications { get; set; }

        static NormalPlcs() {
            DeviceCommunications = new ConcurrentDictionary<string, LocalDeviceCommunication>();
            var json = System.IO.File.ReadAllText(PlcConfigPath);
            var options = new JsonSerializerOptions {
                WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };
            var plcs = System.Text.Json.JsonSerializer.Deserialize<PlcViewSettingModel>(json, options);
            plcs.TryConverterPlcs();
        }

        private static NormalPlcs? _Plcs;

        public static NormalPlcs Instance {
            get { return _Plcs ??= new NormalPlcs(); }
        }

        private NormalPlcs() {
        }

        public LocalDeviceCommunication? this[string key] {
            get
            {
                if (DeviceCommunications.TryGetValue(key, out LocalDeviceCommunication? communication))
                {
                    return communication;
                }

                throw new KeyNotFoundException(nameof(NormalPlcs.DeviceCommunications));
            }
        }

        public ConnectServicesResult ConnectServer() {
            return ConnectServices.PlcsConnect();
        }

        public void Heartbeat() {
            foreach (KeyValuePair<string, LocalDeviceCommunication> item in DeviceCommunications)
            {
                item.Value.StartHeartbeat();
            }
        }
    }
}