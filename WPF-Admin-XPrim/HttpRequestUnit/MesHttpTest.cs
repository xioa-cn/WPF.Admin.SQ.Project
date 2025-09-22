using WPF.Admin.Http.Request.Mes_Http;

namespace HttpRequestUnit;

public class MesHttpTest
{
    [Fact]
    public async void Test1()
    {
        try
        {
            MesHttp mes = new MesHttp("http://localhost:5066");
            var s = await mes.Axios.GetAsync("/weatherforecast");
            var result = await s.Content.ReadAsStringAsync();
            
            var s1 = await mes.Axios.PostAsync<object>("/postTest",new {});
           
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
}