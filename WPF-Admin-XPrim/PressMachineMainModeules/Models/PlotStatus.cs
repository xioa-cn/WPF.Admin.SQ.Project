namespace PressMachineMainModeules.Models;

public class PlotStatus
{
    public bool Plot01End { get; set; } = true;
    public bool Plot02End { get; set; } = true;
    public bool Plot03End { get; set; } = true;
    public bool Plot04End { get; set; } = true;
    public bool Plot05End { get; set; } = true;

    public bool GetHavingStart()
    {
        return !Plot01End || !Plot02End || !Plot03End || !Plot04End || !Plot05End;
    }

    public bool GetAllEnd()
    {
        return Plot01End && Plot02End && Plot03End && Plot04End && Plot05End;
    }
}