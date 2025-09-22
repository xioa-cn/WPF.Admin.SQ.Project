using System.IO;
using System.Windows;
using System.ComponentModel;
using System.IO.Compression;

namespace Xioa.Setup
{
    public class AppConfig
    {
        public static string AppName { get; set; } = "TimedShutdownToPreventInternalCompetition";
        public static string ExeName => $"{AppName}.exe";
    }

    public partial class MainWindow : Window
    {
        private BackgroundWorker _installWorker;

        public MainWindow()
        {
            InitializeComponent();
            InitializeInstallWorker();
            UpdateSpaceInfo();
            
            // 检查是否已安装并更新UI
            if (IsApplicationInstalled())
            {
                Title = $"{AppConfig.AppName} 程序维护";
                InstallButton.Content = "修复";
                UninstallButton.Visibility = Visibility.Visible;
            }
            else
            {
                Title = $" {AppConfig.AppName} 安装程序 ";
                InstallButton.Content = "安装";
                UninstallButton.Visibility = Visibility.Collapsed;
            }
        }

        private void InitializeInstallWorker()
        {
            _installWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            _installWorker.DoWork += InstallWorker_DoWork;
            _installWorker.ProgressChanged += InstallWorker_ProgressChanged;
            _installWorker.RunWorkerCompleted += InstallWorker_RunWorkerCompleted;
        }

        private void UpdateSpaceInfo()
        {
            try
            {
                // 获取嵌入资源的大小
                var resourceInfo = Application.GetResourceStream(
                    new Uri("pack://application:,,,/Xioa.Setup;component/Resources/app.zip"));
                double zipSize = resourceInfo.Stream.Length;
                
                // 假设解压后的大小约为压缩包的2倍（这是一个估计值，你可以根据实际情况调整）
                double estimatedSize = zipSize * 2;
                
                // 转换为MB并显示
                RequiredSpaceText.Text = $"所需空间: {(estimatedSize / 1024 / 1024):F2} MB";
                
                // 获取目标驱动器的可用空间
                string drivePath = Path.GetPathRoot(InstallPathTextBox.Text);
                if (drivePath != null)
                {
                    DriveInfo drive = new DriveInfo(drivePath);
                    double freeSpace = drive.AvailableFreeSpace;
                    AvailableSpaceText.Text = $"可用空间: {(freeSpace / 1024 / 1024 / 1024):F2} GB";
                }
            }
            catch (Exception ex)
            {
                RequiredSpaceText.Text = "所需空间: 计算错误";
                AvailableSpaceText.Text = "可用空间: 计算错误";
            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "选择安装路径",
                FileName = "选择文件夹",
                CheckFileExists = false,
                CheckPathExists = true,
                InitialDirectory = InstallPathTextBox.Text
            };

            if (dialog.ShowDialog() == true)
            {
                string selectedPath = Path.GetDirectoryName(dialog.FileName);
                InstallPathTextBox.Text = selectedPath;
                UpdateSpaceInfo(); // 更新空间信息
            }
        }

        private bool IsApplicationInstalled()
        {
            try
            {              
                // 检查注册表
                using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey($@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{AppConfig.AppName}"))
                {
                    if (key != null)
                    {
                        InstallPathTextBox.Text = key.GetValue("InstallLocation") as string;
                        return true;
                    }
                }

                // 检查常见的安装位置
                string[] commonPaths = new[]
                {
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), AppConfig.AppName),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), AppConfig.AppName),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), AppConfig.AppName)
                };

                foreach (var path in commonPaths)
                {
                    if (Directory.Exists(path) && File.Exists(Path.Combine(path, AppConfig.ExeName)))
                    {
                        return true;
                    }
                }

                // 检查开始菜单快捷方式（所有用户）
                string commonStartMenuPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu),
                    "Programs",
                    AppConfig.AppName,
                    $"{AppConfig.AppName}.lnk");

                if (File.Exists(commonStartMenuPath))
                {
                    return true;
                }

                // 检查公共桌面快捷方式
                string commonDesktopPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory),
                    $"{AppConfig.AppName}.lnk");

                if (File.Exists(commonDesktopPath))
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            // 首先检查是否已安装
            if (IsApplicationInstalled())
            {
                MessageBox.Show(
                    $"检测到系统中已经安装了{AppConfig.AppName}程序。\n请先卸载已有程序后再进行安装。",
                    "安装提示",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (Directory.Exists(InstallPathTextBox.Text) && 
                Directory.GetFiles(InstallPathTextBox.Text).Length > 0)
            {
                var result = MessageBox.Show(
                    "选择的文件夹不为空，是否继续？",
                    "警告",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                    return;
            }

            InstallButton.IsEnabled = false;
            UninstallButton.IsEnabled = false;
            InstallProgressBar.Visibility = Visibility.Visible;
            _installWorker.RunWorkerAsync(new InstallSettings
            {
                InstallPath = InstallPathTextBox.Text,
                CreateDesktopShortcut = CreateDesktopShortcutCheckBox.IsChecked ?? false,
                CreateStartMenuShortcut = CreateStartMenuShortcutCheckBox.IsChecked ?? false
            });
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (_installWorker.IsBusy)
            {
                _installWorker.CancelAsync();
            }
            else
            {
                Close();
            }
        }

        private void InstallWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var settings = (InstallSettings)e.Argument;
            
            // 1. 从嵌入资源中提取安装包
            _installWorker.ReportProgress(0, "正在准备安装...");
            
            // 创建临时文件来存储zip
            string tempZipPath = Path.GetTempFileName();
            try
            {
                // 从资源中提取zip文件
                using (Stream resourceStream = Application.GetResourceStream(
                    new Uri("pack://application:,,,/Xioa.Setup;component/Resources/app.zip")).Stream)
                using (FileStream fileStream = File.Create(tempZipPath))
                {
                    resourceStream.CopyTo(fileStream);
                }

                Directory.CreateDirectory(settings.InstallPath);
                ZipFile.ExtractToDirectory(tempZipPath, settings.InstallPath);
                _installWorker.ReportProgress(50);
            
            // 2. 创建快捷方式
            if (settings.CreateDesktopShortcut)
            {
                CreateShortcut(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{AppConfig.AppName}.lnk"),
                    Path.Combine(settings.InstallPath, "app",AppConfig.ExeName));
            }

            if (settings.CreateStartMenuShortcut)
            {
                string startMenuPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                    "Programs",
                    AppConfig.AppName);
                Directory.CreateDirectory(startMenuPath);
                CreateShortcut(
                    Path.Combine(startMenuPath, $"{AppConfig.AppName}.lnk"),
                    Path.Combine(settings.InstallPath, "app",AppConfig.ExeName));
            }

            // 3. 写入注册表信息
            try
            {
                using (var key = Microsoft.Win32.Registry.LocalMachine.CreateSubKey($@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{AppConfig.AppName}"))
                {
                    if (key != null)
                    {
                        key.SetValue("DisplayName", AppConfig.AppName);
                        key.SetValue("InstallLocation", settings.InstallPath);
                        key.SetValue("DisplayIcon", Path.Combine(settings.InstallPath, AppConfig.ExeName));
                        key.SetValue("UninstallString", Path.Combine(settings.InstallPath, "uninstall.exe"));
                        key.SetValue("Publisher", "Your Company Name");
                        key.SetValue("InstallDate", DateTime.Now.ToString("yyyyMMdd"));
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw new Exception("安装程序需要管理员权限才能继续。请以管理员身份运行安装程序。");
            }
            catch (Exception ex)
            {
                throw new Exception($"写入注册表信息时出错: {ex.Message}");
            }

            _installWorker.ReportProgress(100, "安装完成");
        }
        finally
        {
            // 清理临时文件
            if (File.Exists(tempZipPath))
            {
                try
                {
                    File.Delete(tempZipPath);
                }
                catch { }
            }
        }
    }

        private void InstallWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            InstallProgressBar.Value = e.ProgressPercentage;
            if (e.UserState is string message)
            {
                StatusText.Text = message;
            }
        }

        private void InstallWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                System.Windows.MessageBox.Show(
                    $"安装失败: {e.Error.Message}",
                    "错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            else if (!e.Cancelled)
            {
                System.Windows.MessageBox.Show(
                    "安装完成！",
                    "提示",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }

            Close();
        }

        private void CreateShortcut(string shortcutPath, string targetPath)
        {
            string psCommand = $@"$WshShell = New-Object -ComObject WScript.Shell; " +
                        $@"$Shortcut = $WshShell.CreateShortcut('{shortcutPath}'); " +
                        $@"$Shortcut.TargetPath = '{targetPath}'; " +
                        $@"$Shortcut.WorkingDirectory = '{Path.GetDirectoryName(targetPath)}'; " +
                        $@"$Shortcut.Description = '{AppConfig.AppName} Application'; " +
                        $@"$Shortcut.Save()";

            // 创建PowerShell进程
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{psCommand}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using (var process = System.Diagnostics.Process.Start(startInfo))
            {
                process?.WaitForExit();
            }
        }

        private void RepairApplication()
        {
            InstallButton.Content = "修复";
            Title = $"修复 {AppConfig.AppName}";
        }

        private void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                $"确定要卸载 {AppConfig.AppName} 吗？",
                "卸载确认",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
        
            if (result == MessageBoxResult.Yes)
            {
                InstallButton.IsEnabled = false;
                UninstallButton.IsEnabled = false;
                UninstallApplication();
                
                // 卸载完成后重置界面
                Title = $"安装 {AppConfig.AppName}";
                InstallButton.Content = "安装";
                InstallButton.IsEnabled = true;
                UninstallButton.Visibility = Visibility.Collapsed;
                
                // 清空并重置安装路径
                InstallPathTextBox.Text = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    AppConfig.AppName);
                
                // 重新计算空间信息
                UpdateSpaceInfo();
            }
        }

        private void UninstallApplication()
        {
            try
            {
                // 获取安装路径
                string installPath = "";
                using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey($@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{AppConfig.AppName}"))
                {
                    if (key != null)
                    {
                        installPath = key.GetValue("InstallLocation") as string;
                    }
                }

                if (!string.IsNullOrEmpty(installPath))
                {
                    // 删除开始菜单快捷方式
                    string startMenuPath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                        "Programs",
                        AppConfig.AppName);
                    if (Directory.Exists(startMenuPath))
                    {
                        Directory.Delete(startMenuPath, true);
                    }

                    // 删除桌面快捷方式
                    string desktopPath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                        $"{AppConfig.AppName}.lnk");
                    if (File.Exists(desktopPath))
                    {
                        File.Delete(desktopPath);
                    }

                    // 删除安装目录
                    if (Directory.Exists(installPath))
                    {
                        Directory.Delete(installPath, true);
                    }

                    // 删除注册表项
                    Microsoft.Win32.Registry.LocalMachine.DeleteSubKey(
                        $@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{AppConfig.AppName}",
                        false);

                    MessageBox.Show(
                        $"{AppConfig.AppName} 已成功卸载！",
                        "卸载完成",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"卸载过程中出现错误：{ex.Message}\n请尝试手动删除程序文件和注册表项。",
                    "卸载错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                
                // 发生错误时重新启用按钮
                InstallButton.IsEnabled = true;
                UninstallButton.IsEnabled = true;
            }
        }
    }

    public class InstallSettings
    {
        public string InstallPath { get; set; }
        public bool CreateDesktopShortcut { get; set; }
        public bool CreateStartMenuShortcut { get; set; }
    }
}


