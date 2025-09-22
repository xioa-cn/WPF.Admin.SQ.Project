using System.Windows;
using System.Windows.Input;
using System.Windows.Resources;
using WPF.Admin.Service.Utils;

namespace WPF.Admin.Themes.Helper {
    public static class CursorHelper {
        public static void UseNormalCursor(this Window window) {
            var sri = ApplicationUtils.FindApplicationResourceStream("WPF.Admin.Themes", "Resources/link.ani");
            // Application.GetResourceStream(
            //    new Uri("pack://application:,,,/;component/Resources/link.ani"));
            if (sri is null) return;
            Cursor customCursor = new Cursor(sri);
            window.Cursor = customCursor;
        }
    }
}