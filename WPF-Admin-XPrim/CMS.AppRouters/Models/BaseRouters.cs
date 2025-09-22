using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using WPF.Admin.Models;

namespace CMS.AppRouters.Models
{
    public partial class BaseRouters : BindableBase
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }
        [JsonPropertyName("content")]
        public string Content { get; set; }
        [JsonPropertyName("icon")]
        public string Icon { get; set; }
        [JsonPropertyName("page")]
        public string Page { get; set; }

        [JsonPropertyName("LoginAuth")] public int LoginAuth { get; set; } = 5;
    }

    public partial class Router
    {
        public ObservableCollection<BaseRouters> Routers { get; set; } = new();

        public static Router Instance { get; } = new();

        private Router()
        {

        }
    }


}