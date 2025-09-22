using CommunityToolkit.Mvvm.Messaging;
using SQ.Project.ViewModels;
using System.Windows.Controls;

namespace SQ.Project.Component
{
    public partial class StationFirst : UserControl
    {
        public StationFirst(StationFirstViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
            WeakReferenceMessenger.Default.Register<ListViewScollerMessengerStationFirst>(this, ListViewGoToScollerBar);
        }

        private void ListViewGoToScollerBar(object recipient, ListViewScollerMessengerStationFirst message)
        {
            ListBox_Msg?.ScrollIntoView(message.GoText);
        }
    }

    public class ListViewScollerMessengerStationFirst
    {
        public object GoText { get; set; }


        public ListViewScollerMessengerStationFirst(object goText)
        {
            GoText = goText;
        }
    }
}