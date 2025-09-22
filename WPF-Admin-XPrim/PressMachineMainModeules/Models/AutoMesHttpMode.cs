using RestSharp;
using WPF.Admin.Models.Models;
using WPF.Admin.Service.Logger;
using WPF.Admin.Service.Services.RestClientSevices;
using WPF.Admin.Service.Utils;

namespace PressMachineMainModeules.Models {
    public class AutoMesHttpMode {
        public string HttpMethod { get; set; }
        public string RequestUrl { get; set; }
        public string RequestBody { get; set; }

        private Dictionary<string, string>? _dictionaryHeader;

        public Dictionary<string, string> Headers {
            get { return _dictionaryHeader ??= GetHeadersDic(); }
        }

        private Dictionary<string, string> GetHeadersDic() {
            var dic = new Dictionary<string, string>();
            var headerlist = this.HeadersString.Split("#");
            foreach (var item in headerlist)
            {
                if (string.IsNullOrWhiteSpace(item)) continue;
                var header = item.Split(":");
                dic.Add(header[0].Trim(), header[1].Trim());
            }

            return dic;
        }

        public string HeadersString { get; set; }
        public string AnalysisString { get; set; }

        public MesRequestMethod MesRequestMethod {
            get
            {
                return HttpMethod.ToLower() switch {
                    "get" => MesRequestMethod.Get,
                    "post" => MesRequestMethod.Post,
                    _ => MesRequestMethod.Get
                };
            }
        }

        public async Task<RestResponse> RequestHttp(AutoMesProperties autoMesProperties) {
            var nUrl = autoMesProperties.Analysis(this.RequestUrl);
            var nBody = autoMesProperties.Analysis(this.RequestBody);
            XLogGlobal.Logger?.LogInfo($"执行MES请求{this.MesRequestMethod}:" + nUrl + "\n" + nBody);
            await using var service = new RestClientServices(nUrl, this.Headers);
            var result = await service.SendRequestAsync(
                this.MesRequestMethod,
                nBody);
            return result;
        }

        private bool GetResult(RestResponse result) {
            if (string.IsNullOrWhiteSpace(this.AnalysisString)) return true;
            return AnalysisMesDataHelper.MesResponseIsSuccess(result.Content, this.AnalysisString);
        }

        public async Task<bool> RequestBodyBoolResult(AutoMesProperties autoMesProperties) {
            var response = await this.RequestHttp(autoMesProperties);
            return this.GetResult(response);
        }
    }
}