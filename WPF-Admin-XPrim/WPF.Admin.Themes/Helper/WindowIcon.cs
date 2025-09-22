using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPF.Admin.Models.Utils;

namespace WPF.Admin.Themes.Helper {
    public static class WindowIcon {
        public static void SetWindowIcon(this System.Windows.Window window) {
            var icon = GetIcon("logo.ico");
            window.Icon = icon;
        }

        public static ImageSource GetIcon(string path) {
            var xpath = System.IO.Path.Combine(ApplicationConfigConst.AssetsLogPath, path);
            return GetIconPack(System.IO.File.Exists(xpath) ? xpath : "pack://application:,,,/WPFAdmin;component/Assets/logo/logobase.ico");
        }
        
        private static ImageSource GetIconPack(string path)
        {
            try
            {
                if (path.StartsWith("pack://"))
                {
                    return BitmapFrame.Create(new Uri(path, UriKind.Absolute));
                }
                
                return BitmapFrame.Create(new Uri(path, UriKind.Absolute));
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}