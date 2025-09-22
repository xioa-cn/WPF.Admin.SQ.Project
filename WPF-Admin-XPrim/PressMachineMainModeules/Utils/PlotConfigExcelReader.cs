using System.Collections.ObjectModel;
using System.IO;
using OfficeOpenXml;
using PressMachineMainModeules.Models;
using WPF.Admin.Models.Utils;

namespace PressMachineMainModeules.Utils {
    public static class PlotConfigExcelReader {
        public static ObservableCollection<PlotConfigModel> ReadExcel(string? filePath = null,
            string sheetName = "PlotConfig") {
            var plotConfigModels = new ObservableCollection<PlotConfigModel>();
            // Implement the logic to read the Excel file and populate ioPointPositions
            // This is a placeholder for the actual implementation
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = PressMachineMainModeules.Config.ConfigPlcs.ConfigPath;
            }

            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException(nameof(filePath));
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using ExcelPackage package = new ExcelPackage(filePath, ApplicationConfigConst.Pwd);
            var plotConfig = package.Workbook.Worksheets[sheetName];

            var row = 2;
            var reader = true;
            while (reader)
            {
                try
                {
                    var autoMode = plotConfig.Cells[row, 1].GetValue<string>() ?? string.Empty;
                    if (string.IsNullOrEmpty(autoMode))
                    {
                        reader = false;
                        continue;
                    }

                    var xName = plotConfig.Cells[row, 2].GetValue<string>() ?? string.Empty;
                    var xUnit = plotConfig.Cells[row, 3].GetValue<string>() ?? string.Empty;
                    var yName = plotConfig.Cells[row, 4].GetValue<string>() ?? string.Empty;
                    var yUnit = plotConfig.Cells[row, 5].GetValue<string>() ?? string.Empty;
                    var zoom = plotConfig.Cells[row, 6].GetValue<string>() ?? string.Empty;
                    var pan = plotConfig.Cells[row, 7].GetValue<string>() ?? string.Empty;
                    var color = plotConfig.Cells[row, 8].GetValue<string>() ?? string.Empty;
                    row++;
                    if (autoMode.Contains("备用"))
                    {
                        continue;
                    }

                    var plotConfigModel = new PlotConfigModel(autoMode) {
                        XName = xName,
                        XUnit = xUnit,
                        YName = yName,
                        YUnit = yUnit,
                        IsZoom = zoom.ToLower() == "true",
                        IsPan = pan.ToLower() == "true",
                        PlotColor = color
                    };
                    plotConfigModels.Add(plotConfigModel);
                }
                catch
                {
                    reader = false;
                    continue;
                }
            }


            return plotConfigModels;
        }
    }
}