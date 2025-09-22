using WPF.Admin.Models.Models;
using WPF.Admin.Service.Services.RestClientSevices;
using WPF.Admin.Service.Utils;

namespace AutoPreTest {
    public class AutoMesHttpUploadTest {
        [Fact]
        public async void HttpTest() {
            var data = AutoMesPropertiesTest.TestModel();
            var url = "http://localhost:5066/api/MesHttp?Code=%{Code}%";
            url = data.Analysis(url);
            var method = MesRequestMethod.Get;
            var service = new RestClientServices(url);
            var result = await service.SendRequestAsync(method);
            Assert.NotNull(result.Content);
        }

        [Fact]
        public async void HttpTestIsSuccess() {
            var data = AutoMesPropertiesTest.TestModel();
            var url = "http://localhost:5066/api/MesHttp?Code=%{Code}%";
            url = data.Analysis(url);
            var method = MesRequestMethod.Get;
            var service = new RestClientServices(url);
            var result = await service.SendRequestAsync(method);
            var mesResult = AnalysisMesDataHelper.MesResponseIsSuccess(result.Content, "request:result-string-OK");
            Assert.True(mesResult);
        }

        [Fact]
        public async void HttpTest2() {
            var data = AutoMesPropertiesTest.TestModel();
            var httpbody = "{\"code\": \"%{ResultString}%\",\"recipe\": \"%{Recipe}%\"}";
            var url = "http://localhost:5066/api/MesHttp/post?code=%{Code}%";
            url = data.Analysis(url);
            httpbody = data.Analysis(httpbody);
            var method = MesRequestMethod.Post;
            var service = new RestClientServices(url);
            var result = await service.SendRequestAsync(method, httpbody);
            Assert.NotNull(result.Content);
        }
    }
}