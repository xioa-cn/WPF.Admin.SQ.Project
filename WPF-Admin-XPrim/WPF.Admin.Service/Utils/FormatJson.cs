using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonException = System.Text.Json.JsonException;

namespace WPF.Admin.Service.Utils {
    public class FormatJsonHelper {
        public static string FormatJson(string input) {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new NullReferenceException();
            }

            JToken parsedJson;
            if (input.TrimStart().StartsWith("{"))
            {
                parsedJson = JObject.Parse(input);
            }
            else if (input.TrimStart().StartsWith("["))
            {
                parsedJson = JArray.Parse(input);
            }
            else
            {
                throw new JsonException("无效的JSON格式");
            }

            // 格式化JSON
            string formattedJson = parsedJson.ToString(Formatting.Indented);
            return formattedJson;
        }
    }
}