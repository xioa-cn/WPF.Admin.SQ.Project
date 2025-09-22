using OfficeOpenXml;
using PressMachineMainModeules.Models;
using System.Collections.ObjectModel;
using System.IO;
using WPF.Admin.Models.Utils;

namespace PressMachineMainModeules.Utils
{
    public class HomeExcelReader
    {
        public static ObservableCollection<HomePositionModel> ReadExcel(string? filePath = null,
            string sheetName = "IOPointPositions")
        {
            var homePositionModels = new ObservableCollection<HomePositionModel>();
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

                    var start = ioSheet.Cells[row, 2].GetValue<string>() ?? string.Empty;
                    var pos = ioSheet.Cells[row, 3].GetValue<string>() ?? string.Empty;
                    var pre = ioSheet.Cells[row, 4].GetValue<string>() ?? string.Empty;
                    var result = ioSheet.Cells[row, 5].GetValue<string>() ?? string.Empty;
                    var num = ioSheet.Cells[row, 6].GetValue<string>() ?? string.Empty;
                    var plcnum = ioSheet.Cells[row, 7].GetValue<string>() ?? string.Empty;
                    var maxpos = ioSheet.Cells[row, 8].GetValue<string>() ?? string.Empty;
                    var maxpre = ioSheet.Cells[row, 9].GetValue<string>() ?? string.Empty;
                    var endpre = ioSheet.Cells[row, 10].GetValue<string>() ?? string.Empty;
                    var ok = ioSheet.Cells[row, 11].GetValue<string>() ?? string.Empty;
                    var ng = ioSheet.Cells[row, 12].GetValue<string>() ?? string.Empty;
                    var add1 = ioSheet.Cells[row, 13].GetValue<string>() ?? string.Empty;
                    var add2 = ioSheet.Cells[row, 14].GetValue<string>() ?? string.Empty;
                    var add3 = ioSheet.Cells[row, 15].GetValue<string>() ?? string.Empty;
                    var openStep = ioSheet.Cells[row, 16].GetValue<string>() ?? string.Empty;
                    var step = ioSheet.Cells[row, 17].GetValue<string>() ?? string.Empty;
                    var stepSum = ioSheet.Cells[row, 18].GetValue<string>() ?? "-1";
                    var formulaNum = ioSheet.Cells[row, 19].GetValue<string>() ?? string.Empty;
                    var united = ioSheet.Cells[row, 20].GetValue<string>() ?? string.Empty;
                    var unitedValue = ioSheet.Cells[row, 21].GetValue<string>() ?? string.Empty;
                    row++;
                    if (desc.Contains("备用"))
                    {
                        continue;
                    }

                    var temp = new HomePositionModel
                    {
                        Desc = desc,
                        StartPosition = start,
                        PositionPosition = pos,
                        PressPosition = pre,
                        ResultPosition = result,
                        KeyNum = num,
                        PlcName = plcnum,
                        MaxPosition = maxpos,
                        MaxPress = maxpre,
                        EndPress = endpre,
                        OKSum = ok,
                        NGSum = ng,
                        Add1 = add1,
                        Add2 = add2,
                        Add3 = add3,
                        StartStep = openStep!.ToLower() == "true",
                        Step = step,
                        FormulaNum = formulaNum,
                        StepSum = int.Parse(stepSum),
                        United = united.ToLower() == "true",
                        UnitedValue = unitedValue
                    };
                    homePositionModels.Add(temp);
                }
                catch
                {
                    reader = false;
                    continue;
                }
            }


            return homePositionModels;
        }
    }
}