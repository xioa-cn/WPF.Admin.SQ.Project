using CommunityToolkit.Mvvm.ComponentModel;
using MathNet.Numerics.Differentiation;
using System.ComponentModel;

namespace PressMachineMainModeules.Models
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum StepStatus
    {
        /// <summary>
        /// Waitting for motion command.
        /// </summary>
        [Description("空闲")]
        Idle,

        /// <summary>
        /// Device is running.
        /// </summary>
        [Description("进行中")]
        Busy,

        /// <summary>
        /// Command has been finished.
        /// </summary>
        [Description("完成")]
        Finised
    }
    public partial class Step : ObservableObject
    {
        [ObservableProperty] private int _id;
        [ObservableProperty] private string? _name;
        [ObservableProperty] private StepType _stepType;
        [ObservableProperty] private float _value;
        [ObservableProperty] private float _speed;
        [ObservableProperty] private float _exceedValue;
        [ObservableProperty] private AlarmReaction _alarmReaction;
        [ObservableProperty] private float _time;
        [ObservableProperty] private StepStatus _status = StepStatus.Idle;
        [ObservableProperty] private bool _internalPosition;
    }
}
