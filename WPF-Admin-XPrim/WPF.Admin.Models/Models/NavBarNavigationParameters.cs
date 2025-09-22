using XPrism.Core.Navigations;

namespace WPF.Admin.Models.Models
{
    public class NavBarNavigationParameters
    {
        /// <summary>
        /// key表示路由关键字 如果是多级得路由需要由 '-' 分割
        /// </summary>
        public string? Key { get; set; }
        public INavigationParameters Parameters { get; set; }


        public string[] GetNavKey()
        {
            if(string.IsNullOrWhiteSpace(this.Key))
                return new string[0];
            return this.Key.Split('-');
        }
    }
}
