using System.Diagnostics;
using WPF.Admin.Models.Utils;
using WPF.Admin.Service.Services.Garnets;

namespace AutoPreTest {
    public class GarnetCreateTest {
        [Fact]
        public void Test() {
            GarnetService.StartupCreateExeGarnet();
        }

        [Fact]
        public void Test2() {
            GarnetService.StartupGarnet(9999);
        }

        [Fact]
        public void Test3() {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = ApplicationConfigConst.GarnetPath;
            startInfo.Arguments =
                $"--port {9999} --logdir cache -c cache/checkpoints --storage-tier --aof --aof-commit-freq 0 --recover --compaction-freq 60 --compaction-type Scan";
            var process = new Process();
            process.StartInfo = startInfo;


            process.Start();

            process.Kill();
            int exitCode = process.ExitCode;
        }
    }
}