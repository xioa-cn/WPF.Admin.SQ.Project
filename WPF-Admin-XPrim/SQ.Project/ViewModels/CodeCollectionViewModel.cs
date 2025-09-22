using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.IdentityModel.Logging;
using SQ.Project.Component;
using System.Collections.ObjectModel;
using WPF.Admin.Service.Services;

namespace SQ.Project.ViewModels
{
    public partial class CodeCollectionViewModel : ObservableObject
    {
        [ObservableProperty] private bool _status;

        [ObservableProperty] private string _code = string.Empty;

        [ObservableProperty] private bool _isAutoScroll;

        public ObservableCollection<string> Msg { get; set; }

        public CodeCollectionViewModel()
        {
            Msg = new ObservableCollection<string>();
         
            Status = true;
            Code = "ACC20250922154660-00001";

            Task.Factory.StartNew(TestMsg);
        }

        private async void TestMsg()
        {
            int i = 0;
            while (true)
            {
                MessageToUI($"这是第 {i} 条测试数据");
                i++;
                await Task.Delay(100);
            }
        }

        private void MessageToUI(string message)
        {
            var content = DateTime.Now.ToString("MM/dd-hh:mm:ss") + ": " + message;

            Task.Run(() =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    if (Msg.Count > 500)
                        Msg.RemoveAt(0);
                    Msg.Add(content);
                    if (IsAutoScroll && Msg.Count > 1)
                        WeakReferenceMessenger.Default.Send(new ListViewScollerMessenger(Msg[^1]));
                });
            });

           
        }
    }
}