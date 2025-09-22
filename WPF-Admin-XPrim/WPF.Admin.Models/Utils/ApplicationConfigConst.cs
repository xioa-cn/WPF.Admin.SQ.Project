using WPF.Admin.Models.Models;

namespace WPF.Admin.Models.Utils {
    public class ApplicationConfigConst {
        public static ApplicationGlobalName InstanceApplicationGlobalName { get; set; } = new ApplicationGlobalName() {
            AppBaseOnName = "XioaSystem",
            AllStringName = "Xioa.WPF.Admin"
        };

        public static string SettingJsonFileName { get; } = "appConfigSettings.json";

        public static string BaseConfigName {
            get { return "AppBaseConfig.json"; }
        }

        public static string AssetsLogPath {
            get { return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "logo"); }
        }

        public static string BaseConfigFile {
            get =>
                System.IO.Path.Combine(AssetsLogPath, BaseConfigName);
        }

        public static string SettingJsonFile { get; } =
            System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SettingJsonFileName);

        public static string NoAuthorizationFileName { get; } = "NoAuthorizationRequired.dll";

        private static string? _pressMachineConfigExcel;

        public static string PressMachineConfigExcel {
            get
            {
                if (_pressMachineConfigExcel is not null) return _pressMachineConfigExcel;
                var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IOConfigs");
                if (System.IO.Directory.Exists(path) == false)
                {
                    System.IO.Directory.CreateDirectory(path);
                }

                return _pressMachineConfigExcel ??= System.IO.Path.Combine(path, "PressMachine.xlsx");
            }
        }

        public static string NoAuthorizationFile { get; } =
            System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, NoAuthorizationFileName);


        public static readonly string Code = "DD7B309FD1059F2E471CF06BA8F5F37E";

        public static readonly string Pwd = "19491001";

        public static readonly string _nnnnnnnnnnnnnn = "fe49cdb6-b388-4c05-9b66-0e3f1ad3627f";

        public static readonly string GarnetName = "GarnetServer.exe";

        private static string? _garnetDir;

        public static string GarnetDir {
            get
            {
                if (_garnetDir is not null) return _garnetDir;
                var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
                if (System.IO.Directory.Exists(path) == false)
                {
                    System.IO.Directory.CreateDirectory(path);
                }

                return _garnetDir ??= path;
            }
        }

        public static string GarnetPath {
            get { return System.IO.Path.Combine(GarnetDir, "garnet", GarnetName); }
        }

        public static string GarnetConnectString {
            get
            {
                if (AppSettings.Default is null)
                {
                    return "localhost:9999";
                }

                return "localhost:" + AppSettings.Default?.GarnetPort;
            }
        }

        public static string AutoCodePath {
            get
            {
                var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "codeConfig");
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "codeConfig");
            }
        }
        
        public static string AutoCheckCodeWorkwearFilePath {
            get
            {
                return System.IO.Path.Combine(AutoCodePath,"AutoCheckCodeWorkwear.json");
            }
        }
        
        public static string AutoWorkwearBindingParametersFilePath {
            get
            {
                return System.IO.Path.Combine(AutoCodePath,"WorkwearBindingParameters.json");
            }
        }

        public static string AutoModeJoinChar {
            get
            {
                return "-*-";
            }
        }
    }
}