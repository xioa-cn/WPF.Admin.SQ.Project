using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using WPF.Admin.Models.Models;
using WPF.Admin.Service.Services;
using WPF.Admin.Themes.Converter;
using WPFAdmin.NavigationModules.Messengers;
using WPFAdmin.NavigationModules.ViewModel;

namespace WPFAdmin.NavigationModules.Components;

public partial class NaviControl : UserControl {
    public NaviControl() {
        InitializeComponent();
        WeakReferenceMessenger.Default.Register<NaviSendMessenger<TreeItemModel>>(
            this, ChangeIsChecked
        );
        if (OpenNavIndexHelper.NavIndexConfig.OpenIndex)
        {
            WeakReferenceMessenger.Default.Register<NavIndex>(this, (recipient, message) =>
            {
                if (message.OpenIndex)
                {
                    Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        DispatcherHelper.CheckBeginInvokeOnUI(NavToPage);
                    });
                }
                //WeakReferenceMessenger.Default.Unregister<NavIndex>(this);
            });
        }

        this.Loaded += NaviControl_Loaded;
        this.Unloaded += NaviControl_Unloaded;
    }

    private bool _isRegistered = false;

    private void NaviControl_Unloaded(object sender, RoutedEventArgs e) {
        if (_isRegistered)
        {
            WeakReferenceMessenger.Default.Unregister<NavBarNavigationParameters>(this);
            _isRegistered = false;
        }
    }

    private void NaviControl_Loaded(object sender, RoutedEventArgs e) {
        if (!_isRegistered)
        {
            WeakReferenceMessenger.Default.Register<NavBarNavigationParameters>(this, NaviParamerterPage);
            _isRegistered = true;
        }
    }

    private void NaviParamerterPage(object recipient, NavBarNavigationParameters message) {
        NavToPage(message);
    }

    private void ChangeIsChecked(object recipient, NaviSendMessenger<TreeItemModel> message) {
        if (_isUpdatingSelection) return;

        _isUpdatingSelection = true;
        try
        {
            // 清除之前的选中状态
            if (olditemModel != null)
            {
                olditemModel.IsChecked = false;
            }

            // 设置新的选中状态
            olditemModel = message.Model;
            if (olditemModel != null)
            {
                olditemModel.IsChecked = true;
            }
        }
        finally
        {
            _isUpdatingSelection = false;
        }
    }

    private bool _isCollapsed = false;
    private bool _isUpdatingSelection = false;
    private bool _isMouseOverPopup = false;
    public static TreeItemModel? olditemModel { get; set; }

    public void NavToPage() {
        if (_isUpdatingSelection) return;
        if (this.DataContext is MainViewModel vm)
        {
            if (vm.TreeItems is null || vm.TreeItems.Count == 0) return;
            var value = vm.TreeItems[0];
            _isUpdatingSelection = true;
            try
            {
                if (value.HasChildren && ActualWidth < 200)
                {
                    WeakReferenceMessenger.Default.Send(new ChangeMainBorderSizeMessanger());
                    value.IsExpanded = !value.IsExpanded;
                }

                // 如果是展开状态，处理子项的展开/折叠
                if (ActualWidth >= 200)
                {
                    if (value.HasChildren)
                    {
                        value.IsChecked = false;
                        if (olditemModel is not null)
                        {
                            olditemModel.IsChecked = true;
                        }

                        value.IsExpanded = !value.IsExpanded;
                        //e.Handled = true;
                        return;
                    }
                }

                // 清除之前的选中状态
                if (olditemModel != null)
                {
                    olditemModel.IsChecked = false;
                }

                // 设置新的选中状态
                //olditemModel = value;
                value.IsChecked = true;

                // 处理导航
                if (value.PageCanInterchange == PageCanInterchange.NonePage)
                {
                    var windowOpen = BreadCrumbBar.Items.FirstOrDefault(e => e.Key == value);
                    if (windowOpen.Value is not null)
                    {
                        windowOpen.Value.Activate();
                        windowOpen.Value.Focusable = true;
                        return;
                    }

                    // PageWindow pageWindow = new PageWindow(value);
                    // BreadCrumbBar.items.Add(value, pageWindow);
                    // pageWindow.Show();
                    return;
                }

                var page = new TreeItemModelMessenger() {
                    Item = value,
                    MessengerStatus = MessengerStatus.FromNavBarToPage,
                };
                WeakReferenceMessenger.Default.Send(page);
            }
            finally
            {
                _isUpdatingSelection = false;
            }
        }
    }

    public void NavToPage(NavBarNavigationParameters nav) {
        if (_isUpdatingSelection) return;
        if (this.DataContext is MainViewModel vm)
        {
            TreeItemModel? value = null;
            var findbase = vm.TreeItems;
            var keys = nav.GetNavKey();
            for (var i = 0; i < keys.Length; i++)
            {
                if (value != null && value.HasChildren)
                {
                    value = value.Children.First(e => e.Key == keys[i]);
                }
                else
                {
                    value = findbase.First(e => e.Key == keys[i]);
                }
            }

            if (value is null)
            {
                throw new Exception("没有找到路由");
            }

            if (string.IsNullOrWhiteSpace(value.Page))
            {
                throw new Exception("路由没有设置页面");
            }

            _isUpdatingSelection = true;
            try
            {
                if (value.HasChildren && ActualWidth < 200)
                {
                    WeakReferenceMessenger.Default.Send(new ChangeMainBorderSizeMessanger());
                    value.IsExpanded = !value.IsExpanded;
                }

                // 如果是展开状态，处理子项的展开/折叠
                if (ActualWidth >= 200)
                {
                    if (value.HasChildren)
                    {
                        value.IsChecked = false;
                        if (olditemModel is not null)
                        {
                            olditemModel.IsChecked = true;
                        }

                        value.IsExpanded = !value.IsExpanded;
                        //e.Handled = true;
                        return;
                    }
                }

                // 清除之前的选中状态
                if (olditemModel != null)
                {
                    olditemModel.IsChecked = false;
                }

                // 设置新的选中状态
                //olditemModel = value;
                value.IsChecked = true;

                // 处理导航
                if (value.PageCanInterchange == PageCanInterchange.NonePage)
                {
                    var windowOpen = BreadCrumbBar.Items.FirstOrDefault(e => e.Key == value);
                    if (windowOpen.Value is not null)
                    {
                        windowOpen.Value.Activate();
                        windowOpen.Value.Focusable = true;
                        return;
                    }

                    // PageWindow pageWindow = new PageWindow(value);
                    // BreadCrumbBar.items.Add(value, pageWindow);
                    // pageWindow.Show();
                    return;
                }

                var page = new TreeItemModelMessenger() {
                    Item = value,
                    MessengerStatus = MessengerStatus.FromNavBarToPage,
                    Parameters = nav.Parameters,
                };
                WeakReferenceMessenger.Default.Send(page);
            }
            finally
            {
                _isUpdatingSelection = false;
            }
        }
    }

    private void NavToPage_Click(object sender, RoutedEventArgs e) {
        if (_isUpdatingSelection) return;

        if (sender is RadioButton radioButton && radioButton.Tag is TreeItemModel value)
        {
            _isUpdatingSelection = true;
            try
            {
                if (value.HasChildren && ActualWidth < 200)
                {
                    WeakReferenceMessenger.Default.Send(new ChangeMainBorderSizeMessanger());
                    value.IsExpanded = !value.IsExpanded;
                    return;
                }

                // 如果是展开状态，处理子项的展开/折叠
                if (ActualWidth >= 200)
                {
                    if (value.HasChildren)
                    {
                        value.IsChecked = false;
                        if (olditemModel is not null)
                        {
                            olditemModel.IsChecked = true;
                        }

                        value.IsExpanded = !value.IsExpanded;
                        e.Handled = true;
                        return;
                    }
                }

                // 清除之前的选中状态
                if (olditemModel != null)
                {
                    olditemModel.IsChecked = false;
                }

                // 设置新的选中状态
                //olditemModel = value;
                value.IsChecked = true;

                // 处理导航
                if (value.PageCanInterchange == PageCanInterchange.NonePage)
                {
                    var windowOpen = BreadCrumbBar.Items.FirstOrDefault(e => e.Key == value);
                    if (windowOpen.Value is not null)
                    {
                        windowOpen.Value.Activate();
                        windowOpen.Value.Focusable = true;
                        return;
                    }

                    // PageWindow pageWindow = new PageWindow(value);
                    // BreadCrumbBar.items.Add(value, pageWindow);
                    // pageWindow.Show();
                    return;
                }

                var page = new TreeItemModelMessenger() {
                    Item = value,
                    MessengerStatus = MessengerStatus.FromNavBarToPage,
                };
                WeakReferenceMessenger.Default.Send(page);
            }
            finally
            {
                _isUpdatingSelection = false;
            }
        }
    }

    private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e) {
        ScrollViewer scrollviewer = sender as ScrollViewer;
        if (scrollviewer != null)
        {
            scrollviewer.ScrollToVerticalOffset(scrollviewer.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }

    private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e) {
        if (e.NewSize.Width < 200 && !_isCollapsed)
        {
            _isCollapsed = true;
            CollapseAllItems();
        }
        else if (e.NewSize.Width >= 200)
        {
            _isCollapsed = false;
        }
    }

    private void CollapseAllItems() {
        if (NaviTreeView.ItemsSource == null) return;

        foreach (var item in NaviTreeView.ItemsSource)
        {
            CollapseItem(item as TreeItemModel);
        }
    }

    private void CollapseItem(TreeItemModel item) {
        item.IsExpanded = false;
        foreach (var child in item.Children)
        {
            CollapseItem(child);
        }
    }

    private void Popup_MouseLeave(object sender, MouseEventArgs e) {
        _isMouseOverPopup = false;
        if (sender is FrameworkElement element && element.Parent is Popup popup)
        {
            // 使用Dispatcher延迟关闭，给一个小的延时让鼠标有时间移动到子菜单
            Dispatcher.BeginInvoke(new System.Action(() =>
            {
                if (!_isMouseOverPopup)
                {
                    popup.IsOpen = false;
                }
            }), System.Windows.Threading.DispatcherPriority.Input);
        }
    }

    private void Popup_MouseEnter(object sender, MouseEventArgs e) {
        _isMouseOverPopup = true;
    }
}