using CommunityToolkit.Mvvm.ComponentModel;
using XPrism.Core.BindableBase;

namespace WPF.Admin.Models.Models
{
    public class SignalDbModelEntity
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Plc { get; set; }
        public string Position { get; set; }
        public string Value { get; set; }
    }
    public partial class SignalDbModel : ViewModelBase
    {
        public int Id { get; set; }
        [ObservableProperty] public string _Code;
        [ObservableProperty] public string _Plc;
        [ObservableProperty] public string _Position;
        [ObservableProperty] public string _Value;
    }
}