using System.IO;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;
using System.Windows;

namespace WPF.Admin.Themes.Helper
{
    public class PrintXPSHelper
    {
        public static void PrintXPS(Visual visual)
        {
            string tempXpsFile = Path.GetTempFileName() + ".xps";
            FixedDocument? fixedDocument = null;
            try
            {              
                // 创建用于打印的控件副本
                FrameworkElement printElement = null;
               

                // 创建XPS文档并获取固定文档
                using (XpsDocument xpsDocument = new XpsDocument(tempXpsFile, FileAccess.ReadWrite))
                {
                    XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
                    writer.Write(printElement ?? visual);
                    fixedDocument = xpsDocument.GetFixedDocumentSequence().References[0].GetDocument(true);
                }

                // 创建文档查看器
                DocumentViewer viewer = new DocumentViewer();
                viewer.Document = fixedDocument;

                // 创建打印预览窗口
                PrintPreviewWindow previewWindow = new PrintPreviewWindow() { PrintView = viewer };

                // 显示预览窗口
                if (previewWindow.ShowDialog() == true)
                {

                }

            }
            finally
            {
                // 确保临时文件被删除
                if (File.Exists(tempXpsFile))
                {
                    try
                    {
                        File.Delete(tempXpsFile);
                    }
                    catch { }
                }
            }
        }

        // 打印预览窗口类
        public class PrintPreviewWindow : Window
        {
            public object? PrintView { get; set; }
            public PrintPreviewWindow()
            {
                Title = "打印预览";
                Width = 800;
                Height = 600;
                WindowStartupLocation = WindowStartupLocation.CenterScreen;


                this.Loaded += PrintPreviewWindow_Loaded;
            }

            private void PrintPreviewWindow_Loaded(object sender, RoutedEventArgs e)
            {
                // 保存原始内容
                var originalContent = PrintView;
                // 创建布局
                Grid grid = new Grid();
                grid.Children.Add((UIElement)originalContent);                
                Content = grid;
            }
        }
    }
}
