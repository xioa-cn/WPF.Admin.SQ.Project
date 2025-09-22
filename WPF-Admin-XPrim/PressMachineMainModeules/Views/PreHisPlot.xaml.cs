using PressMachineMainModeules.ViewModels;
using System.Diagnostics;
using System.Windows.Controls;
using WPF.Admin.Themes;
using WPF.Admin.Themes.Helper;
using XPrism.Core.Navigations;

namespace PressMachineMainModeules.Views
{
    /// <summary>
    /// PreHisPlot.xaml 的交互逻辑
    /// </summary>
    public partial class PreHisPlot : Page, INavigationAware
    {
        private bool isFirstLoad = true;

        public static XioaMasks XioaMasks { get; set; }
        public PreHisPlot()
        {
            InitializeComponent();
            XioaMasks = this.XioaMasksView;
        }

        public async Task OnNavigatingToAsync(INavigationParameters parameters)
        {

            var value =
            parameters.GetValue<string>("PlotFullFileName");

            if (value is null) return;


            if (string.IsNullOrWhiteSpace(value)) return;
            if (this.DataContext is PreHisPlotViewModel viewModel)
            {
                Task.Run(() =>
                {
                    if (isFirstLoad)
                    {
                        Thread.Sleep(1000);
                        isFirstLoad = false;
                    }
                    viewModel.LoadingPlot(value);
                });
               
            }
            
        }

        public async Task OnNavigatingFromAsync(INavigationParameters parameters)
        {

        }

        public async Task<bool> CanNavigateToAsync(INavigationParameters parameters)
        {
            return true;
        }

        public async Task<bool> CanNavigateFromAsync(INavigationParameters parameters)
        {
            return true;
        }

       
    }
}
