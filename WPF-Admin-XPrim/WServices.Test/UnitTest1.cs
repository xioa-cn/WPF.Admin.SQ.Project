using WPF.Admin.Models.Models;
using WPF.Admin.Service.Logger;

namespace WServices.Test;

public class UnitTest1 {
    [Fact]
    public void Test1() {
        AppSettings.Default = new AppSettings();
        AppSettings.Default.OpenDblog = true;
        var logger = XLogGlobal.Logger;
        logger.LogError("TestLogger");
    }
}