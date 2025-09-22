using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using PressMachineCMS.Models;
using WPF.Admin.Models;
using System.IO;
using System.Text.Json;
using System.Windows;
using HandyControl.Controls;
using MessageBox = System.Windows.MessageBox;

namespace PressMachineCMS.ViewModels
{
    public class RouterSettingsViewModel : BindableBase
    {
        private ObservableCollection<RouterModel>? _routers;
        private RouterModel? _selectedRouter;

        public RouterSettingsViewModel()
        {
            Routers = new ObservableCollection<RouterModel>();
        }

        public ObservableCollection<RouterModel>? Routers
        {
            get => _routers;
            set => SetProperty(ref _routers, value);
        }

      

        private RelayCommand? _addRootRouterCommand;
        public ICommand AddRootRouterCommand => _addRootRouterCommand ??= new RelayCommand(AddRootRouter);

        private RelayCommand? _addChildRouterCommand;
        public ICommand AddChildRouterCommand => _addChildRouterCommand ??= new RelayCommand(AddChildRouter, CanAddChildRouter);

        private RelayCommand? _deleteRouterCommand;
        public ICommand DeleteRouterCommand => _deleteRouterCommand ??= new RelayCommand(DeleteRouter, CanDeleteRouter);

        private RelayCommand? _saveRouterCommand;
        public ICommand SaveRouterCommand => _saveRouterCommand ??= new RelayCommand(SaveRouter);

        private void AddRootRouter()
        {
            var newRouter = CreateNewRouter();
            Routers?.Add(newRouter);
            SelectedRouter = newRouter;
        }

        public RouterModel? SelectedRouter
        {
            get => _selectedRouter;
            set
            {
                SetProperty(ref _selectedRouter, value);
                _addChildRouterCommand?.NotifyCanExecuteChanged();
                _deleteRouterCommand?.NotifyCanExecuteChanged();
                _saveRouterCommand?.NotifyCanExecuteChanged();
            }
        }

        private void AddChildRouter() {
            if (SelectedRouter == null)
            {
                return;
            }

            var newRouter = CreateNewRouter();
            SelectedRouter.Children?.Add(newRouter);
            SelectedRouter = newRouter;
        }

        private bool CanAddChildRouter()
        {
            return SelectedRouter != null;
        }

        private RouterModel CreateNewRouter()
        {
            return new RouterModel
            {
                Name = "新路由",
                Path = "/",
                ShowInMenu = true,
                RequireAuth = true,
                Sort = GetNextSort()
            };
        }

        private int GetNextSort()
        {
            if (SelectedRouter != null && SelectedRouter.Children != null)
            {
                return SelectedRouter.Children.Count + 1;
            }

            if (Routers is null) return -1;
            return Routers.Count + 1;
        }

        private void DeleteRouter()
        {
            if (SelectedRouter == null) return;

            RemoveFromCollection(Routers);
            SelectedRouter = null;
            return;

            bool RemoveFromCollection(ObservableCollection<RouterModel>? collection) {
                if (collection is null) return false;

                if (!collection.Contains(SelectedRouter))
                {
                    return collection.Any(router => RemoveFromCollection(router.Children));
                }

                collection.Remove(SelectedRouter);
                return true;

            }
        }

        private bool CanDeleteRouter()
        {
            return SelectedRouter != null;
        }

        private void SaveRouter()
        {
            try
            {
                var routerConfig = new RouterConfig
                {
                    Routers = ConvertToJsonModel(Routers)
                };

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                
                string jsonString = JsonSerializer.Serialize(routerConfig, options);

                if (_OpenFileJsonPath is null)
                {
                    Growl.ErrorGlobal("请先选择一个文件路径");
                    return;
                }
                File.WriteAllText(_OpenFileJsonPath,jsonString);
                
                Growl.SuccessGlobal("临时保存成功！！");
            }
            catch (Exception ex)
            {
                Growl.ErrorGlobal($"保存失败：{ex.Message}");
            }
        }

        private List<RouterJsonModel> ConvertToJsonModel(ObservableCollection<RouterModel>? routers)
        {
            var result = new List<RouterJsonModel>();
            if (routers == null)
            {
                return result;
            }

            foreach (var router in routers)
            {
                var jsonModel = new RouterJsonModel {
                    Key = router.Name,
                    Content = router.Name,
                    Icon = router.Icon,
                    Page = router.Component
                };

                if (router.RequireAuth)
                {
                    jsonModel.LoginAuth = router.LoginAuth; // 默认权限级别，可以根据需要修改
                }

                if (router.Children != null && router.Children.Count > 0)
                {
                    jsonModel.Children = ConvertToJsonModel(router.Children);
                }

                result.Add(jsonModel);
            }

            return result;
        }

        private RelayCommand? _loadRouterCommand;
        public ICommand LoadRouterCommand => _loadRouterCommand ??= new RelayCommand(LoadRouters);

        private string? _OpenFileJsonPath;
        
        private void LoadRouters()
        {
            try
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "JSON 文件|*.json|所有文件|*.*",
                    Title = "选择路由配置文件"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    string jsonString = File.ReadAllText(openFileDialog.FileName);
                    _OpenFileJsonPath = openFileDialog.FileName;
                    var routerConfig = JsonSerializer.Deserialize<RouterConfig>(jsonString);
                    
                    Routers?.Clear();
                    if (routerConfig?.Routers != null)
                    {
                        foreach (var jsonRouter in routerConfig.Routers)
                        {
                            Routers?.Add(ConvertFromJsonModel(jsonRouter));
                        }
                    }

                    MessageBox.Show("加载成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private RouterModel ConvertFromJsonModel(RouterJsonModel jsonModel)
        {
            var router = new RouterModel
            {
                Name = jsonModel.Key,
                Path = $"/{jsonModel.Page}",
                Component = jsonModel.Page,
                Icon = jsonModel.Icon,
                ShowInMenu = true,
                RequireAuth = jsonModel.LoginAuth.HasValue,
                LoginAuth = jsonModel.LoginAuth ?? 0,
            };

            foreach (var child in jsonModel.Children)
            {
                router.Children?.Add(ConvertFromJsonModel(child));
            }

            return router;
        }
    }
}