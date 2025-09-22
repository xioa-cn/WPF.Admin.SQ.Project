using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PressMachineMainModeules.Models
{
    /// <summary>
    /// X轴参数
    /// </summary>
    public partial class PressMachineParamsXDa : ObservableValidator
    {
        [Required(ErrorMessage = "不能为空值")]
        [NotifyDataErrorInfo]
        [ObservableProperty] private float _待机位置 = 12;   

        [Required(ErrorMessage = "不能为空值")]
        [NotifyDataErrorInfo]
        [ObservableProperty] private float _第一位置 = 15;

        [Required(ErrorMessage = "不能为空值")]
        [NotifyDataErrorInfo]
        [ObservableProperty] private float _第二位置 = 20;

        [Required(ErrorMessage = "不能为空值")]
        [NotifyDataErrorInfo]
        [ObservableProperty] private float _第三位置 = 25;
        [Required(ErrorMessage = "不能为空值")]
        [NotifyDataErrorInfo]
        [ObservableProperty] private float _第四位置 = 25;


        [Required(ErrorMessage = "不能为空值")]
        [NotifyDataErrorInfo]
        [ObservableProperty] private float _待机速度 = 10;

        [Required(ErrorMessage = "不能为空值")]
        [NotifyDataErrorInfo]
        [ObservableProperty] private float _第一速度 = 10;

        [Required(ErrorMessage = "不能为空值")]
        [NotifyDataErrorInfo]
        [ObservableProperty] private float _第二速度 = 5;

        [Required(ErrorMessage = "不能为空值")]
        [NotifyDataErrorInfo]
        [ObservableProperty] private float _第三速度 = 1;
        [Required(ErrorMessage = "不能为空值")]
        [NotifyDataErrorInfo]
        [ObservableProperty] private float _第四速度 = 1;

    }
}
