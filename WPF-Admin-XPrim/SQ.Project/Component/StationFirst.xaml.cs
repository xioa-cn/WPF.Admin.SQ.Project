using System.Windows.Controls;
using SQ.Project.ViewModels;

namespace SQ.Project.Component
{
    public partial class StationFirst : UserControl
    {
        public StationFirst(StationFirstViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}