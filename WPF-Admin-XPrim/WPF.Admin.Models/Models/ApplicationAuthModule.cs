namespace WPF.Admin.Models.Models {
    public class ApplicationAuthModule {
        public static DateTime DllCreateTime { get; set; }


        private static DateTime? _authOutTime;

        public static DateTime AuthOutTime {
            get { return _authOutTime ??= DateTime.Parse(WPF.Admin.Models.Models.AppSettings.Default.AuthOutTime); }
        }

        public static DateTime StartTime { get; set; } = DateTime.Now; 

        private static double _interval = 1000000L;

        public static double _Interval {
            get
            {
                if (ListenApplicationVersions.NormalVersion != ApplicationVersions.NoAuthorization &&
                    ApplicationAuthModule.AuthOutTime < DateTime.Now)
                {
                    return -1;
                }

                return _interval;
            }
            set { _interval = Math.Min(value, 24.5); }
        } // 体验时间限制

        private static bool _AuthTaskFlay;

        public static bool AuthTaskFlag {
            get { return _AuthTaskFlay; }
            set { _AuthTaskFlay = value; }
        }
    }
}