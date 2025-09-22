using System.IO;
using System.Text;
using System.Text.Json;
using WPF.Admin.Models.Models;

namespace WPF.Admin.Themes.Converter
{
    public static class SerializeHelper
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,

            WriteIndented = true,  // 格式化 JSON 输出
            PropertyNameCaseInsensitive = true  // 属性名称不区分大小写
        };

        private static readonly Encoding _encoding = new UTF8Encoding(true); // 使用 BOM 的 UTF8 编码

        /// <summary>
        /// 异步将对象序列化为 JSON 并保存到文件
        /// </summary>
        public static async Task SerializeAsync<T>(string filename, T target)
        {
            if (ApplicationAuthTaskFactory.AuthFlag)
            {
                throw new SerializationException("授权失败，无法保存文件");
            }
            try
            {
                // 确保目录存在
                var directory = Path.GetDirectoryName(filename);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // 序列化并写入文件
                var jsonString = JsonSerializer.Serialize(target, _options);
                await File.WriteAllTextAsync(filename, jsonString, _encoding);
            }
            catch (Exception ex)
            {
                throw new SerializationException($"保存文件 {filename} 时发生错误", ex);
            }
        }

        /// <summary>
        /// 将对象序列化为 JSON 并保存到文件
        /// </summary>
        public static void Serialize<T>(string filename, T target)
        {
            if (ApplicationAuthTaskFactory.AuthFlag)
            {
                throw new SerializationException("授权失败，无法保存文件");
            }
            try
            {
                // 确保目录存在
                var directory = Path.GetDirectoryName(filename);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // 序列化并写入文件
                var jsonString = JsonSerializer.Serialize(target, _options);
                File.WriteAllText(filename, jsonString, _encoding);
            }
            catch (Exception ex)
            {
                throw new SerializationException($"保存文件 {filename} 时发生错误", ex);
            }
        }

        /// <summary>
        /// 异步从文件读取并反序列化为对象
        /// </summary>
        public static async Task<T> DeserializeAsync<T>(string filename)
        {
            if (ApplicationAuthTaskFactory.AuthFlag)
            {
                throw new SerializationException("授权失败，无法读取文件");
            }
            try
            {
                if (!File.Exists(filename))
                {
                    throw new FileNotFoundException($"文件 {filename} 不存在");
                }

                var jsonString = await File.ReadAllTextAsync(filename, _encoding);
                return JsonSerializer.Deserialize<T>(jsonString, _options);
            }
            catch (Exception ex)
            {
                throw new SerializationException($"读取文件 {filename} 时发生错误", ex);
            }
        }

        /// <summary>
        /// 从文件读取并反序列化为对象
        /// </summary>
        public static T Deserialize<T>(string filename)
        {
            if (ApplicationAuthTaskFactory.AuthFlag)
            {
                throw new SerializationException("授权失败，无法读取文件");
            }
            try
            {
                if (!File.Exists(filename))
                {

                    throw new FileNotFoundException($"文件 {filename} 不存在");
                }

                var jsonString = File.ReadAllText(filename, _encoding);

                return JsonSerializer.Deserialize<T>(jsonString, _options);
            }
            catch (Exception ex)
            {

                throw new SerializationException($"读取文件 {filename} 时发生错误", ex);
            }
        }
    }

    /// <summary>
    /// 自定义序列化异常
    /// </summary>
    public class SerializationException : Exception
    {
        public SerializationException(string message, Exception innerException = null)
            : base(message, innerException)
        {
        }
    }
}
