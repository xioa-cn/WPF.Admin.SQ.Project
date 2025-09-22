using PressMachineMainModeules.Models;

namespace AutoPreTest {
    public class ExcelReaderAutoMesTest {
        [Fact]
        public async void Test() {
            var data = AutoMesPropertiesTest.TestModel();
            if (AutoMesConfigManager.Instance.AutoMesConfig.OpenBeforeChecked)
            {
                var result =
                    await AutoMesConfigManager.Instance.AutoMesConfig.BeforeAutoMesHttp?.RequestBodyBoolResult(data);
               Assert.True(result);
            }

            if (AutoMesConfigManager.Instance.AutoMesConfig.OpenAfterChecked)
            {
                if (AutoMesConfigManager.Instance.AutoMesConfig.AfterAutoMesMode == AutoMesAfterMode.Sql)
                {
                    var result =
                        await AutoMesConfigManager.Instance.AutoMesConfig.AfterAutoMesSql?.ExecuteSaveSql(data);
                }
                else if (AutoMesConfigManager.Instance.AutoMesConfig.AfterAutoMesMode == AutoMesAfterMode.Http)
                {
                    var result =
                        await AutoMesConfigManager.Instance.AutoMesConfig.AfterAutoMesHttp?.RequestHttp(data);
                }
            }
        }
    }
}