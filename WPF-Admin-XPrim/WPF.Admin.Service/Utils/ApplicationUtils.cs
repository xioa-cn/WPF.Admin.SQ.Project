using System.IO;
using System.Windows;
using System.Windows.Resources;

namespace WPF.Admin.Service.Utils {
    public static class ApplicationUtils {
        public static string FindApplicationResourceFile(string appName, string resourcesName) {
            using StreamReader reader = new StreamReader(FindApplicationResourceStream(appName, resourcesName));
            string result = reader.ReadToEnd();
            return result;
        }

        public static Stream FindApplicationResourceStream(string appName, string resourcesName) {
            return FindApplicationResourceStreamInfo(appName, resourcesName).Stream;
        }

        public static StreamResourceInfo FindApplicationResourceStreamInfo(string appName, string resourcesName) {
            if (string.IsNullOrEmpty(appName))
            {
                throw new ArgumentNullException(nameof(appName));
            }

            if (string.IsNullOrEmpty(resourcesName))
            {
                throw new ArgumentNullException(nameof(resourcesName));
            }

            StreamResourceInfo? sri = Application.GetResourceStream(
                new Uri($"pack://application:,,,/{appName};component/{resourcesName}"));
            if (sri is null) throw new Exception("Not Found Resources");

            return sri;
        }
    }
}