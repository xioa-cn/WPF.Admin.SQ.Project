using WPF.Admin.Models.Models;
using XPrism.Core.Navigations;

namespace WPFAdmin.NavigationModules.Messengers;

public enum MessengerStatus {
    None,
    FromNavBarToPage,
    FromWindowToPage,
}

public class TreeItemModelMessenger {
    public MessengerStatus MessengerStatus { get; set; }
    public TreeItemModel Item { get; set; }
    public INavigationParameters? Parameters { get; set; }
}