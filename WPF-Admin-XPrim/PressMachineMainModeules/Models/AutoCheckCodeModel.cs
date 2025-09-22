namespace PressMachineMainModeules.Models {
    public partial class AutoCheckCodeModel {
        /// <summary>
        /// 主码检查
        /// </summary>
        public bool OpenCheckMainCode { get; set; }

        /// <summary>
        /// 主码数量
        /// </summary>
        public int CheckMainCodeSum { get; set; }

        /// <summary>
        /// 零件码检查
        /// </summary>
        public bool OpenCheckPartialCode { get; set; }

        /// <summary>
        /// 零件码检查PLC对应
        /// </summary>
        public string CheckPartialCodePlcName { get; set; }

        /// <summary>
        /// 零件码检查PLC地址
        /// </summary>
        public string CheckPartialCodeDbPosintion { get; set; }

        /// <summary>
        /// 零件码数量
        /// </summary>
        public int CheckPartialCodeSum { get; set; }

        /// <summary>
        /// 工装码检查
        /// </summary>
        public bool OpenWorkwearCode { get; set; }

        /// <summary>
        /// 工装绑定配方
        /// </summary>
        public bool OpenWorkwearChangeParameter { get; set; }

        /// <summary>
        /// 是否开启分布扫码
        /// </summary>
        public bool SelectStepCheckCodeOpen { get; set; }

        /// <summary>
        /// 步序号是否有延迟
        /// </summary>
        public bool StepSleep { get; set; }

        /// <summary>
        /// 曲线条码绑定
        /// </summary>
        public bool OpenPlotBindingCode { get; set; }
    }
}