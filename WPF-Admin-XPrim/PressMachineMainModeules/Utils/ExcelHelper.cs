using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF.Admin.Models.Models;

namespace PressMachineMainModeules.Utils
{
    public class ExcelHelper
    {
        public static void Create(string filename, List<string> sheetNames = null, string password = null)
        {
            if (ApplicationAuthTaskFactory.AuthFlag)
            {
                throw new Exception("授权失败，无法保存数据");
            }
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentException("File name cannot be null or empty.");
            }

            FileInfo info = new FileInfo(filename);
            using (ExcelPackage package = new ExcelPackage(info))
            {

                foreach (var sheet in package.Workbook.Worksheets)
                {
                    package.Workbook.Worksheets.Delete(sheet);
                }

                if (sheetNames != null && sheetNames.Count > 0)
                {
                    foreach (var sheetName in sheetNames)
                    {
                        package.Workbook.Worksheets.Add(sheetName);
                    }
                }

                if (string.IsNullOrEmpty(password))
                {
                    package.Save();
                }
                else
                {
                    package.Save(password);
                }
            }
        }
    }
}
