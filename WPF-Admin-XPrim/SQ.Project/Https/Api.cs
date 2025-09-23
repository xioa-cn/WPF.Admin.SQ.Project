namespace SQ.Project.Https
{
    public class Api
    {
        public static string BasicAccount
        {
            get { return "xioa"; }
        }

        public static string BasicPassword
        {
            get { return "xioa"; }
        }

        public static string BaseUrl
        {
            get { return "http://localhost:8080/"; }
        }

        /// <summary>
        /// 批次扫码
        /// </summary>
        public static string TypeInBomPost
        {
            // RequestBody
            // {
            //     "plid": "string",
            //     "materialCode": "string"
            // }
            get { return BaseUrl + "api/TypeInOrderBom/TypeIn"; }
        }

        /// <summary>
        /// 前道检查
        /// </summary>
        public static string CheckCodePost
        {
            // RequestBody
            // {
            //     "plid": "string",
            //     "itm": "string",
            //     "wsid": "string"
            // }
            get { return BaseUrl + "api/CheckCode/Check"; }
        }

        /// <summary>
        /// 数据保存
        /// </summary>
        public static string SaveDataPost
        {
            // RequestBody
            // {
            //     "plid": "string",
            //     "wsid": "string",
            //     "result": 0,
            //     "itm": "string",
            //     "keyMaterial": "string",
            //     "staticData": {
            //         "startTime": "2025-09-23T01:20:15.959Z",
            //         "endTime": "2025-09-23T01:20:15.959Z"
            //     },
            //     "laserCode": "string",
            //     "data": "string",
            //     "packing": {
            //         "packingCode": "string",
            //         "tier": 0,
            //         "row": 0,
            //         "column": 0
            //     },
            //     "popOnline": "2025-09-23T01:20:15.959Z"
            // }
            get { return BaseUrl + "api/Save/SavePreviousInspection"; }
        }

        /// <summary>
        /// 数据下线
        /// </summary>
        public static string OfflinePost
        {
            // RequestBody
            // {
            //     "plid": "string",
            //     "itm": "string"
            // }
            get { return BaseUrl + "api/Save/Offline"; }
        }

        /// <summary>
        /// 设备状态采集
        /// </summary>
        public static string StationStatusPost
        {
            // RequestBody
            // {
            //     "wsid": "string",
            //     "plid": "string",
            //     "status": 0,
            //     "errorCode": "string",
            //     "msg": "string"
            // }
            get { return BaseUrl + "api/StationStatusCollection/Status"; }
        }
    }
}