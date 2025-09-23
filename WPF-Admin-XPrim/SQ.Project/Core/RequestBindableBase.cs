using System.Net;
using System.Text;
using RestSharp;
using SQ.Project.Https;
using WPF.Admin.Models;

namespace SQ.Project.Core
{
    public abstract class RequestBindableBase : BindableBase
    {
        private readonly RestClient _restClient;

        public RequestBindableBase()
        {
            _restClient = new RestClient();
            SetBasicAuth(Api.BasicAccount, Api.BasicPassword);
        }

        public string DateTimeNowStr
        {
            get { return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

        /// <summary>
        /// 发送GET请求并返回指定类型的结果
        /// </summary>
        /// <typeparam name="T">返回数据的类型</typeparam>
        /// <param name="url">请求的URL地址</param>
        /// <returns>请求返回的结果</returns>
        protected virtual async Task<T> GetAsync<T>(string url)
        {
            var request = new RestRequest(url, Method.Get);
            var response = await _restClient.ExecuteAsync<T>(request);

            if (!response.IsSuccessful)
            {
                HandleRequestError(response);
            }

            return response.Data;
        }

        /// <summary>
        /// 发送POST请求并返回指定类型的结果
        /// </summary>
        /// <typeparam name="T">返回数据的类型</typeparam>
        /// <param name="url">请求的URL地址</param>
        /// <param name="data">要发送的数据</param>
        /// <returns>请求返回的结果</returns>
        protected virtual async Task<T?> PostAsync<T>(string url, object data)
        {
            var request = new RestRequest(url, Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddBody(data); // 使用JSON格式发送数据

            var response = await _restClient.ExecuteAsync<T>(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                HandleRequestError(response);
            }

            return response.Data;
        }

        /// <summary>
        /// 设置基本认证信息
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        protected void SetBasicAuth(string username, string password)
        {
            // 生成基本认证的Base64编码字符串
            var base64Credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));

            // 设置默认请求头
            _restClient.AddDefaultHeader("Authorization", $"Basic {base64Credentials}");
        }

        /// <summary>
        /// 处理请求错误
        /// </summary>
        /// <param name="response">请求响应</param>
        protected virtual void HandleRequestError(RestResponse response)
        {
            // 可以根据需要重写此方法以自定义错误处理
            // throw new HttpRequestException($"请求失败: {response.StatusCode} - {response.ErrorMessage}");
        }
    }
}