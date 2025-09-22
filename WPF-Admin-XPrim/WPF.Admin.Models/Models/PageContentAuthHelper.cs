

using CommunityToolkit.Mvvm.ComponentModel;

namespace WPF.Admin.Models.Models
{
    public partial class PageContentAuth : ObservableObject
    {
        [ObservableProperty]
        private LoginAuth _Admin = LoginAuth.Admin;
        [ObservableProperty]
        private LoginAuth _Employee = LoginAuth.Employee;
        [ObservableProperty]
        private LoginAuth _Engineer = LoginAuth.Engineer;
    }
    public class PageContentAuthHelper
    {
        public static PageContentAuth ContentAuth { get; set; } = new PageContentAuth();
    }
}
