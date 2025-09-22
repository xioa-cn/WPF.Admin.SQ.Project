using OfficeOpenXml;
using PressMachineMainModeules.Config;
using System.IO;
using PressMachineMainModeules.Models;
using WPF.Admin.Models.Utils;

namespace PressMachineMainModeules.Utils
{
    public class ConfigCodeExcelReader
    {
        public static ConfigCode ReadExcel(string? filePath = null, string sheetName = "Code")
        {
            var configCode = new ConfigCode();
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
            var codeSheet = package.Workbook.Worksheets[sheetName];

            var row = 2;
            var reader = true;
            while (reader)
            {
                try
                {
                    if (row == 4)
                    {
                        break;
                    }

                    var key = codeSheet.Cells[row, 1].GetValue<string>() ?? string.Empty;
                    if (string.IsNullOrEmpty(key))
                    {
                        reader = false;
                        continue;
                    }

                    var openvalue = codeSheet.Cells[row, 2].GetValue<string>() ?? string.Empty;
                    var parametervalue = codeSheet.Cells[row, 3].GetValue<string>() ?? string.Empty;
                    var codeResult = codeSheet.Cells[row, 4].GetValue<string>() ?? string.Empty;
                    var plcName = codeSheet.Cells[row, 5].GetValue<string>() ?? string.Empty;

                    row++;
                    if (openvalue.ToLower() == "true")
                    {
                        configCode.Parameter = parametervalue;
                    }
                    else
                    {
                        continue;
                    }

                    if (key.Contains("串口"))
                    {
                        configCode.CodeMode = CodeModeEnum.ComCode;
                        configCode.CodeResult = codeResult;
                        configCode.PlcName = plcName;
                    }
                    else if (key.Contains("PLC"))
                    {
                        configCode.CodeMode = CodeModeEnum.PlcCode;
                        configCode.CodeResult = codeResult;
                        configCode.PlcName = plcName;
                    }
                }
                catch
                {
                    reader = false;
                    continue;
                }
            }

            if (row == 4)
            {
                configCode.BarcodePlagiarismCheck = new ConfigInformationModel();

                var openvalue = codeSheet.Cells[row, 2].GetValue<string>() ?? string.Empty;
                var parametervalue = codeSheet.Cells[row, 3].GetValue<string>() ?? string.Empty;
                var codeResult = codeSheet.Cells[row, 4].GetValue<string>() ?? string.Empty;
                var plcName = codeSheet.Cells[row, 5].GetValue<string>() ?? string.Empty;
                if (openvalue.ToLower() == "true")
                {
                    configCode.BarcodePlagiarismCheck.IsOpen = true;
                    configCode.BarcodePlagiarismCheck.DbPosint = codeResult;
                    configCode.BarcodePlagiarismCheck.PlcName = plcName;
                }

                row++;
            }

            if (row == 5)
            {
                configCode.Rework = new ConfigInformationModel();
                var openvalue = codeSheet.Cells[row, 2].GetValue<string>() ?? string.Empty;
                var parametervalue = codeSheet.Cells[row, 3].GetValue<string>() ?? string.Empty;
                var codeResult = codeSheet.Cells[row, 4].GetValue<string>() ?? string.Empty;
                var plcName = codeSheet.Cells[row, 5].GetValue<string>() ?? string.Empty;
                if (openvalue.ToLower() == "true")
                {
                    configCode.Rework.IsOpen = true;
                    configCode.Rework.DbPosint = codeResult;
                    configCode.Rework.PlcName = plcName;
                }
            }


            return configCode;
        }
    }
}