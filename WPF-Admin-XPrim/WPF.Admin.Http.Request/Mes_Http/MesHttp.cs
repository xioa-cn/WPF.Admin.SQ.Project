using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using WPF.Admin.Service.Services.Login;
using Xioa.Admin.Request.Tools.NetAxios;

namespace WPF.Admin.Http.Request.Mes_Http {
    public class MesHttp {
        private readonly ApplicationAxios _axios = new ApplicationAxios();

        public IAxios Axios => _axios.Axios;

        public MesHttp(NAxiosConfig? nAxiosConfig, bool sslError = false) {
            SetAxiosConfig(nAxiosConfig, sslError);
        }

        public MesHttp(string url, bool sslError = false) {
            SetAxiosConfig(NormalConfig(url), sslError);
        }

        private void SetAxiosConfig(NAxiosConfig? nAxiosConfig, bool sslError) {
            _axios.IgnoreSslErrorsSslError = sslError;
            _axios._AxiosConfig = nAxiosConfig;
        }

        private NAxiosConfig NormalConfig(string url) {
            return new NAxiosConfig {
                BaseUrl = url,
                Timeout = 5000,
                Headers = {
                    ["accept"] = "*/*",
                },
                RetryCount = 3,
                RetryDelay = 3000,
                RetryCondition = (exception, retryCount) =>
                {
                    var res = exception switch {
                        HttpRequestException httpException =>
                            httpException.StatusCode >= System.Net.HttpStatusCode.InternalServerError ||
                            httpException.StatusCode == System.Net.HttpStatusCode.RequestTimeout ||
                            httpException.StatusCode == System.Net.HttpStatusCode.TooManyRequests,
                        TaskCanceledException => true,
                        SocketException => true,
                        IOException => true,
                        _ => false
                    };
                    return res;
                }
            };
        }
    }
}