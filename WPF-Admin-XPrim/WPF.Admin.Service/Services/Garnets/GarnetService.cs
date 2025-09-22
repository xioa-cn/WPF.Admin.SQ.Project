using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using WPF.Admin.Models.Utils;
using WPF.Admin.Service.Logger;
using WPF.Admin.Service.Utils;

namespace WPF.Admin.Service.Services.Garnets {
    public class GarnetService {
        //  log目录         --logdir
        //  缓存目录         cache
        //  检查点目录      cache/checkpoints
        //  存储层目录      storage-tier
        //  appendonly      --aof
        //  aof提交频率     --aof-commit-freq
        //  恢复            --recover
        //  压缩频率        --compaction-freq
        //  压缩类型        --compaction-type
        //  scan
        //  --logdir cache -c cache/checkpoints --storage-tier --aof --aof-commit-freq 0 --recover --compaction-freq 60 --compaction-type Scan
        
        /**
         * 手动停止 garnet 服务
         * netstat -ano | findstr 9999 查询pid
         * taskkill /PID 1236 /T /F    杀死进程
         *
         *
         * 
         */
        
        
        
        
        
        public static void StartupCreateExeGarnet() {
            if (System.IO.File.Exists(ApplicationConfigConst.GarnetPath))
            {
                return;
            }

            string tempZipPath = Path.GetTempFileName();
            var garnetStream =
                ApplicationUtils.FindApplicationResourceStream("WPF.Admin.Service", "Resources/garnet.zip");
            using (FileStream fileStream = File.Create(tempZipPath))
            {
                garnetStream.CopyTo(fileStream);
            }

            Directory.CreateDirectory(ApplicationConfigConst.GarnetDir);
            ZipFile.ExtractToDirectory(tempZipPath, ApplicationConfigConst.GarnetDir);

            if (!File.Exists(tempZipPath))
            {
                return;
            }

            try
            {
                File.Delete(tempZipPath);
            }
            catch { }

        }

        private static Process? process;

        public static void StartupGarnet(int port) {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = ApplicationConfigConst.GarnetPath;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments =
                $"--port {port} --logdir cache -c cache/checkpoints --storage-tier --aof --aof-commit-freq 0 --recover --compaction-freq 60 --compaction-type Scan";
            process = new Process();
            
            process.StartInfo = startInfo;
            try
            {
                // 启动进程
                process.Start();
                process.WaitForExit();
                int exitCode = process.ExitCode;
            }
            catch (Exception ex)
            {
                XLogGlobal.Logger?.LogError("Garnet Server: " + ex.Message, ex);
            }
        }

        public static void ShutdownGarnet() {
            process?.Kill();
        }
    }
}