using System.Windows.Controls;
using System.Windows.Input;

namespace WPF.Admin.Themes.I18n
{
    public partial class ChangeLangs : UserControl
    {
        public ChangeLangs()
        {
            InitializeComponent();
        }


        // 鼠标进入时显示Popup
        private void LangTextBlock_MouseEnter(object sender, MouseEventArgs e)
        {            
            langPopup.IsOpen = true;
        }

        // 鼠标离开时隐藏Popup
        private void LangTextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            //langPopup.IsOpen = false;
        }

        private void LangTextBlock_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
           
            langPopup.IsOpen = !langPopup.IsOpen;
        }

        private void ClosePopup(object sender, System.Windows.RoutedEventArgs e)
        {
            langPopup.IsOpen = !langPopup.IsOpen;
        }
    }
}