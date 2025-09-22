using CMS.ReaderConfigLIbrary.Models;

namespace CMS.ReaderConfigLIbrary.Services {
    public static class ConnectServices {
        /// <summary>
        /// 连接PLCs
        /// </summary>
        /// <returns></returns>
        public static ConnectServicesResult PlcsConnect() {
            var result = new List<ConnectServicesMessage>();
            var ret = new ConnectServicesResult {
                Result = true,
                ConnectServicesMessages = result
            };
            foreach (var localDeviceCommunication in NormalPlcs.DeviceCommunications)
            {
                var connectRet = localDeviceCommunication.Value.ConnectServer();
                if (connectRet.DeviceCommunicationState is not DeviceCommunicationState.NotConnect)
                {
                    continue;
                }

                result.Add(new ConnectServicesMessage
                    { Key = localDeviceCommunication.Key, Message = connectRet.Message });
                ret.Result = false;
            }
            
            return ret;
        }
    }
}