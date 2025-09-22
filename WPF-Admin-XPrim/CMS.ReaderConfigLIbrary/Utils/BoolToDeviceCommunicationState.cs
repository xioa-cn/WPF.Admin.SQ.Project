using CMS.ReaderConfigLIbrary.Models;

namespace CMS.ReaderConfigLIbrary.Utils {
    public static class BoolToDeviceCommunicationState {
        public static DeviceCommunicationState ToDeviceCommunicationState(this bool value) {
            return value! ? DeviceCommunicationState.Connect : DeviceCommunicationState.NotConnect;
        }
    }
}