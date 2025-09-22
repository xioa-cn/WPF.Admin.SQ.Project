using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PressMachineCMS.Models
{
    public class RouterJsonModel
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [JsonPropertyName("page")]
        public string Page { get; set; }

        [JsonPropertyName("LoginAuth")]
        public int? LoginAuth { get; set; }

        [JsonPropertyName("children")]
        public List<RouterJsonModel> Children { get; set; }
    }

    public class RouterConfig
    {
        [JsonPropertyName("Routers")]
        public List<RouterJsonModel> Routers { get; set; }
    }
}