using System.Text.Json.Serialization;

namespace WPF.Admin.Models.Models {
    public class ApplicationGlobalName {
        [JsonPropertyName("AppBaseOnName")] public string AppBaseOnName { get; set; }

        [JsonPropertyName("AppBaseUrl")]
        public string AppBaseUrl { get; set; } =
            System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "logo", "welcome.html");
        [JsonPropertyName("AppCompanyName")] public string AllStringName { get; set; } 
    }
}