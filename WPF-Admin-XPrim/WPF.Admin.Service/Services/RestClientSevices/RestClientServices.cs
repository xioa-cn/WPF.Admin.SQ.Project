using RestSharp;
using WPF.Admin.Models.Models;

namespace WPF.Admin.Service.Services.RestClientSevices {
    public class RestClientServices : IDisposable, IAsyncDisposable {
        private readonly RestClient _client;
        private readonly RestRequest _request;

        public RestClientServices(string mesRequestUrl, Dictionary<string, string>? headers = null) {
            _client = new RestClient(mesRequestUrl);
            _request = new RestRequest {
                Timeout = TimeSpan.FromSeconds(5)
            };

            if (headers is null)
            {
                return;
            }

            foreach (KeyValuePair<string, string> item in headers)
            {
                _request.AddHeader(item.Key, item.Value);
            }
        }

        public async Task<RestResponse> SendRequestAsync(MesRequestMethod mesRequest, string? mesData = null) {
            if (mesRequest == MesRequestMethod.Get)
            {
                _request.Method = Method.Get;
            }
            else
            {
                _request.Method = Method.Post;
                var body = mesData ?? string.Empty;
                _request.AddParameter("application/json", body, ParameterType.RequestBody);
            }

            return await _client.ExecuteAsync(_request);
        }

        public void Dispose() {
            _client.Dispose();
        }

        public async ValueTask DisposeAsync() {
            if (_client is IAsyncDisposable clientAsyncDisposable)
            {
                await clientAsyncDisposable.DisposeAsync();
            }
            else
            {
                _client.Dispose();
            }
        }
    }
}