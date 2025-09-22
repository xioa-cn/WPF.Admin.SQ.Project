using System.Windows;
using System.Windows.Media;

namespace WPF.Admin.Themes.Helper
{
    public class VisualHelper
    {
        // 视觉树查找父级控件的辅助方法
        public static T? FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            // 逐级向上查找父级
            var parentObject = VisualTreeHelper.GetParent(child);
    
            // 如果找到父级为T类型，返回
            if (parentObject is T parent)
                return parent;
    
            // 递归查找直到找到或到达根节点
            return parentObject != null ? FindVisualParent<T>(parentObject) : null;
        }
    }
}