

using OfficeOpenXml;
using PressMachineMainModeules.Models;
using System.Collections.ObjectModel;
using System.IO;
using WPF.Admin.Models.Utils;

namespace PressMachineMainModeules.Utils
{
    public static class ManualParameterExcelReader
    {
        public static ObservableCollection<ManualParametersModel> ReadExcel(string? filePath = null, string sheetName = "IOPointPositions")
        {
            var manualParameters = new ObservableCollection<ManualParametersModel>();
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
            var ioSheet = package.Workbook.Worksheets[sheetName];

            var row = 2;
            var reader = true;
            while (reader)
            {
                try
                {
                    var desc = ioSheet.Cells[row, 1].GetValue<string>() ?? string.Empty;
                    if (string.IsNullOrEmpty(desc))
                    {
                        reader = false;
                        continue;
                    }
                    var type = ioSheet.Cells[row, 2].GetValue<string>() ?? string.Empty;
                    var db = ioSheet.Cells[row, 3].GetValue<string>() ?? string.Empty;
                    var point = ioSheet.Cells[row, 4].GetValue<string>() ?? string.Empty;
                    var unit = ioSheet.Cells[row, 5].GetValue<string>() ?? string.Empty;
                    var plcname = ioSheet.Cells[row, 6].GetValue<string>() ?? string.Empty;
                    var min = ioSheet.Cells[row, 7].GetValue<string>() ?? string.Empty;
                    var max = ioSheet.Cells[row, 8].GetValue<string>() ?? string.Empty;
                    
                    if (desc.Contains("备用"))
                    {
                        continue;
                    }
                    var temp = new ManualParametersModel
                    {
                        Desc = desc,
                        Db = db,
                        Position = point,
                        Type = type,
                        Unit = unit,
                        PlcName = plcname,
                        Min = min,
                        Max = max
                    };
                    var sf = ioSheet.Cells[row, 7].GetValue<string>() ?? string.Empty;

                    if (!string.IsNullOrEmpty(sf))
                    {
                        (Calculation cal, Multiplier mul) = MultiplierHelper.AnalysisCalculation(sf);
                        temp.Calculation = cal;
                        temp.Multiplier = mul;
                    }
                    
                    manualParameters.Add(temp);
                    row++;
                }
                catch
                {
                    reader = false;
                    continue;
                }

            }


            return manualParameters;
        }
    }
}
