using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Xml.Serialization;

namespace WPF.Admin.Models.Utils
{
    public static class DeepCopyHelper
    {

        // 使用JSON序列化（.NET Core 3.0+，无需标记特性）
        public static T JsonClone<T>(T obj)
        {
            if (obj == null)
                return default;

            var json = JsonSerializer.Serialize(obj);
            return JsonSerializer.Deserialize<T>(json);
        }

        // 方法3: 使用XML序列化
        public static T XmlClone<T>(T obj) where T : class
        {
            if (obj == null)
                return null;

            using (var stream = new MemoryStream())
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stream, obj);
                stream.Position = 0;
                return serializer.Deserialize(stream) as T;
            }
        }
    }
}