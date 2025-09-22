namespace PressMachineCMS.Config {
    public class CMSConfig {
        private static CMSConfig? _cmsConfig;

        public static CMSConfig Instance {
            get { return _cmsConfig ??= new CMSConfig(); }
        }

        private string? _configDir;

        public string ConfigDir {
            get
            {
                _configDir ??= System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CMSConfig");
                if (!System.IO.Directory.Exists(_configDir))
                {
                    System.IO.Directory.CreateDirectory(_configDir);
                }

                return _configDir;
            }
        }

        public static string ConfigJsonDir(string jsonName) {
            if (!jsonName.Contains(".json"))
            {
                jsonName += ".json";
            }

            return System.IO.Path.Combine(Instance.ConfigDir, jsonName);
        }
    }
}