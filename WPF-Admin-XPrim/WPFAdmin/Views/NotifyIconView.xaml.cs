using System.Windows.Controls;
using WPF.Admin.Models.Models;
using WPFAdmin.ViewModels;

namespace WPFAdmin.Views;

public partial class NotifyIconView : UserControl {
    public NotifyIconView(Action<CloseEnum>? action = null) {
        this.DataContext = new NotifyIconViewModel(action);
        InitializeComponent();
    }
}