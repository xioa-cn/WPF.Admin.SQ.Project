using CMS.ReaderConfigLIbrary.Models;
using OfficeOpenXml;
using System.IO;
using WPF.Admin.Models.Utils;

namespace PressMachineMainModeules.Utils
{
    public class ConfigPlcsExcelReader
    {
        public static List<PlcViewSettingContent> ReadExcel(string? filePath = null, string sheetName = "IOPointPositions")
        {
            var plcs = new List<PlcViewSettingContent>();
            // Implement the logic to read the Excel file and populate ioPointPositions
            // This is a placeholder for the actual implementation
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
            var ioSheet = package.Workbook.Worksheets[sheetName];

            var row = 2;
            var reader = true;
            while (reader)
            {
                try
                {
                    var key = ioSheet.Cells[row, 1].GetValue<string>() ?? string.Empty;
                    if (string.IsNullOrEmpty(key))
                    {
                        reader = false;
                        continue;
                    }
                    var plctype = ioSheet.Cells[row, 2].GetValue<string>() ?? string.Empty;
                    var desc = ioSheet.Cells[row, 3].GetValue<string>() ?? string.Empty;
                    var ip = ioSheet.Cells[row, 4].GetValue<string>() ?? string.Empty;
                    var port = ioSheet.Cells[row, 5].GetValue<string>() ?? string.Empty;
                    var station = ioSheet.Cells[row, 6].GetValue<string>() ?? string.Empty;
                    var serialport = ioSheet.Cells[row, 7].GetValue<string>() ?? string.Empty;
                    var _baudRate = ioSheet.Cells[row, 8].GetValue<string>() ?? string.Empty;
                    var _dataBits = ioSheet.Cells[row, 9].GetValue<string>() ?? string.Empty;
                    var _stopBits = ioSheet.Cells[row, 10].GetValue<string>() ?? string.Empty;
                    var _parity = ioSheet.Cells[row, 11].GetValue<string>() ?? string.Empty;
                    var _havingHeartbeat = ioSheet.Cells[row, 12].GetValue<string>() ?? string.Empty;
                    var _heartbeatInterval = ioSheet.Cells[row, 13].GetValue<string>() ?? string.Empty;
                    var _heartbeatValue = ioSheet.Cells[row, 14].GetValue<string>() ?? string.Empty;
                    var _heartbeatPoint = ioSheet.Cells[row, 15].GetValue<string>() ?? string.Empty;
                    var _heartbeatType = ioSheet.Cells[row, 16].GetValue<string>() ?? string.Empty;

                    row++;
                    if (desc.Contains("备用"))
                    {
                        continue;
                    }
                    if(!Enum.TryParse(_heartbeatType, out HeartbeatType heartbeatType)
                            )
                    {
                        throw new ArgumentException("Invalid HeartbeatType value");
                    }
                    var temp = new PlcViewSettingContent
                    {
                        Key = key,
                        Type = plctype switch
                        {
                            "Melsec_MC_Binary" => PlcType.Melsec_MC_Binary,
                            "Siemens_S7_S1200" => PlcType.Siemens_S7_S1200,
                            "Siemens_S7_S1500" => PlcType.Siemens_S7_S1500,
                            "Modbus_TCP" => PlcType.Modbus_TCP,
                            "Modbus_RTU" => PlcType.Modbus_RTU,
                            "Inovance_TcpNet" => PlcType.Inovance_TcpNet,
                            "Inovance_TcpNet_AM" => PlcType.Inovance_TcpNet_AM,
                            "Omron_FinsTcp" => PlcType.Omron_FinsTcp,
                            _ => throw new ArgumentException("Unknown PLC type")
                        },
                        Ip = ip,
                        Desc = desc,
                        Port = int.TryParse(port, out var p) ? p : 502,
                        Station = byte.TryParse(station, out var s) ? s : (byte)1,
                        SerialPort = serialport,
                        BaudRate = int.TryParse(_baudRate, out var br) ? br : 9600,
                        DataBits = int.TryParse(_dataBits, out var db) ? db : 8,
                        StopBits = int.TryParse(_stopBits, out var sb) ? sb : 1,
                        Parity = int.TryParse(_parity, out var pty) ? pty : 0,
                        HavingHeartbeat = _havingHeartbeat == "True" || _havingHeartbeat == "true",
                        HeartbeatInterval = int.TryParse(_heartbeatInterval, out var hi) ? hi : 500,
                        HeartbeatValue = _heartbeatValue ?? string.Empty,
                        HeartbeatPoint = _heartbeatPoint ?? string.Empty,
                        HeartbeatType = heartbeatType

                    };
                    plcs.Add(temp);

                }
                catch
                {
                    reader = false;
                    continue;
                }

            }


            return plcs;
        }
    }
}
