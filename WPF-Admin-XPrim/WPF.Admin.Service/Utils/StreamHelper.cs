using System.IO;

namespace WPF.Admin.Service.Utils
{
    public class StreamHelper
    {
        // <summary>
        /// 获取完整的文件流
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>包含完整文件内容的Stream，失败时返回null</returns>
        static Stream? GetCompleteFileStream(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"文件不存在: {filePath}");
                return null;
            }
        
            try
            {
                // 创建一个内存流用于存储完整的文件内容
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // 打开文件流
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        // 将文件流的所有内容复制到内存流
                        fileStream.CopyTo(memoryStream);
                    }
                
                    // 重置内存流的位置到起始点
                    memoryStream.Position = 0;
                
                    // 返回完整的流（这里返回内存流的一个副本，避免外部操作影响内部状态）
                    return new MemoryStream(memoryStream.ToArray());
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"I/O错误: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"权限错误: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
            }
        
            return null;
        }
    }
}