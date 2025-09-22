using PressMachineMainModeules.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace PressMachineMainModeules.Components
{
    /// <summary>
    /// PressMachineParamstersView.xaml 的交互逻辑
    /// </summary>
    public partial class PressMachineParamstersView : UserControl
    {
        public static readonly DependencyProperty TitleDaProperty =
          DependencyProperty.Register(
              "Title",
              typeof(string), typeof(PressMachineParamstersView),
              new FrameworkPropertyMetadata("参数",
                  new PropertyChangedCallback(OnTitleDaChanged)
                  )
              { BindsTwoWayByDefault = true }
              );

        private static void OnTitleDaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PressMachineParamstersView)d).Title = (string)e.NewValue!;
        }


        public string? Title
        {
            get { return (string)GetValue(TitleDaProperty); }
            set { SetValue(TitleDaProperty, value); }
        }


        #region 电缸PLC参数

        public static readonly DependencyProperty PressMachineParamsDaProperty =
            DependencyProperty.Register(
                "PressMachineParamsDa",
                typeof(PressMachineParamsDa), typeof(PressMachineParamstersView),
                new FrameworkPropertyMetadata(new PressMachineParamsDa(),
                    new PropertyChangedCallback(OnPloModelControlDtChanged)
                    )
                { BindsTwoWayByDefault = true }
                );

        private static void OnPloModelControlDtChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PressMachineParamstersView)d).PressMachineParamsDa = (PressMachineParamsDa)e.NewValue!;
        }

        public PressMachineParamsDa PressMachineParamsDa
        {
            get { return (PressMachineParamsDa)GetValue(PressMachineParamsDaProperty); }
            set { SetValue(PressMachineParamsDaProperty, value); }
        }

        #endregion

        #region 判断参数

        public static readonly DependencyProperty MonitorRecsDaProperty =
            DependencyProperty.Register(
                "MonitorRecs",
                typeof(ObservableCollection<MonitorRec>), typeof(PressMachineParamstersView),
                new FrameworkPropertyMetadata(new ObservableCollection<MonitorRec>(),
                    new PropertyChangedCallback(OnPloModelControlRecChanged)
                    )
                { BindsTwoWayByDefault = true }
                );

        private static void OnPloModelControlRecChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PressMachineParamstersView)d).MonitorRecs = (ObservableCollection<MonitorRec>)e.NewValue!;
        }

        public ObservableCollection<MonitorRec> MonitorRecs
        {
            get { return (ObservableCollection<MonitorRec>)GetValue(MonitorRecsDaProperty); }
            set { SetValue(MonitorRecsDaProperty, value); }
        }

        #endregion

        public PressMachineParamstersView()
        {
            InitializeComponent();
            MonitorRec.ItemsSource = MonitorRecs;
           
        }

        public List<MonitorType> MonitorTypeItemsSource { get; set; }

        private List<T> GetEnumValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        private void AddWindow_Click(object sender, RoutedEventArgs e)
        {
            // 添加新记录
            this.MonitorRecs.Add(new MonitorRec { Id = MonitorRecs.Count + 1 });
            ChangeId();
        }

        private void InsertWindow_Click(object sender, RoutedEventArgs e)
        {
            // 插入新记录到选中行的上方
            if (MonitorRec.SelectedItem is MonitorRec selectedRecord)
            {
                int index = MonitorRecs.IndexOf(selectedRecord);
                MonitorRecs.Insert(index, new MonitorRec { Id = MonitorRecs.Count + 1 });
            }
            else
            {
                MessageBox.Show("请先选择一行进行插入。");
            }
            ChangeId();
        }

        private void DeleteWindow_Click(object sender, RoutedEventArgs e)
        {
            // 删除选中行
            if (MonitorRec.SelectedItem is MonitorRec selectedRecord)
            {
                MonitorRecs.Remove(selectedRecord);
            }
            else
            {
                MessageBox.Show("请先选择一行进行删除。");
            }
            ChangeId();
        }

        private void ClearWindow_Click(object sender, RoutedEventArgs e)
        {
            // 清空所有记录
            MonitorRecs.Clear();
        }

        private void ChangeId()
        {
            for (int i = 0; i < MonitorRecs.Count; i++)
            {
                MonitorRecs[i].Id = i + 1;
            }
        }
    }
}
