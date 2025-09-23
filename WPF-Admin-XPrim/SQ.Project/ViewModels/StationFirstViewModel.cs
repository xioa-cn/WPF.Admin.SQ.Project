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
    public partial class StationFirstViewModel : RequestBindableBase, IWorkstation
    {
        public string Wsid { get; set; } = "OP10";

        [ObservableProperty] private string _code = string.Empty;

        [ObservableProperty] private bool _status;

        [ObservableProperty] private bool _mesResult;

        [ObservableProperty] private bool _isAutoScroll;

        public ObservableCollection<string> Msg { get; set; }

        public StationFirstViewModel()
        {
            Msg = new ObservableCollection<string>();

            Code = "ACC20250922154660-00001";

            Status = true;

            MesResult = true;

            Task.Factory.StartNew(Test, TaskCreationOptions.LongRunning);

            Task.Factory.StartNew(RequestTest);
        }

        private async Task RequestTest()
        {
            var statusResult = await this.PostAsync<ResponseInfo<ResultMessageInfo>>(Api.StationStatusPost,
                new StationStatusInfo()
                {
                    Wsid = Wsid,
                    Status = 1,
                    Msg = "测试数据"
                }
            );


            var result = await this.PostAsync<ResponseInfo<ResultMessageInfo>>(Api.CheckCodePost,
                new
                {
                    plid = Const.Plid,
                    itm = "TESTOPERATION-010-00001",
                    wsid = "OP10"
                });

            if (result.IsSuccess && result.Data.Result)
            {
                result = await this.PostAsync<ResponseInfo<ResultMessageInfo>>(Api.SaveDataPost,
                    new SaveDataBody()
                    {
                        Wsid = "OP10",
                        Itm = "TESTOPERATION-010-00001",
                        Result = 1,
                        PopOnline = DateTimeNowStr,
                    });
            }
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

            Task.Run(() =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    if (Msg.Count > 500)
                        Msg.RemoveAt(0);
                    Msg.Add(content);
                    if (IsAutoScroll && Msg.Count > 1)
                        WeakReferenceMessenger.Default.Send(new ListViewScollerMessengerStationFirst(Msg[^1]));
                });
            });

            UserLogService.Instance?.LogInfo(message, Wsid);
        }
    }
}