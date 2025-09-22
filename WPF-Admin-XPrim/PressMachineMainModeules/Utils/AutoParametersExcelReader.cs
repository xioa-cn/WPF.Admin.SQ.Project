using OfficeOpenXml;
using PressMachineMainModeules.Models;
using System.IO;
using WPF.Admin.Models.Utils;

namespace PressMachineMainModeules.Utils {
    public class AutoParametersExcelReader {
        public static AutoParameterModel ReadExcel(string? filePath = null, string sheetName = "Parameters") {
            // Implement the logic to read the Excel file and populate the AutoParameterModel
            // This is a placeholder for the actual implementation
            AutoParameterModel model = new AutoParameterModel();
            // Example of populating the model with dummy data
            if (string.IsNullOrEmpty(filePath))
            {
                filePath =  PressMachineMainModeules.Config.ConfigPlcs.ConfigPath;
            }

            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException(nameof(filePath));
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using ExcelPackage package = new ExcelPackage(filePath, ApplicationConfigConst.Pwd);
            var autoParameters = package.Workbook.Worksheets[sheetName];

            var row = 2;
            var reader = true;
            while (reader)
            {
                try
                {
                    var key = autoParameters.Cells[row, 1].GetValue<string>() ?? string.Empty;
                    if (string.IsNullOrEmpty(key))
                    {
                        reader = false;
                        continue;
                    }


                    if (!model.TryGetValue(key, out var instance))
                    {
                        model[key] = new AutoParameterContent() {
                            Key = key,

                            Instance = new AutoParametersInstance()
                        };
                    }

                    var type = autoParameters.Cells[row, 2].GetValue<string>() ?? string.Empty;

                    model[key].Type = type;
                    model[key].Instance.ParameterName = key;
                    model[key].Instance.Type = type;
                    model[key].AutoMode = autoParameters.Cells[row, 3].GetValue<string>() ?? string.Empty;
                    var desc = autoParameters.Cells[row, 4].GetValue<string>() ?? string.Empty;

                    AutoParameterContentModel c;

                    if (desc.Contains("#"))
                    {
                        c = new AutoParameterContentModel {
                            Desc = desc,
                            Type = "1",
                            Unit = desc,
                            Db = desc,
                            Point = desc,
                            PlcName = autoParameters.Cells[row, 9].GetValue<string>() ?? string.Empty,
                            Min = string.Empty,
                            Max = string.Empty
                        };
                    }
                    else
                    {
                        c = new AutoParameterContentModel {
                            Desc = desc,
                            Type = autoParameters.Cells[row, 5].GetValue<string>() ?? string.Empty,
                            Unit = autoParameters.Cells[row, 6].GetValue<string>() ?? string.Empty,
                            Db = autoParameters.Cells[row, 7].GetValue<string>() ?? string.Empty,
                            Point = autoParameters.Cells[row, 8].GetValue<string>() ?? string.Empty,
                            PlcName = autoParameters.Cells[row, 9].GetValue<string>() ?? string.Empty,
                            Min = autoParameters.Cells[row, 11].GetValue<string>() ?? string.Empty,
                            Max = autoParameters.Cells[row, 12].GetValue<string>() ?? string.Empty,
                        };
                    }

                    var sf = autoParameters.Cells[row, 13].GetValue<string>() ?? string.Empty;

                    if (!string.IsNullOrEmpty(sf))
                    {
                        (Calculation cal, Multiplier mul) = MultiplierHelper.AnalysisCalculation(sf);
                        c.Calculation = cal;
                        c.Multiplier = mul;
                    }

                    model[key].Step = autoParameters.Cells[row, 10].GetValue<string>() ?? string.Empty;
                    model[key].Instance.Content.Add(c);
                    model[key].Instance.CheckAutoMode();
                    row++;
                }
                catch
                {
                    reader = false;
                    continue;
                }
            }

            return model;
        }
    }
}