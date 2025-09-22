
using PressMachineMainModeules.Models;
using System.Windows;
using HandyControl.Controls;
using PressMachineMainModeules.Config;
using WPF.Admin.Themes.Controls;

namespace PressMachineMainModeules.Components
{
    /// <summary>
    /// AutoParametersContentView.xaml 的交互逻辑
    /// </summary>
    public partial class AutoParametersContentView : System.Windows.Controls.UserControl
    {
        public static readonly DependencyProperty TitleDaProperty =
          DependencyProperty.Register(
              "Title",
              typeof(string), typeof(AutoParametersContentView),
              new FrameworkPropertyMetadata("参数",
                  new PropertyChangedCallback(OnTitleDaChanged)
                  )
              { BindsTwoWayByDefault = true }
              );

        private static void OnTitleDaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AutoParametersContentView)d).Title = (string)e.NewValue!;
        }


        public string? Title
        {
            get { return (string)GetValue(TitleDaProperty); }
            set { SetValue(TitleDaProperty, value); }
        }


        public static readonly DependencyProperty AutoModeProperty = 
            ElementBase.Property<AutoParametersContentView,string>(nameof(AutoModeProperty),"");

        public string AutoMode
        {
            get => (string)GetValue(AutoModeProperty);
            set => SetValue(AutoModeProperty, value);
        }

        public AutoParametersContentView()
        {
            InitializeComponent();
        }


        public static readonly DependencyProperty ContentParameterProperty =
          DependencyProperty.Register(
              "ContentParameter",
              typeof(AutoParametersInstance), typeof(AutoParametersContentView),
              new FrameworkPropertyMetadata(null,
                  new PropertyChangedCallback(OnContent1Changed)
                  )
              { BindsTwoWayByDefault = true }
              );

        private static void OnContent1Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AutoParametersContentView)d).ContentParameter = (AutoParametersInstance)e.NewValue!;
        }


        public AutoParametersInstance? ContentParameter
        {
            get { return (AutoParametersInstance?)GetValue(ContentParameterProperty); }
            set { SetValue(ContentParameterProperty, value); }
        }



        private void AddWindow_Click(object sender, RoutedEventArgs e)
        {
            // 添加新记录
            this.ContentParameter.MonitorRecs.Add(new MonitorRec { Id = ContentParameter.MonitorRecs.Count + 1 });
            ChangeId();
        }

        private void InsertWindow_Click(object sender, RoutedEventArgs e)
        {
            // 插入新记录到选中行的上方
            if (MonitorRec.SelectedItem is MonitorRec selectedRecord)
            {
                int index = ContentParameter.MonitorRecs.IndexOf(selectedRecord);
                ContentParameter.MonitorRecs.Insert(index, new MonitorRec { Id = ContentParameter.MonitorRecs.Count + 1 });
            }
            else
            {
                Growl.WarningGlobal(Common.t("Msg.ParameterContent.Insertion"));
            }
            ChangeId();
        }

        private void DeleteWindow_Click(object sender, RoutedEventArgs e)
        {
            // 删除选中行
            if (MonitorRec.SelectedItem is MonitorRec selectedRecord)
            {
                ContentParameter.MonitorRecs.Remove(selectedRecord);
            }
            else
            {
                Growl.WarningGlobal(Common.t("Msg.ParameterContent.Delete"));
            }
            ChangeId();
        }

        private void ClearWindow_Click(object sender, RoutedEventArgs e)
        {
            // 清空所有记录
            ContentParameter.MonitorRecs.Clear();
        }

        private void ChangeId()
        {
            Thread.Sleep(10);
            for (int i = 0; i < ContentParameter.MonitorRecs.Count; i++)
            {
                ContentParameter.MonitorRecs[i].Id = i + 1;
            }
        }
    }
}
