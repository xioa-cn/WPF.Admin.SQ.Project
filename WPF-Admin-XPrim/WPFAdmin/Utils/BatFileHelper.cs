using System.IO;
using System.Windows;

namespace WPFAdmin.Utils
{
    public static class BatFileHelper
    {

        public static void CreateBatFile()
        {
            CreateBat("auc");
            CreateBat("cas");
            CreateBat("cms");
            CreateBat("rea");
            CreateBat("rpc");
            CreateBat("crs");
        }

        private static void CreateBat(string bat)
        {
            var auc = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
                $"WPFAdmin.{bat}.bat");
            if (System.IO.File.Exists(auc))
            {
                return;
            }
            var sri = Application.GetResourceStream(
                new Uri($"pack://application:,,,/WPFAdmin;component/WPFAdmin.{bat}.bat"));
            if (sri == null)
            {
                throw new Exception("Route file not found");
            }

            using StreamReader reader = new StreamReader(sri.Stream);
            System.IO.File.WriteAllText(auc, reader.ReadToEnd());
        }
    }
}