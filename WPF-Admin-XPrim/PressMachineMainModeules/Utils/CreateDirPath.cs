using System.IO;
using WPF.Admin.Models.Models;

namespace PressMachineMainModeules.Utils
{
    public class CreateDirPath
    {
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="Exception"></exception>
        public static void CreateFolderIfNotExist(string path)
        {
            if (ApplicationAuthTaskFactory.AuthFlag)
            {
                throw new Exception("授权失败，无法探测文件");
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
