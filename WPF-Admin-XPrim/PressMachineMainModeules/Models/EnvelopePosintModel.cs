using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace PressMachineMainModeules.Models
{
    public partial class PosintModel : ObservableObject
    {
        [ObservableProperty]
        private float _x1;
        [ObservableProperty]
        private float _y1;
      
        [ObservableProperty]
        private float _y2;
    }

    public partial class EnvelopePosintModel : ObservableObject
    {
        [ObservableProperty] private bool _isSelected;
        [ObservableProperty] private bool _top = true;
        [ObservableProperty] private bool _bottom = true;
        [ObservableProperty]
        private ObservableCollection<PosintModel> _Posints = new ObservableCollection<PosintModel>();
    }

    public enum WeakEnvelopePosintModelStatus
    {
        Show,
        Hide
    }

    public class WeakEnvelopePosintModel
    {
        public string AutoMode { get; set; }
        public EnvelopePosintModel? Value { get; set; }
        public WeakEnvelopePosintModelStatus Status { get; set; } = WeakEnvelopePosintModelStatus.Show;
    }

    public class WeakEnvelopePosintView_Model
    {
        public PosintModel[] Points { get; set; }
    }
}
