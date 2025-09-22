using CommunityToolkit.Mvvm.ComponentModel;
using PressMachineMainModeules.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PressMachineMainModeules.Models
{
    /// <summary>
    /// PLC参数
    /// </summary>

    public partial class PressMachineParamsDa : ObservableValidator
    {
        private bool _isValidating = false;


        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "不能为空值")]
        private float _待机位置 = 10;


        [Required(ErrorMessage = "不能为空值")]
        [NotifyDataErrorInfo]
        [ObservableProperty] private float _预压位置 = 10;

        [Required(ErrorMessage = "不能为空值")]
        [NotifyDataErrorInfo]
        [ObservableProperty] private float _预压速度 = 20;



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
        [ObservableProperty] private float _第四位置 = 35;


        [Required(ErrorMessage = "不能为空值")]
        [NotifyDataErrorInfo]
        [ObservableProperty] private float _待机速度 = 15;

        [Required(ErrorMessage = "不能为空值")]
        [NotifyDataErrorInfo]
        [ObservableProperty] private float _第一速度 = 10;
       
        [Required(ErrorMessage = "不能为空值")]
        [NotifyDataErrorInfo]
        [ObservableProperty] private float _第二速度 = 5;
     
        [Required(ErrorMessage = "不能为空值")]
        [NotifyDataErrorInfo]
        [ObservableProperty] private float _第三速度 = 3;
        [Required(ErrorMessage = "不能为空值")]
        [NotifyDataErrorInfo]
        [ObservableProperty] private float _第四速度 = 2;
        
        [Required(ErrorMessage = "不能为空值")]
        [NotifyDataErrorInfo]
        [ObservableProperty] private float _保压时间 = 1;
        [Required(ErrorMessage = "不能为空值")]
    
        [NotifyDataErrorInfo]
        [ObservableProperty] private float _保护压力 = 100;
       
        [Required(ErrorMessage = "不能为空值")]
        [NotifyDataErrorInfo]
        [ObservableProperty] private float _压力补偿 = 0;

      
        [Required(ErrorMessage = "不能为空值")]
        [NotifyDataErrorInfo]
        [ObservableProperty] private float _位置容差 = 0;

        public bool CanSubmit()
        {
            ValidateAllProperties();
            return !HasErrors;
        }
    }
}
