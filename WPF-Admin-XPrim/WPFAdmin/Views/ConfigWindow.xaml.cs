using System.Windows;

namespace WPFAdmin.Views;

public partial class ConfigWindow : Window {
    public ConfigWindow() {
        InitializeComponent();
        this.Closed += ConfigWindow_Closed;
    }

    private void ConfigWindow_Closed(object? sender, EventArgs e)
    {
        Environment.Exit(0);
    }
}