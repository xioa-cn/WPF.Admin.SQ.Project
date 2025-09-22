using System.IO;
using OfficeOpenXml;
using PressMachineMainModeules.Config;
using PressMachineMainModeules.Models;
using WPF.Admin.Models.Utils;
using WPF.Admin.Service.Logger;

namespace PressMachineMainModeules.Utils {
    public static class CheckCodeExcelReader {
        public static AutoCheckCodeModel ReadExcel(string? filePath = null, string sheetName = "SelfCheckCode") {
            var autoCheckCodeModel = new AutoCheckCodeModel();
            // Implement the logic to read the Excel file and populate ioPointPositions
            // This is a placeholder for the actual implementation
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = ConfigPlcs.ConfigPath;
            }

            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException(nameof(filePath));
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using ExcelPackage package = new ExcelPackage(filePath, ApplicationConfigConst.Pwd);
            var sheet = package.Workbook.Worksheets[sheetName];

            var row = 2;

            try
            {
                var mainCodeCheck = sheet.Cells[row, 2].GetValue<string>() ?? string.Empty;
                autoCheckCodeModel.OpenCheckMainCode = mainCodeCheck.ToLower() == "open";
                row++;
                var mainCodeSum = sheet.Cells[row, 2].GetValue<string>() ?? string.Empty;
                autoCheckCodeModel.CheckMainCodeSum = int.Parse(mainCodeSum);
                row++;
                var partialCodeCheck = sheet.Cells[row, 2].GetValue<string>() ?? string.Empty;
                autoCheckCodeModel.OpenCheckPartialCode = partialCodeCheck.ToLower() == "open";
                row++;
                var partialCodeSum = sheet.Cells[row, 2].GetValue<string>() ?? string.Empty;
                autoCheckCodeModel.CheckPartialCodeSum = int.Parse(partialCodeSum);
                var partialCodePlcName = sheet.Cells[row, 3].GetValue<string>() ?? string.Empty;
                autoCheckCodeModel.CheckPartialCodePlcName = partialCodePlcName;
                var partialCodePlcDb = sheet.Cells[row, 4].GetValue<string>() ?? string.Empty;
                autoCheckCodeModel.CheckPartialCodeDbPosintion = partialCodePlcDb;
                row++;
                var workwearCodeCheck = sheet.Cells[row, 2].GetValue<string>() ?? string.Empty;
                autoCheckCodeModel.OpenWorkwearCode = workwearCodeCheck.ToLower() == "open";
                row++;
                var workwearCodeChangeParameter = sheet.Cells[row, 2].GetValue<string>() ?? string.Empty;
                autoCheckCodeModel.OpenWorkwearChangeParameter = workwearCodeChangeParameter.ToLower() == "open";
                row++;
                var selectStepCheckCodeOpen = sheet.Cells[row, 2].GetValue<string>() ?? string.Empty;
                autoCheckCodeModel.SelectStepCheckCodeOpen = selectStepCheckCodeOpen.ToLower() == "open";
                row++;
                var stepSleep = sheet.Cells[row, 2].GetValue<string>() ?? string.Empty;
                autoCheckCodeModel.StepSleep = stepSleep.ToLower() == "open";
                row++;
                var openPlotBindingCode = sheet.Cells[row, 2].GetValue<string>() ?? string.Empty;
                autoCheckCodeModel.OpenPlotBindingCode = openPlotBindingCode.ToLower() == "open";
            }
            catch (Exception ex)
            {
                XLogGlobal.Logger?.LogError($"{sheetName}读取Excel文件失败", ex);
            }
            
            return autoCheckCodeModel;
        }
    }
}