using OfficeOpenXml;
using PressMachineMainModeules.Models;
using System.Diagnostics;
using System.IO;
using WPF.Admin.Models.Models;

namespace PressMachineMainModeules.Utils
{
    public class DataExcelHelper : ExcelHelper
    {
        public static void Save(
            string filename,
            string productName,
            string code,
            Result ret,
            float inflectionPos,
            float inflectionPre,
            IList<Step> steps,
            IList<MonitorRec> recs,
            CurvePara curvePara,
            IList<Measurement> rawData,
            IList<Measurement> filteredData,
            double[] fittedTimes,
            double[] fittedPositions,
            double[] fittedPressures,
            string saveModel,
            PressMachineCoreParamsDa recipeDetail,
            float endPressure,
            float maxPressure,
            string password = null)
        {

            if (ApplicationAuthTaskFactory.AuthFlag)
            {
                throw new Exception("授权失败，无法保存数据");
            }

            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentException("File name cannot be null or empty.");
            }

            //string tempFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "Template.xls");

            //if (!File.Exists(tempFilename))
            //{
            //    throw new FileNotFoundException("Not found Template.xls file");
            //}

            //if (File.Exists(filename))
            //{
            //    File.Delete(filename);
            //}

            //File.Copy(tempFilename, filename,true);

            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            //ExcelPackage package = string.IsNullOrEmpty(password)
            //    ? new ExcelPackage(info)
            //    : new ExcelPackage(info, password);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new FileInfo(filename)))
            {
                ExcelWorksheet motionParaSheet = package.Workbook.Worksheets.Add("MotionPara");
                ExcelWorksheet monitorRecSheet = package.Workbook.Worksheets.Add("MonitorRec");
                ExcelWorksheet dataSheet = package.Workbook.Worksheets.Add("Data");
                ExcelWorksheet paraSheet = package.Workbook.Worksheets.Add("Para");
                ExcelWorksheet PlcParaSheet = package.Workbook.Worksheets.Add("PLCPara");

                if (steps != null && steps.Count > 0)
                {
                    motionParaSheet.Cells.Clear();
                    motionParaSheet.Cells[1, 1].Value = steps.Count;
                    for (int i = 0; i < steps.Count; i++)
                    {
                        int row = i + 2;
                        int col = 1;

                        motionParaSheet.Cells[row, col].Value = steps[i].Id;
                        col++;
                        motionParaSheet.Cells[row, col].Value = steps[i].Name;
                        col++;
                        motionParaSheet.Cells[row, col].Value = steps[i].StepType;
                        col++;
                        motionParaSheet.Cells[row, col].Value = steps[i].Value;
                        col++;
                        motionParaSheet.Cells[row, col].Value = steps[i].Speed;
                        col++;
                        motionParaSheet.Cells[row, col].Value = steps[i].Time;
                        col++;
                        motionParaSheet.Cells[row, col].Value = steps[i].ExceedValue;
                        col++;
                        motionParaSheet.Cells[row, col].Value = steps[i].AlarmReaction;
                        col++;
                        motionParaSheet.Cells[row, col].Value = steps[i].InternalPosition;
                    }
                }

                if (recs != null && recs.Count > 0)
                {
                    monitorRecSheet.Cells.Clear();
                    monitorRecSheet.Cells[1, 1].Value = recs.Count;
                    for (int i = 0; i < recs.Count; i++)
                    {
                        int row = i + 2;
                        int col = 1;

                        monitorRecSheet.Cells[row, col].Value = recs[i].Id;
                        col++;
                        monitorRecSheet.Cells[row, col].Value = recs[i].Name;
                        col++;
                        monitorRecSheet.Cells[row, col].Value = recs[i].MonitorType;
                        col++;
                        monitorRecSheet.Cells[row, col].Value = recs[i].MinX;
                        col++;
                        monitorRecSheet.Cells[row, col].Value = recs[i].MaxX;
                        col++;
                        monitorRecSheet.Cells[row, col].Value = recs[i].MinY;
                        col++;
                        monitorRecSheet.Cells[row, col].Value = recs[i].MaxY;
                        col++;
                        monitorRecSheet.Cells[row, col].Value = recs[i].AlarmReaction;
                        col++;
                        monitorRecSheet.Cells[row, col].Value = recs[i].Result;
                    }
                }

                if (curvePara != null)
                {
                    paraSheet.Cells[1, 2].Value = curvePara.MinX;
                    paraSheet.Cells[2, 2].Value = curvePara.MaxX;
                    paraSheet.Cells[3, 2].Value = curvePara.MinY;
                    paraSheet.Cells[4, 2].Value = curvePara.MaxY;
                    paraSheet.Cells[5, 2].Value = curvePara.InflexionMinX;
                    paraSheet.Cells[6, 2].Value = curvePara.InflexionMaxX;
                    paraSheet.Cells[7, 2].Value = curvePara.InflexionMinY;
                    paraSheet.Cells[8, 2].Value = curvePara.InflexionMaxY;
                }

                dataSheet.Cells[2, 2].Value = productName;
                dataSheet.Cells[3, 2].Value = code;
                dataSheet.Cells[4, 2].Value = ret;
                dataSheet.Cells[5, 2].Value = inflectionPos;
                dataSheet.Cells[6, 2].Value = inflectionPre;
                dataSheet.Cells[8, 9].Value = "时间";
                dataSheet.Cells[8, 10].Value = "位移";
                dataSheet.Cells[8, 11].Value = "压力";

                dataSheet.Cells[8, 13].Value = "日期时间";
                dataSheet.Cells[9, 13].Value = DateTime.Now.ToString();

                dataSheet.Cells[11, 13].Value = "最终压力";
                dataSheet.Cells[12, 13].Value = endPressure;

                dataSheet.Cells[14, 13].Value = "最大压力";
                dataSheet.Cells[15, 13].Value = maxPressure;

                if (rawData != null && rawData.Count > 0)
                {
                    dataSheet.Cells[1, 2].Value = rawData.Count;
                    DateTime startTime = rawData[0].TimeStamp;
                    dataSheet.Cells[7, 2].Value = startTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    int startRow = 9;
                    for (int i = 0; i < rawData.Count; i++)
                    {
                        int col = 1;
                        string timeSpan = (rawData[i].TimeStamp - startTime).TotalSeconds.ToString("F3");
                        dataSheet.Cells[i + startRow, col].Value = timeSpan;
                        col++;
                        dataSheet.Cells[i + startRow, col].Value = rawData[i].Position.ToString("F3");
                        col++;
                        dataSheet.Cells[i + startRow, col].Value = rawData[i].Pressure.ToString("F3");
                    }
                }

                if (filteredData != null && filteredData.Count > 0)
                {
                    dataSheet.Cells[1, 5].Value = filteredData.Count;
                    DateTime startTime = filteredData[0].TimeStamp;
                    int startRow = 9;
                    for (int i = 0; i < filteredData.Count; i++)
                    {
                        int col = 5;
                        string timeSpan = (filteredData[i].TimeStamp - startTime).TotalSeconds.ToString("F3");
                        dataSheet.Cells[i + startRow, col].Value = timeSpan;
                        col++;
                        dataSheet.Cells[i + startRow, col].Value = filteredData[i].Position.ToString("F3");
                        col++;
                        dataSheet.Cells[i + startRow, col].Value = filteredData[i].Pressure.ToString("F3");
                    }
                }

                if (fittedTimes != null
                    && fittedPositions != null
                    && fittedPressures != null
                    && fittedTimes.Length > 0
                    && fittedPositions.Length > 0
                    && fittedPressures.Length > 0)
                {
                    dataSheet.Cells[1, 9].Value = fittedTimes.Length;
                    int startRow = 9;
                    for (int i = 0; i < fittedTimes.Length; i++)
                    {
                        int col = 9;
                        dataSheet.Cells[i + startRow, col].Value = fittedTimes[i].ToString("F3");
                        col++;
                        dataSheet.Cells[i + startRow, col].Value = fittedPositions[i].ToString("F3");
                        col++;
                        dataSheet.Cells[i + startRow, col].Value = fittedPressures[i].ToString("F3");
                    }
                }

                //判断当前模式并保存当前PLC参数
                if (recipeDetail != null)
                {
                    if (saveModel == "位置模式")
                    {
                        //PlcParaSheet.Cells[1, 1].Value = "位置模式";

                        //PlcParaSheet.Cells[2, 2].Value = "待机位置：";
                        //PlcParaSheet.Cells[3, 2].Value = "返程速度：";
                        //PlcParaSheet.Cells[4, 2].Value = "保压时间：";
                        //PlcParaSheet.Cells[5, 2].Value = "第一位置：";
                        //PlcParaSheet.Cells[6, 2].Value = "第一速度：";
                        //PlcParaSheet.Cells[7, 2].Value = "第二位置：";
                        //PlcParaSheet.Cells[8, 2].Value = "第二速度：";
                        //PlcParaSheet.Cells[9, 2].Value = "第三位置：";
                        //PlcParaSheet.Cells[10, 2].Value = "第三速度：";

                        //PlcParaSheet.Cells[2, 3].Value = recipeDetail.StandByPosFirst.ToString();
                        //PlcParaSheet.Cells[3, 3].Value = recipeDetail.ReturnSpeedFirst.ToString();
                        //PlcParaSheet.Cells[4, 3].Value = recipeDetail.HoldTimeFirst.ToString();
                        //PlcParaSheet.Cells[5, 3].Value = recipeDetail.PreFirst1.ToString();
                        //PlcParaSheet.Cells[6, 3].Value = recipeDetail.SpeedFirst1.ToString();
                        //PlcParaSheet.Cells[7, 3].Value = recipeDetail.PreFirst2.ToString();
                        //PlcParaSheet.Cells[8, 3].Value = recipeDetail.SpeedFirst2.ToString();
                        //PlcParaSheet.Cells[9, 3].Value = recipeDetail.PreFirst3.ToString();
                        //PlcParaSheet.Cells[10, 3].Value = recipeDetail.SpeedFirst3.ToString();

                    }
                    else
                    {
                        //PlcParaSheet.Cells[1, 1].Value = "压力模式";

                        //PlcParaSheet.Cells[2, 2].Value = "待机位置：";
                        //PlcParaSheet.Cells[3, 2].Value = "返程速度：";
                        //PlcParaSheet.Cells[4, 2].Value = "保压时间：";
                        //PlcParaSheet.Cells[5, 2].Value = "工件位置：";
                        //PlcParaSheet.Cells[6, 2].Value = "工件速度：";
                        //PlcParaSheet.Cells[7, 2].Value = "保压位置：";
                        //PlcParaSheet.Cells[8, 2].Value = "第一压力：";
                        //PlcParaSheet.Cells[9, 2].Value = "第一速度：";
                        //PlcParaSheet.Cells[10, 2].Value = "第二压力：";
                        //PlcParaSheet.Cells[11, 2].Value = "第二速度：";
                        //PlcParaSheet.Cells[12, 2].Value = "第三压力：";
                        //PlcParaSheet.Cells[13, 2].Value = "第三速度：";
                        //PlcParaSheet.Cells[14, 2].Value = "压力上限：";
                        //PlcParaSheet.Cells[15, 2].Value = "压力下限：";
                        //PlcParaSheet.Cells[16, 2].Value = "压力启用状态：";

                        //PlcParaSheet.Cells[2, 3].Value = recipeDetail.StandByPosSecond.ToString();
                        //PlcParaSheet.Cells[3, 3].Value = recipeDetail.ReturnSpeedSecond.ToString();
                        //PlcParaSheet.Cells[4, 3].Value = recipeDetail.HoldTimeSecond.ToString();
                        //PlcParaSheet.Cells[5, 3].Value = recipeDetail.ProductPosSecond.ToString();
                        //PlcParaSheet.Cells[6, 3].Value = recipeDetail.ProductSpeedSecond.ToString();
                        //PlcParaSheet.Cells[7, 3].Value = recipeDetail.ProtectPosSecond.ToString();
                        //PlcParaSheet.Cells[8, 3].Value = recipeDetail.PreSecond1.ToString();
                        //PlcParaSheet.Cells[9, 3].Value = recipeDetail.SpeedSecond1.ToString();
                        //PlcParaSheet.Cells[10, 3].Value = recipeDetail.PreSecond2.ToString();
                        //PlcParaSheet.Cells[11, 3].Value = recipeDetail.SpeedSecond2.ToString();
                        //PlcParaSheet.Cells[12, 3].Value = recipeDetail.PreSecond3.ToString();
                        //PlcParaSheet.Cells[13, 3].Value = recipeDetail.SpeedSecond3.ToString();
                        //PlcParaSheet.Cells[14, 3].Value = recipeDetail.PressureCeiling.ToString();
                        //PlcParaSheet.Cells[15, 3].Value = recipeDetail.PressureLower.ToString();
                        //PlcParaSheet.Cells[16, 3].Value = recipeDetail.PressureJudgeState.ToString();

                    }
                }

                package.Save();
            }
        }

        //out string workModel
        public static void Read(
                string filename,
                out string productName,
                out string code,
                out Result ret,
                out float inflectionPos,
                out float inflectionPre,
                out List<MonitorRec> recs,
                out CurvePara curvePara,
                out double[] fittedTimes,
                out double[] fittedPositions,
                out double[] fittedPressures,

                string password = null)
        {
            productName = string.Empty;
            code = string.Empty;
            ret = Result.None;
            inflectionPos = float.MinValue;
            inflectionPre = float.MinValue;
            recs = new List<MonitorRec>();
            curvePara = new CurvePara();
            fittedTimes = new double[0];
            fittedPositions = new double[0];
            fittedPressures = new double[0];

            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentException("File name cannot be null or empty.");
            }
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            FileInfo info = new FileInfo(filename);
            ExcelPackage package = string.IsNullOrEmpty(password)
                ? new ExcelPackage(info)
                : new ExcelPackage(info, password);

            using (package)
            {
                ExcelWorksheet monitorRecSheet = package.Workbook.Worksheets["MonitorRec"];
                ExcelWorksheet dataSheet = package.Workbook.Worksheets["Data"];
                ExcelWorksheet paraSheet = package.Workbook.Worksheets["Para"];
                //ExcelWorksheet plcParaSheet = package.Workbook.Worksheets["PlcPara"];

                curvePara.MinX = paraSheet.GetValue<float>(1, 2);
                curvePara.MaxX = paraSheet.GetValue<float>(2, 2);
                curvePara.MinY = paraSheet.GetValue<float>(3, 2);
                curvePara.MaxY = paraSheet.GetValue<float>(4, 2);
                curvePara.InflexionMinX = paraSheet.GetValue<float>(5, 2);
                curvePara.InflexionMaxX = paraSheet.GetValue<float>(6, 2);
                curvePara.InflexionMinY = paraSheet.GetValue<float>(7, 2);
                curvePara.InflexionMaxY = paraSheet.GetValue<float>(8, 2);

                int recCount = monitorRecSheet.GetValue<int>(1, 1);
                for (int i = 0; i < recCount; i++)
                {
                    int col = 1;
                    int row = i + 2;
                    MonitorRec rec = new MonitorRec();
                    rec.Id = monitorRecSheet.GetValue<int>(row, col);
                    col++;
                    rec.Name = monitorRecSheet.GetValue<string>(row, col);

                    col++;
                    var monitorTypeStr = monitorRecSheet.GetValue<string>(row, col);
                    if (Enum.TryParse(monitorTypeStr, out MonitorType monitorType))
                    {
                        rec.MonitorType = monitorType;
                    }

                    col++;
                    rec.MinX = monitorRecSheet.GetValue<float>(row, col);
                    col++;
                    rec.MaxX = monitorRecSheet.GetValue<float>(row, col);
                    col++;
                    rec.MinY = monitorRecSheet.GetValue<float>(row, col);
                    col++;
                    rec.MaxY = monitorRecSheet.GetValue<float>(row, col);
                    col++;
                    var alarmReactionStr = monitorRecSheet.GetValue<string>(row, col);
                    if (Enum.TryParse(alarmReactionStr, out AlarmReaction alarmReaction))
                    {
                        rec.AlarmReaction = alarmReaction;
                    }

                    col++;
                    var resultStr = monitorRecSheet.GetValue<string>(row, col);
                    if (Enum.TryParse(resultStr, out Result result))
                    {
                        rec.Result = result;
                    }

                    recs.Add(rec);
                }

                productName = dataSheet.GetValue<string>(2, 2);
                code = dataSheet.GetValue<string>(3, 2);

                var totalResultStr = dataSheet.GetValue<string>(4, 2);
                if (Enum.TryParse(totalResultStr, out Result totalResult))
                {
                    ret = totalResult;
                }

                inflectionPos = dataSheet.GetValue<float>(5, 2);
                inflectionPre = dataSheet.GetValue<float>(6, 2);

                int fittedCount = dataSheet.GetValue<int>(1, 9);
                fittedTimes = new double[fittedCount];
                fittedPositions = new double[fittedCount];
                fittedPressures = new double[fittedCount];
                int startRow = 9;
                for (int i = 0; i < fittedCount; i++)
                {
                    int col = 9;
                    fittedTimes[i] = dataSheet.GetValue<double>(i + startRow, col);
                    col++;
                    fittedPositions[i] = dataSheet.GetValue<double>(i + startRow, col);
                    col++;
                    fittedPressures[i] = dataSheet.GetValue<double>(i + startRow, col);

                    Debug.WriteLine($"{i}  {fittedTimes[i]}  {fittedPositions[i]}  {fittedPressures[i]}");
                }

                //workModel = plcParaSheet.GetValue<string>(1, 1);
            }
        }
    }
}
