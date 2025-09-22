namespace PressMachineMainModeules.Interfaces
{
    public interface IPositionSwitchable
    {
        /// <summary>
        /// 切换指定Border到大容器位置
        /// </summary>
        /// <param name="borderName">Border的名称 (pre1, pre2, pre3, pre4)</param>
        /// <returns>切换是否成功</returns>
        bool SwitchToLargePosition(string borderName);

        /// <summary>
        /// 获取当前在大容器位置的Border名称
        /// </summary>
        /// <returns>当前大容器Border的名称</returns>
        string GetCurrentLargeBorderName();
    }
} 