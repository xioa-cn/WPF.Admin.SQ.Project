using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using WPF.Admin.Models;
using WPF.Admin.Models.Db;
using WPF.Admin.Models.Models;
using WPF.Admin.Service.Services;
using WPF.Admin.Themes.I18n;

namespace SystemModules.ViewModels
{
    public partial class SystemUserViewModel : BindableBase
    {
        /// <summary>
        /// 存储所有数据项的集合
        /// </summary>
        private List<LoginUser>? _allItems;

        /// <summary>
        /// 搜索文本，用于过滤数据
        /// </summary>
        [ObservableProperty] private string _searchText = string.Empty;

        /// <summary>
        /// 当前页显示的数据集合
        /// </summary>
        [ObservableProperty] private ObservableCollection<LoginUser>? _currentPageData;

        /// <summary>
        /// 当前页码，从1开始
        /// </summary>
        [ObservableProperty] private int _currentPage = 1;

        /// <summary>
        /// 总页数
        /// </summary>
        [ObservableProperty] private int _totalPages;

        /// <summary>
        /// 总记录数
        /// </summary>
        [ObservableProperty] private int _totalItems;

        /// <summary>
        /// 当前选择的每页显示记录数
        /// </summary>
        [ObservableProperty] private int _selectedPageSize;

        /// <summary>
        /// 可选的每页显示记录数列表
        /// </summary>
        public List<int> PageSizes { get; } = new List<int> { 10, 20, 50, 100 };

        /// <summary>
        /// 构造函数，初始化数据和默认设置
        /// </summary>
        public SystemUserViewModel()
        {
            (t, _) = CSharpI18n.UseI18n();
            // 初始化数据
            Task.Run(() =>
            {
                _allItems = GenerateSampleData();
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    SelectedPageSize = PageSizes[0]; // 默认每页10条
                    UpdatePagingInfo();
                    LoadCurrentPageData();
                });
            });
        }

        /// <summary>
        /// 当每页显示记录数改变时触发
        /// </summary>
        /// <param name="value">新的每页显示记录数</param>
        partial void OnSelectedPageSizeChanged(int value)
        {
            CurrentPage = 1; // 重置到第一页
            UpdatePagingInfo();
            LoadCurrentPageData();
        }

        /// <summary>
        /// 当搜索文本改变时触发
        /// </summary>
        /// <param name="value">新的搜索文本</param>
        partial void OnSearchTextChanged(string value)
        {
            CurrentPage = 1; // 重置到第一页
            UpdatePagingInfo();
            LoadCurrentPageData();
        }

        /// <summary>
        /// 执行搜索命令
        /// </summary>
        [RelayCommand]
        private void Search()
        {
            CurrentPage = 1;
            UpdatePagingInfo();
            LoadCurrentPageData();
        }

        /// <summary>
        /// 跳转到第一页命令
        /// </summary>
        [RelayCommand]
        private void FirstPage()
        {
            if (CurrentPage == 1)
            {
                return;
            }

            CurrentPage = 1;
            LoadCurrentPageData();
        }

        /// <summary>
        /// 跳转到上一页命令
        /// </summary>
        [RelayCommand]
        private void PreviousPage()
        {
            if (CurrentPage <= 1)
            {
                return;
            }

            CurrentPage--;
            LoadCurrentPageData();
        }

        /// <summary>
        /// 跳转到下一页命令
        /// </summary>
        [RelayCommand]
        private void NextPage()
        {
            if (CurrentPage >= TotalPages) return;
            CurrentPage++;
            LoadCurrentPageData();
        }

        /// <summary>
        /// 跳转到最后一页命令
        /// </summary>
        [RelayCommand]
        private void LastPage()
        {
            if (CurrentPage == TotalPages) return;
            CurrentPage = TotalPages;
            LoadCurrentPageData();
        }

        /// <summary>
        /// 更新分页信息，包括总记录数和总页数
        /// </summary>
        private void UpdatePagingInfo()
        {
            var filteredItems = GetFilteredItems();
            if (filteredItems != null) TotalItems = filteredItems.Count;
            TotalPages = (int)Math.Ceiling(TotalItems / (double)SelectedPageSize);

            // 确保当前页不超过总页数
            if (CurrentPage > TotalPages)
            {
                CurrentPage = Math.Max(1, TotalPages);
            }
        }

        /// <summary>
        /// 加载当前页的数据
        /// </summary>
        private void LoadCurrentPageData()
        {
            var filteredItems = GetFilteredItems();
            if (filteredItems == null) return;
            var pageData = filteredItems
                .Skip((CurrentPage - 1) * SelectedPageSize)
                .Take(SelectedPageSize)
                .ToList();
            CurrentPageData ??= new ObservableCollection<LoginUser>();

            CurrentPageData.Clear();
            foreach (var item in pageData)
            {
                CurrentPageData.Add(item);
            }
        }

        /// <summary>
        /// 根据搜索条件获取过滤后的数据
        /// </summary>
        /// <returns>过滤后的数据集合</returns>
        private List<LoginUser>? GetFilteredItems()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return _allItems;

            // 查询符合条件的数据，忽略大小写
            return _allItems?
                .Where(x =>
                    x.UserName.Contains(SearchText))
                .ToList();
        }

        /// <summary>
        /// 生成示例数据
        /// </summary>
        /// <returns>包含示例数据的集合</returns>
        private static List<LoginUser> GenerateSampleData()
        {
            using var db = new SysDbContent();
            var items = db.LoginUsers
                .OrderBy(x => x.LoginAuth)
                .ThenBy(x => x.CreateTime)
                .ToList();

            return items;
        }
    }
}