using CommunityToolkit.Mvvm.ComponentModel;
using HandyControl.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressMachineMainModeules.Models
{
    public partial class PloModelControlDt : ObservableObject
    {
        /// <summary>
        /// 当前位移
        /// </summary>
        [ObservableProperty] private float? nowPos = 0;
        /// <summary>
        /// 当前压力
        /// </summary>
        [ObservableProperty] private float? nowPre = 0;
        /// <summary>
        /// 压装结果
        /// </summary>
        [ObservableProperty] private string? result;
        /// <summary>
        /// 最大位移
        /// </summary>
        [ObservableProperty] private float? maxPos = 0;
        /// <summary>
        /// 最大压力
        /// </summary>
        [ObservableProperty] private float? maxPre = 0;
        /// <summary>
        /// 拐点位移
        /// </summary>
        [ObservableProperty] private float? inflectionPos = 0;
        /// <summary>
        /// 拐点压力
        /// </summary>
        [ObservableProperty] private float? inflectionPre = 0;
        [ObservableProperty] private float? sudu = 0;
        [ObservableProperty] private float? proSum = 0;
        [ObservableProperty] private float? time = 0;
        [ObservableProperty] private float? hz = 0;
        public void ResetResult()
        {
            this.Result = "";
        }


        public void SetResult(string result)
        {
            this.Result = result;
        }


        public void SetNowValue(float x,float y)
        {
            this.NowPos = x;
            this.NowPre = y;
        }
    }
}
