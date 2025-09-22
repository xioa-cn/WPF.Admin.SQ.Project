using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using System.IO;
using System.Windows;
using WPF.Admin.Models;

namespace CMS.AppRouters.ViewModels
{
    public partial class RoutersSettingsViewModel : BindableBase
    {
        public List<string> Routers { get; set; } = [
            ""
        ];

        [ObservableProperty] private string _RouterText;

        public RoutersSettingsViewModel()
        {
            RouterText = """
                         {
                           "Routers": [
                            
                           ]
                         }
                         """;
            RouterText =
                System.IO.File.ReadAllText(path);
        }
        private string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "router.json");
        [RelayCommand]
        private void Save()
        {
            var result = HandyControl.Controls.MessageBox.Show("是否同时保存到sln源代码和程序配置中？", "提示", MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Cancel)
            {
                return;
            }
            try
            {

                SaveDebugSettings(this.RouterText);

                if (result == MessageBoxResult.Yes)
                {
                    SaveSlnSolution(this.RouterText);
                }
                Growl.SuccessGlobal("保存成功");
            }
            catch (Exception e)
            {
                HandyControl.Controls.MessageBox.Show(e.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveSlnSolution(string routerText)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo path = new DirectoryInfo(currentPath);
            var dir = string.Empty;
            if (path.Parent.Parent.FullName.EndsWith("WPFAdmin"))
            {
                dir = path.Parent.Parent.FullName;
            }
            else if (path.Parent.Parent.Parent.FullName.EndsWith("WPFAdmin"))
            {
                dir = path.Parent.Parent.Parent.FullName;
            }
            else
            {
                HandyControl.Controls.MessageBox.Show("没有找到源代码");
                return;
            }

            var slnFilePath = System.IO.Path.Combine(dir, "router.json");

            if (System.IO.File.Exists(slnFilePath))
            {
                System.IO.File.WriteAllText(slnFilePath, routerText);

            }
            else
            {
                HandyControl.Controls.MessageBox.Show("没有找到源配置");
                return;
            }

        }

        private void SaveDebugSettings(string routerText)
        {
            System.IO.File.WriteAllText(path, routerText);
        }
    }
}