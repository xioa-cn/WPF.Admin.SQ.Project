namespace WPF.Admin.Models.Attributes {
    [AttributeUsage(AttributeTargets.Class)]
    public class ExternalGrpcAttribute(string rpcName) : Attribute {
        public string RpcName { get; set; } = rpcName;
    }
}