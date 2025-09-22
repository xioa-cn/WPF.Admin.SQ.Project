using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPF.Admin.Themes.Helper.Extensions.Temp {
    public static class IconHelper {
        // 将 WPF ImageSource 转换为 System.Drawing.Icon
        public static Icon ToIcon(this ImageSource imageSource) {
            if (imageSource == null)
                return null;

            // 将 ImageSource 转换为 BitmapSource
            BitmapSource bitmapSource = imageSource as BitmapSource;
            if (bitmapSource == null)
            {
                // 如果不是 BitmapSource，尝试创建一个
                BitmapFrame bitmapFrame = imageSource as BitmapFrame;
                if (bitmapFrame != null)
                    bitmapSource = bitmapFrame;
                else
                {
                    // 尝试从 Uri 加载（适用于 pack URI）
                    try
                    {
                        var uri = new Uri(imageSource.ToString());
                        bitmapSource = BitmapFrame.Create(uri);
                    }
                    catch
                    {
                        return null;
                    }
                }
            }

            // 将 BitmapSource 转换为 System.Drawing.Bitmap
            using (var memoryStream = new MemoryStream())
            {
                // 创建 PNG 编码器
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(memoryStream);

                // 从流中创建 Bitmap
                using (var bitmap = new Bitmap(memoryStream))
                {
                    // 从 Bitmap 创建 Icon
                    IntPtr hIcon = bitmap.GetHicon();
                    Icon icon = Icon.FromHandle(hIcon);

                    // 注意：必须释放 HICON 资源
                    Win32.DestroyIcon(hIcon);

                    return icon;
                }
            }
        }

        // Win32 API 声明
        private static class Win32 {
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern bool DestroyIcon(IntPtr hIcon);
        }
    }
}