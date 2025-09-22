using System.Collections.ObjectModel;
using System.IO;
using OfficeOpenXml;
using PressMachineMainModeules.Config;
using PressMachineMainModeules.Models;
using WPF.Admin.Models.Models;
using WPF.Admin.Models.Utils;

namespace PressMachineMainModeules.Utils {
    public static class MesConfigExcelReader {
        public static AutoMesConfigModel ReadExcel(string? filePath = null, string sheetName = "MesConfig") {
            var autoMesConfigModel = new AutoMesConfigModel();
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
            var reader = true;

            var dataUploadOpen = sheet.Cells[row, 2].GetValue<string>() ?? string.Empty;
            var dataUploadSqlOpen = sheet.Cells[row, 3].GetValue<string>() ?? string.Empty;
            var dataUploadDbType = sheet.Cells[row, 4].GetValue<string>() ?? string.Empty;
            var dataUploadConnectString = sheet.Cells[row, 5].GetValue<string>() ?? string.Empty;
            var dataUploadSqlCommand = sheet.Cells[row, 6].GetValue<string>() ?? string.Empty;
            var dataUploadHttpOpen = sheet.Cells[row, 7].GetValue<string>() ?? string.Empty;
            var datauploadHttpUrl = sheet.Cells[row, 8].GetValue<string>() ?? string.Empty;
            var dataUploadHttpMethod = sheet.Cells[row, 9].GetValue<string>() ?? string.Empty;
            var dataUploadHttpBody = sheet.Cells[row, 10].GetValue<string>() ?? string.Empty;
            var dataUploadHttpHeader = sheet.Cells[row, 11].GetValue<string>() ?? string.Empty;
            var dataUploadAnalysis = sheet.Cells[row, 12].GetValue<string>() ?? string.Empty;

            if (dataUploadOpen.ToLower() == "true")
            {
                autoMesConfigModel.OpenAfterChecked = true;
                if (dataUploadSqlOpen.ToLower() == "true")
                {
                    autoMesConfigModel.AfterAutoMesMode = AutoMesAfterMode.Sql;
                    autoMesConfigModel.AfterAutoMesSql = new AutoMesSqlMode {
                        ConnectString = dataUploadConnectString,
                        SqlType = Enum.Parse<AutoMesSqlDbType>(dataUploadDbType),
                        SqlCommand = dataUploadSqlCommand
                    };
                }

                if (dataUploadHttpOpen.ToLower() == "true")
                {
                    autoMesConfigModel.AfterAutoMesMode = AutoMesAfterMode.Http;
                    autoMesConfigModel.AfterAutoMesHttp = new AutoMesHttpMode {
                        HttpMethod = dataUploadHttpMethod,
                        RequestUrl = datauploadHttpUrl,
                        RequestBody = dataUploadHttpBody,
                        HeadersString = dataUploadHttpHeader,
                        AnalysisString = dataUploadAnalysis
                    };
                }
            }

            row++;
            var dataCheckedOpen = sheet.Cells[row, 2].GetValue<string>() ?? string.Empty;
            var dataCheckedSqlOpen = sheet.Cells[row, 3].GetValue<string>() ?? string.Empty;
            var dataCheckedDbType = sheet.Cells[row, 4].GetValue<string>() ?? string.Empty;
            var dataCheckedConnectString = sheet.Cells[row, 5].GetValue<string>() ?? string.Empty;
            var dataCheckedSqlCommand = sheet.Cells[row, 6].GetValue<string>() ?? string.Empty;
            var dataCheckedHttpOpen = sheet.Cells[row, 7].GetValue<string>() ?? string.Empty;
            var dataCheckedHttpUrl = sheet.Cells[row, 8].GetValue<string>() ?? string.Empty;
            var dataCheckedHttpMethod = sheet.Cells[row, 9].GetValue<string>() ?? string.Empty;
            var dataCheckedHttpBody = sheet.Cells[row, 10].GetValue<string>() ?? string.Empty;
            var dataCheckedHttpHeader = sheet.Cells[row, 11].GetValue<string>() ?? string.Empty;
            var dataCheckedAnalysis = sheet.Cells[row, 12].GetValue<string>() ?? string.Empty;
            if (dataCheckedOpen.ToLower() == "true")
            {
                autoMesConfigModel.OpenBeforeChecked = true;
                autoMesConfigModel.BeforeAutoMesHttp = new AutoMesHttpMode {
                    HttpMethod = dataCheckedHttpMethod,
                    RequestUrl = dataCheckedHttpUrl,
                    RequestBody = dataCheckedHttpBody,
                    HeadersString = dataCheckedHttpHeader,
                    AnalysisString = dataCheckedAnalysis
                };
            }

            return autoMesConfigModel;
        }
    }
}