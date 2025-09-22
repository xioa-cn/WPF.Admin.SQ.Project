using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using PressMachineMainModeules.Interfaces;
using HandyControl.Controls;
using System.Windows.Threading;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Messaging;
using PressMachineMainModeules.Models;
using PressMachineMainModeules.ViewModels;
using WPF.Admin.Service.Services;
using WPF.Admin.Themes.Helper;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;

namespace PressMachineMainModeules.Views;

public partial class Home : Page, IPositionSwitchable {
    private int rotationState = 0; // 用于跟踪旋转状态
    private bool isAnimating = false; // 防止动画过程中重复点击
    private Border? currentLargeBorder = null; // 跟踪当前在大容器位置的Border
    private readonly Dictionary<string, Border> borderMap;

    [MethodImpl(MethodImplOptions.NoOptimization)]
    public Home() {
        InitializeComponent();
        currentLargeBorder = pre4; // 初始时pre4在大容器位置

        // 初始化Border映射
        borderMap = new Dictionary<string, Border> {
            { "pre1", pre1 },
            { "pre2", pre2 },
            { "pre3", pre3 },
            { "pre4", pre4 }
        };
        this.Loaded += Home_Loaded;


        this.Unloaded += Home_Unloaded;
    }

    private void Home_Unloaded(object sender, RoutedEventArgs e) {
        WeakReferenceMessenger.Default.Unregister<PressMachineGettingIndex>(this);
    }

    private bool isFirst = true;

    private void Home_Loaded(object sender, RoutedEventArgs e) {
        WeakReferenceMessenger.Default.Register<PressMachineGettingIndex>(this, ChangeIndexPolt);

        if (!isFirst) return;
        if (this.DataContext is not HomeViewModel vm) return;
        isFirst = false;
        vm.Init();
    }

    private void ChangeIndexPolt(object recipient, PressMachineGettingIndex message) {
        DispatcherHelper.CheckBeginInvokeOnUI(() => { SwitchPosition(message.Key); });
    }

    private void zx_PreView(object sender, RoutedEventArgs e) {
        SwitchPosition("pre1");
    }

    public bool SwitchToLargePosition(string borderName) {
        if (!borderMap.TryGetValue(borderName, out var targetBorder))
            return false;

        if (currentLargeBorder == null  )
            return false;

        if (targetBorder == currentLargeBorder)
            return true;

        // 获取位置信息
        var targetRow = Grid.GetRow(targetBorder);
        var targetColumn = Grid.GetColumn(targetBorder);
        var targetRowSpan = Grid.GetRowSpan(targetBorder);

        var largeRow = 0;
        var largeColumn = 2;
        var largeRowSpan = 2;

        // 创建动画
        var fadeOutAnimation = new DoubleAnimation {
            From = 1.0,
            To = 0.0,
            Duration = TimeSpan.FromMilliseconds(200),
            FillBehavior = FillBehavior.Stop
        };

        // 确保在UI线程中执行
        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
        {
            // 开始淡出动画
            targetBorder.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            currentLargeBorder.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            // 使用Dispatcher延迟执行位置交换
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                // 执行位置交换
                Grid.SetRow(targetBorder, largeRow);
                Grid.SetColumn(targetBorder, largeColumn);
                Grid.SetRowSpan(targetBorder, largeRowSpan);

                Grid.SetRow(currentLargeBorder, targetRow);
                Grid.SetColumn(currentLargeBorder, targetColumn);
                Grid.SetRowSpan(currentLargeBorder, 1);

                // 确保元素可见
                targetBorder.Opacity = 1;
                currentLargeBorder.Opacity = 1;

                // 更新当前大容器引用
                currentLargeBorder = targetBorder;
            }), DispatcherPriority.Render);
        }));
        SnackbarHelper.Show("加载成功");
        return true;
    }

    public string GetCurrentLargeBorderName() {
        return borderMap.FirstOrDefault(x => x.Value == currentLargeBorder).Key ?? "unknown";
    }


    public void SwitchPosition(string borderName) {
        bool success = this.SwitchToLargePosition(borderName);
        if (success)
        {
            // 切换成功
            string currentLarge = this.GetCurrentLargeBorderName();

            //Growl.SuccessGlobal($"Successfully switched. Current large border: {currentLarge}");
        }
        else
        {
            // 切换失败
            Growl.WarningGlobal("曲线视图切换失败");
        }
    }

    private void SwitchPosition_Click(object sender, RoutedEventArgs e) {
        return;
        var button = (Button)sender;
        var clickedBorder = FindParent<Border>(button);

        if (clickedBorder == null) return;

        // 找到对应的borderName
        var borderName = borderMap.FirstOrDefault(x => x.Value == clickedBorder).Key;
        if (borderName != null)
        {
            SwitchToLargePosition(borderName);
        }
    }

    // 辅助方法：查找父元素
    private static T? FindParent<T>(DependencyObject child) where T : DependencyObject {
        DependencyObject parentObject = VisualTreeHelper.GetParent(child);

        if (parentObject == null) return null;

        if (parentObject is T parent)
        {
            return parent;
        }
        else
        {
            return FindParent<T>(parentObject);
        }
    }

    private void PrintThisViewClick(object sender, RoutedEventArgs e)
    {
        
    }
}