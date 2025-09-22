using System.Collections.ObjectModel;
using WPF.Admin.Models;

namespace PressMachineCMS.Models 
{
    /// <summary>
    /// 路由配置数据模型
    /// </summary>
    public class RouterModel : BindableBase 
    {
        private string _name;
        private string _path;
        private string _component;
        private string _icon;
        private int _sort;
        private bool _showInMenu;
        private bool _requireAuth;
        private ObservableCollection<RouterModel>? _children;

        private int _loginAuth;
        public int LoginAuth
        {
            get => _loginAuth;
            set => SetProperty(ref _loginAuth, value);
        }

        /// <summary>
        /// 路由名称，用于显示在菜单中的文本
        /// </summary>
        public string Name {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        /// <summary>
        /// 路由路径，例如 "/dashboard"
        /// </summary>
        public string Path {
            get => _path;
            set => SetProperty(ref _path, value);
        }

        /// <summary>
        /// 组件路径，对应页面组件的位置
        /// </summary>
        public string Component {
            get => _component;
            set => SetProperty(ref _component, value);
        }

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string Icon {
            get => _icon;
            set => SetProperty(ref _icon, value);
        }

        /// <summary>
        /// 排序序号，决定菜单项的显示顺序
        /// </summary>
        public int Sort {
            get => _sort;
            set => SetProperty(ref _sort, value);
        }

        /// <summary>
        /// 是否在菜单中显示此路由
        /// </summary>
        public bool ShowInMenu {
            get => _showInMenu;
            set => SetProperty(ref _showInMenu, value);
        }

        /// <summary>
        /// 是否需要认证才能访问此路由
        /// </summary>
        public bool RequireAuth {
            get => _requireAuth;
            set => SetProperty(ref _requireAuth, value);
        }

        /// <summary>
        /// 子路由集合
        /// </summary>
        public ObservableCollection<RouterModel>? Children {
            get => _children ??= new ObservableCollection<RouterModel>();
            set => SetProperty(ref _children, value);
        }
    }
}