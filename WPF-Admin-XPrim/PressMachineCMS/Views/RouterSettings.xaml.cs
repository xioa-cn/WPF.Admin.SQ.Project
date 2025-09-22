using System.Windows;
using System.Windows.Controls;
using PressMachineCMS.Models;
using PressMachineCMS.ViewModels;

namespace PressMachineCMS.Views
{
    public partial class RouterSettings : Page
    {
        public RouterSettings()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is RouterSettingsViewModel viewModel)
            {
                viewModel.SelectedRouter = e.NewValue as RouterModel;
            }
        }
    }
}