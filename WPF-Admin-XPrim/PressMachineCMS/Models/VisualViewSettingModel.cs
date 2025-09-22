using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using WPF.Admin.Models;

namespace PressMachineCMS.Models
{
    public partial class VisualViewSettingModel : BindableBase
    {
        [ObservableProperty] private int _column;
        public ObservableCollection<VisualContent> Content { get; set; } = new ObservableCollection<VisualContent>();
    }

    public partial class VisualContent : BindableBase
    {
        [ObservableProperty] private string drwaing;
        [ObservableProperty] private string pressPos;
        [ObservableProperty] private string pressPre;
        [ObservableProperty] private string pressRes;
        [ObservableProperty] private string pressMachineNo;
        [ObservableProperty] private string? pressMachineName = "NormalPressMachine";
        [ObservableProperty] private string? plcKey = null;
        [ObservableProperty] private bool havingVisualContentContent;
        public ObservableCollection<VisualContentContent> VisualContentContent { get; set; } = new ObservableCollection<VisualContentContent>();
        public ObservableCollection<VisualContentContent> ShowVisualContent { get; set; } = new ObservableCollection<VisualContentContent>();
    }

    public partial class VisualContentContent : BindableBase
    {
        [ObservableProperty] private string? name = "NormalContent";
        [ObservableProperty] private string? point = null;
        [ObservableProperty] private string? unit = null;
        public override string ToString()
        {
            return $"{name}-{point}-{unit}";
        }
    }
}