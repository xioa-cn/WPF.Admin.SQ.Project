using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using HandyControl.Controls;
using WPF.Admin.Models.Models;
using WPF.Admin.Models.Utils;
using WPF.Admin.Service.Logger;
using WPF.Admin.Service.Services;
using WPF.Admin.Themes.Helper;

namespace WPF.Admin.Themes.CodeAuth {
    public static class ApplicationCodeAuth {
        public static DateTime StartTime {
            get { return ApplicationAuthModule.StartTime; }
        } // 体验时间开始时间

        public static bool AuthTaskFlag {
            get => ApplicationAuthModule.AuthTaskFlag;
            set { ApplicationAuthModule.AuthTaskFlag = value; }
        } // 是否开启授权任务

        /// <summary>
        /// 授权标志 True为授权失败
        /// </summary>
        public static bool AuthFlag {
            get
            {
                if (AuthTaskFlag)
                    return (DateTime.Now - StartTime).TotalHours >= ApplicationAuthModule._Interval ? true : false;
                return false;
            }
        }

        public static void AuthTask() {
            AuthTaskFlag = true;
            Task.Run(() =>
            {
                while (true)
                {
                    if ((DateTime.Now - StartTime).TotalHours >= ApplicationAuthModule._Interval - 0.5)
                    {
                        Thread.Sleep(5000);
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            Growl.ErrorGlobal("体验时间限制到期！！ 请申请正式版软件 或 添加激活码");
                        });

                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.FileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory
                            , "WPFAdmin.auc.bat");
                        startInfo.WindowStyle = ProcessWindowStyle.Normal;
                        using Process process = new Process();
                        process.StartInfo = startInfo;
                        try
                        {
                            // 启动进程
                            process.Start();
                            process.WaitForExit();
                            int exitCode = process.ExitCode;
                            Console.WriteLine($"BAT文件执行完毕，退出代码: {exitCode}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"执行BAT文件时出错: {ex.Message}");
                        }
                        finally
                        {
                            Environment.Exit(0);
                        }
                    }

                    Thread.Sleep(6000000);
                }
            });
        }

        private static string StartupHelper(string authPathFile) {
            var authcode = System.IO.File.ReadAllText(
                authPathFile).Replace("\n", "").Replace("\r", "").Replace(" ", "");

           // var nor = TextCodeHelper.Encrypt();

            var dcode = TextCodeHelper.Decrypt(authcode);

            var auth = nasduabwduadawdb(dcode);

            if (auth == ApplicationConfigConst.Code)
            {
                AuthTaskFlag = false;
            }
            else
            {
                AuthTask();
            }

            return authcode;
        }

        public static void Startup() {
            string authcode = "string.Empty";
            try
            {
                if (ApplicationAuthModule.AuthOutTime < DateTime.Now)
                {
                    var authPathFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "authcode.txt");
                    if (AppSettings.Default is not null &&
                        AppSettings.Default.ApplicationVersions == ApplicationVersions.NoAuthorization)
                    {
                        authPathFile = TextCodeHelper.NoAuthorizationFile;
                        if (ApplicationAuthModule.DllCreateTime.IsOnTimeDay(DateTime.Now))
                        {
                            TextCodeHelper.NoAuthorizationRequired();
                        }
                    }

                    if (!System.IO.File.Exists(authPathFile))
                    {
                        var message = "授权文件不存在，请联系管理员获取授权码。";
                        Growl.ErrorGlobal(message);
                        throw new Exception(message);
                    }

                    if (ListenApplicationVersions.NormalVersion != ApplicationVersions.NoAuthorization)
                    {
                        authcode = StartupHelper(authPathFile);
                    }
                }
            }
            catch (Exception ex)
            {
                XLogGlobal.Logger?.LogError(ex.Message);

                AuthTask();
            }

            try
            {
                //ReflectionMethods.getMethod<HslCommunication.Authorization>("nasduabwduadawdb");

                Console.WriteLine("Starting method replacement...");


                var result =
                    HslCommunication.Authorization.SetAuthorizationCode("fe49cdb6-b388-4c05-9b66-0e3f1ad3627f");

#if DEBUG
                Console.WriteLine($"Method call result: {result}");

                if (result)
                {
                    Debug.WriteLine("GOD: Here you go!!!");
                }
#else
            if (result)
            {
                Debug.WriteLine("GOD: Here you go!!!");
            }


#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }


        public static string nasduabwduadawdb(string miawdiawduasdhasd) {
            StringBuilder stringBuilder = new StringBuilder();
            MD5 mD = MD5.Create();
            byte[] array = mD.ComputeHash(Encoding.Unicode.GetBytes(miawdiawduasdhasd));
            mD.Clear();
            foreach (var t in array)
            {
                stringBuilder.Append((255 - t).ToString("X2"));
            }

            return stringBuilder.ToString();
        }
    }
}