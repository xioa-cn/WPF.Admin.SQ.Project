using PressMachineMainModeules.Components;
using PressMachineMainModeules.ViewModels;
using System.Windows;
using System.Windows.Controls;
namespace PressMachineMainModeules.Views
{
    public class GridCalculator
    {
        public static (int rows, int columns) CalculateGrid(int count)
        {
            if (count <= 0)
                throw new ArgumentException("数必须大于0", nameof(count));

            if (count == 2)
                return (1, 2);

            // 计算行数：平方根向上取整
            int rows = (int)Math.Ceiling(Math.Sqrt(count));

            // 计算列数：总数除以行数向上取整
            int columns = (int)Math.Ceiling((double)count / rows);

            return (rows, columns);
        }
        public static (int row, int column) FindPosition(int row, int colum, int index)
        {
            if (index <= 0)
                throw new ArgumentException("序号必须大于0", nameof(index));
            var row1 = (index - 1) / colum + 1; // 计算行号
            var column = (index - 1) % colum + 1; // 计算列号

            return (row1, column);
        }

    }

    /// <summary>
    /// AutoHomeView.xaml 的交互逻辑
    /// </summary>
    public partial class AutoHomeView : Page
    {
        public AutoHomeView()
        {
            InitializeComponent();
            this.Loaded += AutoHomeView_Loaded;


        }

        private List<AutoPlotView> autoPlotViews = new List<AutoPlotView>();
        private bool first = true;

        private void AutoHomeView_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is AutoHomeViewModel vm && first)
            {
                var plotCount = vm.PlotModels.Count;
                var result = GridCalculator.CalculateGrid(plotCount);
                this.plotGrid.RowDefinitions.Clear();
                this.plotGrid.ColumnDefinitions.Clear();
                for (int i = 0; i < result.rows; i++)
                {
                    this.plotGrid.RowDefinitions.Add(new RowDefinition());
                }
                for (int i = 0; i < result.columns; i++)
                {
                    this.plotGrid.ColumnDefinitions.Add(new ColumnDefinition());
                }

                for (int i = 0; i < plotCount; i++)
                {
                    var position = GridCalculator.FindPosition(result.rows, result.columns, i + 1);
                    AutoPlotView plotView = null;

                    plotView = new AutoPlotView
                    {
                        DataContext = vm.PlotModels[i],
                    };
                  
                    autoPlotViews.Add(plotView);
                    first = false;

                    Grid.SetRow(plotView, position.row - 1);
                    Grid.SetColumn(plotView, position.column - 1);
                    this.plotGrid.Children.Add(plotView);
                }


                vm.NormalInitialized();
            }
        }
    }
}
