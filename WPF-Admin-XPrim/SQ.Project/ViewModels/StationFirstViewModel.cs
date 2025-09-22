using CommunityToolkit.Mvvm.ComponentModel;
using WPF.Admin.Models;

namespace SQ.Project.ViewModels
{
    public partial class StationFirstViewModel : BindableBase
    {
        [ObservableProperty] private string _code = string.Empty;

        [ObservableProperty] private bool _status;

        public StationFirstViewModel()
        {
            Code = "ACC20250922154660-00001";

            Status = true;
        }
    }
}