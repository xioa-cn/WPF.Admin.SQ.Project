using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.Exceptions;
using WPF.Admin.Service.Logger;

namespace WPF.Admin.Service.Utils {
    public class AnalysisMesDataHelper {
        public static object? AnalysisJsonProperty(string json, string property) {
            try
            {
                dynamic jsonObject = JObject.Parse(json);

                var properties = property.Split(":");
                object? result = new object();
                foreach (var item in properties)
                {
                    jsonObject = jsonObject[item];
                }

                return jsonObject;
            }
            catch (Exception e)
            {
                XLogGlobal.Logger?.LogError(e.Message, e);
                return null;
            }
        }

        /// <summary>
        /// 判断mes返回的json数据是否满足条件
        /// </summary>
        /// <param name="json">请求mes的反馈</param>
        /// <param name="rola">mes结果解析规则 如 request:result-string-OK </param>
        /// <returns>mes反馈能否继续结果</returns>
        /// <exception cref="Exception">置顶规则表格式不正确</exception>
        /// <exception cref="InvalidOperationException">未知类型</exception>
        public static bool MesResponseIsSuccess(string? json, string rola) {
            if (string.IsNullOrEmpty(json))
            {
                throw new Exception("mes返回结果为空");
            }

            var rolalist = rola.Split("-");
            if (rolalist.Length != 3)
            {
                throw new Exception("置顶规则表格式不正确");
            }

            var value = AnalysisJsonProperty(json, rolalist[0]);
            var type = rolalist[1];

            XLogGlobal.Logger?.LogInfo("Mes数据解析:" + json + "\n" + "目标值" + rola);

            if (type.ToLower() == "bool")
            {
                return (bool)(value ?? throw new InvalidOperationException()) == bool.Parse(rolalist[2]);
            }
            else if (type.ToLower() == "string")
            {
                var stringValue = value.ToString();
                return string.Equals(stringValue.ToLower().Trim(), rolalist[2].ToLower().Trim(),
                    StringComparison.OrdinalIgnoreCase);
            }
            else if (type.ToLower() == "number")
            {
                return (int)(value ?? throw new InvalidOperationException()) == int.Parse(rolalist[2]);
            }

            throw new Exception("未知类型");
        }
    }
}