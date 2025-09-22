using CMS.ReaderConfigLIbrary.Utils;
using HslCommunication.Core.Device;

namespace CMS.ReaderConfigLIbrary.Models {
    public enum DeviceCommunicationState {
        None,
        Connect,
        NotConnect,
    }

    public class LocalDeviceCommunication {
        public DeviceCommunication? Plc { get; set; }

        public DeviceCommunicationState State { get; set; }

        private LocalDeviceCommunication(DeviceCommunication plc) {
            this.Plc = plc;
        }

        private readonly bool havingbeat;
        private readonly double beatInterval;
        private readonly string beatPoint;
        private readonly HeartbeatType _HeartbeatType;
        private readonly string beatValue;

        private LocalDeviceCommunication(DeviceCommunication plc, bool havingbeat, double beatInterval,
            string beatPoint, HeartbeatType type, string beatValue) {
            this.Plc = plc;
            this.beatPoint = beatPoint;
            this.beatValue = beatValue;
            this._HeartbeatType = type;
            this.havingbeat = havingbeat;
            this.beatInterval = beatInterval;
        }

        public static LocalDeviceCommunication Create(DeviceCommunication plc) {
            return new LocalDeviceCommunication(plc);
        }

        public static LocalDeviceCommunication Create(DeviceCommunication plc, bool havingbeat, double beatInterval,
            string beatPoint, HeartbeatType type, string beatValue) {
            return new LocalDeviceCommunication(plc, havingbeat, beatInterval, beatPoint, type, beatValue);
        }

        public ConnectServerMessage ConnectServer() {
            switch (this.Plc)
            {
                case null:
                    throw new ArgumentNullException(nameof(this.Plc));
                case DeviceTcpNet tcp: {
                    tcp.ConnectClose();
                    var ret = tcp.ConnectServer();
                    State = ret.IsSuccess.ToDeviceCommunicationState();
                    return ConnectServerMessage.Normal(State, ret.Message);
                }
                case DeviceSerialPort sp: {
                    sp.Close();
                    var ret = sp.Open();
                    State = ret.IsSuccess.ToDeviceCommunicationState();
                    return ConnectServerMessage.Normal(State, ret.Message);
                }
                default:
                    throw new AggregateException(this.Plc.GetType().FullName);
            }
        }

        private string? heartbeatPoint;
        private bool heartbeatEnable;
        private object[] heartbeatValue = new object[2];
        private PeriodicTimer? heartbeatTimer;

        private void Heartbeat(Type t) {
            if (this.Plc is null)
                throw new ArgumentNullException(nameof(this.Plc));
            if (string.IsNullOrEmpty(heartbeatPoint))
                throw new ArgumentNullException(nameof(heartbeatPoint));
            var ret = this.Plc.WriteHelper(heartbeatPoint,
                (heartbeatEnable ? heartbeatValue[0] : heartbeatValue[1]), t);
            heartbeatEnable = !heartbeatEnable;
            this.State = ret.IsSuccess.ToDeviceCommunicationState();
        }

        private bool beatRealStartup;

        public void StartHeartbeat() {
            if (beatRealStartup)
            {
                return;
            }

            if (!this.havingbeat)
                return;

            if (string.IsNullOrEmpty(this.beatValue))
            {
                return;
            }

            if (string.IsNullOrEmpty(this.beatPoint))
            {
                return;
            }

            Type t = this._HeartbeatType.GetBeatType();
            var values = beatValue.Split('#').ToArray();

            StartHeartbeat(t, beatPoint, beatInterval, null, values);
        }

        public void StartHeartbeat(Type t, string address, double sleep = 500, CancellationToken? token = null,
            params object[] values) {
            if (this.heartbeatTimer is not null)
                return;
            if (values.Length != 2)
                throw new ArgumentException(nameof(values));
            heartbeatPoint = address;
            heartbeatValue[0] = values[0];
            heartbeatValue[1] = values[1];
            heartbeatTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(sleep));
            CancellationToken stoppingToken = token ?? HeartbeatCancellationTokenSource.Token;
            beatRealStartup = true;
            Task.Run(async () =>
            {
                try
                {
                    while (await heartbeatTimer.WaitForNextTickAsync(stoppingToken))
                        Heartbeat(t);
                }
                catch (OperationCanceledException ex)
                {
                    beatRealStartup = false;
                    heartbeatTimer?.Dispose();
                    heartbeatTimer = null;
                }
            });
        }

        public CancellationTokenSource HeartbeatCancellationTokenSource { get; set; }
            = new CancellationTokenSource();

        public void ResetNormalToken() {
            if (this.beatRealStartup)
            {
                throw new Exception("心跳启动时 无法重置令牌");
            }

            HeartbeatCancellationTokenSource = new CancellationTokenSource();
        }
    }
}