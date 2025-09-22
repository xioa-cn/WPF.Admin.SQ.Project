using SQ.Project.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SQ.Project.Views
{
    /// <summary>
    /// SQVisualHome.xaml 的交互逻辑
    /// </summary>
    public partial class SQVisualHome : Page, IDisposable
    {
        private SQVisualHomeViewModel? _viewModel;
        private bool _disposed;
        public SQVisualHome()
        {
            this.Loaded += SQVisualHome_Loaded;
            Unloaded += SQVisualHome_Unloaded;
            InitializeComponent();
        }

        private void SQVisualHome_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.DataContext != null && this.DataContext is SQVisualHomeViewModel v)
            {
                _viewModel = v;
            }

            if (_disposed)
            {
                _disposed = false;
            }

            _viewModel?.InitTimer();
        }
        private void SQVisualHome_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // 清理托管资源
                    _viewModel.Dispose();
                }

                _disposed = true;
            }
        }

        ~SQVisualHome()
        {
            Dispose(false);
        }
    }
}
