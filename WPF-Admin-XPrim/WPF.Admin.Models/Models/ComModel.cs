using System.IO.Ports;
using CommunityToolkit.Mvvm.ComponentModel;
using XPrism.Core.BindableBase;

namespace WPF.Admin.Models.Models
{
    public class ComModelEntity
    {
        public int Id { get; set; }
        public string? ComName { get; set; }
        public int ComBaudRate { get; set; }
        public int ComDataBits { get; set; }
        public Parity ComParity { get; set; }
        public StopBits ComStopBits { get; set; }
        public bool AutoConnect { get; set; }
    }

    public partial class ComModel : ViewModelBase
    {

        [ObservableProperty] private bool _autoConnect;
        [ObservableProperty] private string _comName;
        [ObservableProperty] private int _comBaudRate;
        [ObservableProperty] private int _comDataBits;
        [ObservableProperty] private Parity _comParity;
        [ObservableProperty] private StopBits _comStopBits;
    }
}