namespace CMS.ReaderConfigLIbrary.Models {
    public class ConnectServerMessage {
        public DeviceCommunicationState DeviceCommunicationState { get; set; }
        public string Message { get; set; }

        public ConnectServerMessage(string message) {
            Message = message;
        }
        public static ConnectServerMessage Normal(DeviceCommunicationState deviceCommunicationState, string mes) {
            return new ConnectServerMessage(mes) { DeviceCommunicationState = deviceCommunicationState };
        }
    }
}