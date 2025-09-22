using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HandyControl.Controls;
using PressMachineMainModeules.Config;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.EntityFrameworkCore;
using PressMachineMainModeules.Utils;
using WPF.Admin.Models;
using WPF.Admin.Models.Models;
using XPrism.Core.Navigations;

namespace PressMachineMainModeules.ViewModels {
    internal partial class PreHisTableViewModel : BindableBase {
        [ObservableProperty] private string _title = nameof(PreHisTableViewModel);

        /// <summary>
        /// 存储所有数据项的集合
        /// </summary>
        private List<PreSaveModel>? _allItems;

        /// <summary>
        /// 搜索文本，用于过滤数据
        /// </summary>
        [ObservableProperty] private string _searchText = string.Empty;

        /// <summary>
        /// 当前页显示的数据集合
        /// </summary>
        [ObservableProperty] private ObservableCollection<PreSaveModel>? _currentPageData;

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
        public PreHisTableViewModel() {
            // 初始化数据
            _allItems = GenerateSampleData();
            SelectedPageSize = PageSizes[0]; // 默认每页10条
            UpdatePagingInfo();
            LoadCurrentPageData();
        }

        /// <summary>
        /// 当每页显示记录数改变时触发
        /// </summary>
        /// <param name="value">新的每页显示记录数</param>
        partial void OnSelectedPageSizeChanged(int value) {
            CurrentPage = 1; // 重置到第一页
            UpdatePagingInfo();
            LoadCurrentPageData();
        }

        /// <summary>
        /// 当搜索文本改变时触发
        /// </summary>
        /// <param name="value">新的搜索文本</param>
        partial void OnSearchTextChanged(string value) {
            CurrentPage = 1; // 重置到第一页
            UpdatePagingInfo();
            LoadCurrentPageData();
        }

        /// <summary>
        /// 执行搜索命令
        /// </summary>
        [RelayCommand]
        private void Search() {
            CurrentPage = 1;
            UpdatePagingInfo();
            LoadCurrentPageData();
        }

        /// <summary>
        /// 跳转到第一页命令
        /// </summary>
        [RelayCommand]
        private void FirstPage() {
            if (CurrentPage != 1)
            {
                CurrentPage = 1;
                LoadCurrentPageData();
            }
        }

        /// <summary>
        /// 跳转到上一页命令
        /// </summary>
        [RelayCommand]
        private void PreviousPage() {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                LoadCurrentPageData();
            }
        }

        /// <summary>
        /// 跳转到下一页命令
        /// </summary>
        [RelayCommand]
        private void NextPage() {
            if (CurrentPage >= TotalPages) return;
            CurrentPage++;
            LoadCurrentPageData();
        }

        /// <summary>
        /// 跳转到最后一页命令
        /// </summary>
        [RelayCommand]
        private void LastPage() {
            if (CurrentPage == TotalPages) return;
            CurrentPage = TotalPages;
            LoadCurrentPageData();
        }

        /// <summary>
        /// 更新分页信息，包括总记录数和总页数
        /// </summary>
        private void UpdatePagingInfo() {
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
        private void LoadCurrentPageData() {
            var filteredItems = GetFilteredItems();
            if (filteredItems == null) return;
            var pageData = filteredItems
                .Skip((CurrentPage - 1) * SelectedPageSize)
                .Take(SelectedPageSize)
                .ToList();

            CurrentPageData = new ObservableCollection<PreSaveModel>(pageData);
        }

        /// <summary>
        /// 根据搜索条件获取过滤后的数据
        /// </summary>
        /// <returns>过滤后的数据集合</returns>
        private List<PreSaveModel>? GetFilteredItems() {
            if (ApplicationAuthTaskFactory.AuthFlag)
            {
                throw new Exception("授权失败，无法读取数据");
            }
            if (string.IsNullOrWhiteSpace(SearchText))
                return _allItems;
            try
            {
                using var db = new PressMachineDataContext();
                var resultList = db.PreSaveModels .AsNoTracking() // 取消实体跟踪
                    .Where(e=> EF.Functions.Like(e.Code, $"%{SearchText}%"))
                    .Take(200)
                    .OrderByDescending(e => e.CreateTime)
                    .ToList();

                // 查询符合条件的数据 切忽略大小写
                return resultList?
                    .Where(x =>
                        x.Code.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            catch (Exception e)
            {
                Growl.Error(e.Message);
                return null;
            }
        }

        /// <summary>
        /// 生成示例数据
        /// </summary>
        /// <returns>包含示例数据的集合</returns>
        private static List<PreSaveModel> GenerateSampleData() {
            var items = new List<PreSaveModel>();


            return items;
        }

        /// <summary>
        /// 查看曲线
        /// </summary>
        [RelayCommand]
        private void GoToLook(PreSaveModel item) {
            if (string.IsNullOrWhiteSpace(item.PoltFilePath))
            {
                Growl.ErrorGlobal("记录中没有包含曲线文件");
                return;
            }

            if (!System.IO.File.Exists(item.PoltFilePath))
            {
                Growl.ErrorGlobal("曲线文件不存在，可能被用户移除");
                return;
            }


            WeakReferenceMessenger.Default.Send(new NavBarNavigationParameters {
                Key = "历史数据-曲线查看",
                Parameters = new NavigationParameters {
                    { "PlotFullFileName", item.PoltFilePath }
                    //{"PlotFullFileName","C:\\Users\\Administrator\\Desktop\\2024餐补统计1112月份(1).xlsx" }
                }
            });
            //Console.WriteLine(item);
        }
    }
}