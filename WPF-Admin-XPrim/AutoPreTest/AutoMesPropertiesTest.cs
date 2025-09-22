using PressMachineMainModeules.Models;

namespace AutoPreTest {
    public class AutoMesPropertiesTest {
        [Fact]
        public void TestGet() {
            var autoMesProperties = TestModel();
            var test = autoMesProperties["ResultString1"];
        }

        [Fact]
        public void TestAnalysisAutoMesSendStringModeTwo() {
            var autoMesProperties = TestModel();
            var wantString = autoMesProperties.Analysis("I Will Get Value %{Code}%");
            Assert.Equal("I Will Get Value Code", wantString);
            wantString = autoMesProperties.Analysis("I %{Code}% Get Value ");
            Assert.Equal("I Code Get Value ", wantString);
        }
        
        [Fact]
        public void TestAnalysisAutoMesSendStringModeOne() {
            var autoMesProperties = TestModel();
            var wantString = autoMesProperties.Analysis("%{Code}% Is Get Value");
            Assert.Equal("Code Is Get Value", wantString);
            wantString = autoMesProperties.Analysis("%{ResultInt}% Is Get Value");
            Assert.Equal("10 Is Get Value", wantString);
        }

        public static AutoMesProperties TestModel() {
            return new AutoMesProperties {
                Code = "Code",
                Recipe = "TESTRecipe",
                ResultString = "OK",
                ResultInt = 10,
                MaxPre = 20,
                FinalPre = 30,
                MaxPos = 40,
                StartTime = DateTime.Now,
                FinalTime = DateTime.Now.AddMinutes(10),
                Step = 50,
                AutoModeID = "12",
                FilePath = "Path"
            };
        }
    }
}