namespace CMS.ReaderConfigLIbrary.Models {
    public class ConnectServicesMessage {
        public string? Key { get; set; }
        public string? Message { get; set; }
    }

    public class ConnectServicesResult {
        public bool Result { get; set; }
        public IEnumerable<ConnectServicesMessage> ConnectServicesMessages { get; set; }
    }
}