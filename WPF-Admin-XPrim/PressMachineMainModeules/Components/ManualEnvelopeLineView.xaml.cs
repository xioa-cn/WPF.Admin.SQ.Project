using CommunityToolkit.Mvvm.Messaging;
using PressMachineMainModeules.Models;
using PressMachineMainModeules.ViewModels;
using System.Windows;
using WPF.Admin.Themes.Controls;

namespace PressMachineMainModeules.Components
{
    /// <summary>
    /// ManualEnvelopeLineView.xaml 的交互逻辑
    /// </summary>
    public partial class ManualEnvelopeLineView : System.Windows.Controls.UserControl
    {
        public static readonly DependencyProperty EnvaelpenProperty =
            ElementBase.Property<ManualEnvelopeLineView, EnvelopePosintModel>(nameof(EnvaelpenProperty));

        public EnvelopePosintModel Envaelpen
        {
            get => (EnvelopePosintModel)GetValue(EnvaelpenProperty);
            set => SetValue(EnvaelpenProperty, value);
        }


        public static readonly DependencyProperty AutoModeProperty =
            ElementBase.Property<ManualEnvelopeLineView, string>(nameof(AutoModeProperty), "");

        public string AutoMode
        {
            get => (string)GetValue(AutoModeProperty);
            set => SetValue(AutoModeProperty, value);
        }

        public ManualEnvelopeLineView()
        {
            this.Loaded += ManualEnvelopeLineView_Loaded;
            this.Unloaded += ManualEnvelopeLineView_Unloaded;
            this.InitializeComponent();
        }

        private void ManualEnvelopeLineView_Unloaded(object sender, RoutedEventArgs e)
        {
            WeakReferenceMessenger.Default.Unregister<WeakEnvelopePosintView_Model>(this);
        }

        private void ManualEnvelopeLineView_Loaded(object sender, RoutedEventArgs e)
        {
            WeakReferenceMessenger.Default.Register<WeakEnvelopePosintView_Model>(this, AcceptData);
        }

        private void AcceptData(object recipient, WeakEnvelopePosintView_Model message)
        {
            this.Envaelpen.Posints = new System.Collections.ObjectModel.ObservableCollection<PosintModel>(
                message.Points);
        }

        private void AddValue(object sender, RoutedEventArgs e)
        {
            this.Envaelpen.Posints.Insert(0, new PosintModel());
        }

        private void DelValue(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn)
            {
                if (btn.Tag is PosintModel model)
                    this.Envaelpen.Posints.Remove(model);
            }
        }

        private void ShowEnvelope(object sender, RoutedEventArgs e)
        {
            this.Envaelpen.Posints = new System.Collections.ObjectModel
                .ObservableCollection<PosintModel>(this.Envaelpen.Posints.OrderBy(e => e.X1).ToArray());
            WeakReferenceMessenger.Default.Send(new WeakEnvelopePosintModel()
            {
                AutoMode = this.AutoMode,
                Value = this.Envaelpen
            });
        }

        private void HideEnvelope(object sender, RoutedEventArgs e)
        {
            
            WeakReferenceMessenger.Default.Send(new WeakEnvelopePosintModel()
            {
                AutoMode = this.AutoMode,
                Value = this.Envaelpen,
                Status = WeakEnvelopePosintModelStatus.Hide
            });
        }

        private void OrderByValue(object sender, RoutedEventArgs e)
        {
            this.Envaelpen.Posints = new System.Collections.ObjectModel
               .ObservableCollection<PosintModel>(this.Envaelpen.Posints.OrderBy(e => e.X1).ToArray());
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            AutoEnvelopeLineWindow autoEnvelopeLineWindow = new AutoEnvelopeLineWindow
            {
                DataContext = AutoEnvelopeLineViewModel.Instance,
                Owner = Window.GetWindow(this),
            };

            autoEnvelopeLineWindow.Show();
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            this.Envaelpen.Posints.Clear();
        }
    }
}
