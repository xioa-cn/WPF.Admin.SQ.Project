using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using SQ.Project.Component;
using System.Collections.ObjectModel;
using SQ.Project.Core;
using SQ.Project.Https;
using SQ.Project.Interfaces;
using SQ.Project.Models;
using WPF.Admin.Service.Services;
using WPF.Admin.UserLogger;

namespace SQ.Project.ViewModels
{
    public partial class CodeCollectionViewModel : RequestBindableBase,IWorkstation
    {
        public string Wsid { get; set; } = "总线扫码";
        
        [ObservableProperty] private bool _status;

        [ObservableProperty] private string _code = string.Empty;

        [ObservableProperty] private bool _isAutoScroll;

        public ObservableCollection<string> Msg { get; set; }

        public CodeCollectionViewModel() : base()
        {
            Msg = new ObservableCollection<string>();

            Status = true;
            Code = "ACC20250922154660-00001";

            Task.Factory.StartNew(Test, TaskCreationOptions.LongRunning);

            Task.Factory.StartNew(RequestTest);
        }

        private async Task RequestTest()
        {
            var result = await this.PostAsync<ResponseInfo<ResultMessageInfo>>(Api.TypeInBomPost,
                new
                {
                    plid = Const.Plid,
                    materialCode = "string"
                });
        }

        private async Task Test()
        {
            int i = 0;
            while (true)
            {
                MessageToUI($"这是第 {i} 条测试数据");
                i++;
                await Task.Delay(5000);
            }
        }

        private void MessageToUI(string message)
        {
            var content = DateTime.Now.ToString("MM/dd-hh:mm:ss") + ": " + message;

            Task.Run((Action)(() =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI((Action)(() =>
                {
                    if (Msg.Count > 500)
                        Msg.RemoveAt(0);
                    Msg.Add(content);
                    if (IsAutoScroll && Msg.Count > 1)
                        WeakReferenceMessenger.Default.Send(new ListViewScollerMessengerStationFirst((object)Msg[^1]));
                }));
            }));

            UserLogService.Instance?.LogInfo(message, Wsid);
        }
    }
}