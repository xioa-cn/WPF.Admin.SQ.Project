using System.Collections.ObjectModel;
using PressMachineMainModeules.Models;
using PressMachineMainModeules.ViewModels;

namespace PressMachineMainModeules.Utils {
    public static class AutoPlotAddHomePositionHelper {
        public static void AddHomePosition(this ObservableCollection<AutoPlotValue> autoPlotValue,
            HomePositionModel item) {
            // if (!string.IsNullOrEmpty(item.StartPosition))
            // {
            //     autoPlotValue.Add(new AutoPlotValue {
            //         PlcName = item.PlcName,
            //         DbPoint = item.StartPosition,
            //     });
            // }

            if (!string.IsNullOrEmpty(item.PressPosition))
            {
                autoPlotValue.Add(new AutoPlotValue {
                    PlcName = item.PlcName,
                    DbPoint = item.PressPosition,
                });
            }

            if (!string.IsNullOrEmpty(item.PositionPosition))
            {
                autoPlotValue.Add(new AutoPlotValue {
                    PlcName = item.PlcName,
                    DbPoint = item.PositionPosition,
                });
            }

            if (!string.IsNullOrEmpty(item.MaxPosition))
            {
                autoPlotValue.Add(new AutoPlotValue {
                    PlcName = item.PlcName,
                    DbPoint = item.MaxPosition,
                });
            }

            if (!string.IsNullOrEmpty(item.MaxPress))
            {
                autoPlotValue.Add(new AutoPlotValue {
                    PlcName = item.PlcName,
                    DbPoint = item.MaxPress,
                });
            }

            if (!string.IsNullOrEmpty(item.ResultPosition))
            {
                autoPlotValue.Add(new AutoPlotValue
                {
                    Name = "压装结果",
                    PlcName = item.PlcName,
                    DbPoint = item.ResultPosition,
                });
            }

            if (!string.IsNullOrEmpty(item.OKSum) && !string.IsNullOrEmpty(item.NGSum))
            {
                autoPlotValue.Add(new AutoPlotValue {
                    PlcName = item.PlcName,
                    DbPoint = item.OKSum,
                });
                autoPlotValue.Add(new AutoPlotValue {
                    PlcName = item.PlcName,
                    DbPoint = item.NGSum,
                });
                // 总数量
                var allSum = new AutoPlotValue {
                    Name = "生产总数",
                    Unit = ""
                };
                autoPlotValue.Add(allSum);
                // 合格率
                autoPlotValue.Add(new AutoPlotValue {
                    Name = "合格率",
                    Unit = "%"
                });
            }

            if (!string.IsNullOrEmpty(item.EndPress))
            {
                autoPlotValue.Add(new AutoPlotValue {
                    PlcName = item.PlcName,
                    DbPoint = item.EndPress,
                });
            }

            if (!string.IsNullOrEmpty(item.Add1))
            {
                autoPlotValue.Add(new AutoPlotValue {
                    PlcName = item.PlcName,
                    DbPoint = item.Add1,
                });
            }

            if (!string.IsNullOrEmpty(item.Add2))
            {
                autoPlotValue.Add(new AutoPlotValue {
                    PlcName = item.PlcName,
                    DbPoint = item.Add2,
                });
            }

            if (!string.IsNullOrEmpty(item.Add3))
            {
                autoPlotValue.Add(new AutoPlotValue {
                    PlcName = item.PlcName,
                    DbPoint = item.Add3,
                });
            }
        }
    }
}