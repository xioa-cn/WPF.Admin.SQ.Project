using CommunityToolkit.Mvvm.Messaging;
using SQ.Project.ViewModels;
using System.Windows.Controls;

namespace SQ.Project.Component
{
    public partial class CodeCollection : UserControl
    {
        // private readonly CodeCollectionViewModel _viewModel;
        public CodeCollection(CodeCollectionViewModel viewModel)
        {
            // _viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();

            WeakReferenceMessenger.Default.Register<ListViewScollerMessenger>(this, ListViewGoToScollerBar);
        }

        private void ListViewGoToScollerBar(object recipient, ListViewScollerMessenger message)
        {         
            ListBox_Msg?.ScrollIntoView(message.GoText);          
        }
    }



    public class ListViewScollerMessenger
    {
        public object GoText { get; set; }


        public ListViewScollerMessenger(object goText)
        {
            GoText = goText;
        }
    }
}