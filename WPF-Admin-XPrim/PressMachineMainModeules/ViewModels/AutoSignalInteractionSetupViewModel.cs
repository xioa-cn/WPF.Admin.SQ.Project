using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using HandyControl.Controls;
using PressMachineMainModeules.Config;
using WPF.Admin.Models;
using WPF.Admin.Models.Background;
using WPF.Admin.Models.Db;
using WPF.Admin.Models.Models;
using WPF.Admin.Service.Logger;
using WPF.Admin.Themes.I18n;

namespace PressMachineMainModeules.ViewModels
{
    public partial class AutoSignalInteractionSetupViewModel : BindableBase
    {
        [ObservableProperty] private ComModel _comModel;

        public ObservableCollection<SignalDbModel> SignalDbModels { get; set; }

        public AutoSignalInteractionSetupViewModel()
        {
            (t, _) = CSharpI18n.UseI18n();
            SignalDbModels =
                new ObservableCollection<SignalDbModel>();
            Initialized();
        }


        public void Start()
        {
            Task.Factory.StartNew(Comm, TaskCreationOptions.LongRunning);
        }

        private async Task Comm()
        {
            while (true)
            {
                if (_comModel.AutoConnect)
                {
                    try
                    {
                        using SerialPort serialPort = new SerialPort(_comModel.ComName, _comModel.ComBaudRate,
                            _comModel.ComParity, _comModel.ComDataBits, _comModel.ComStopBits);

                        serialPort.Open();

                        byte[] buffer = new byte[2048];
                        DateTime startTime = DateTime.Now;
                        int existCount = 0;

                        var list = this.SignalDbModels.ToList().AsReadOnly();

                        while (true)
                        {
                            int count = serialPort.BytesToRead;

                            if (count > 0)
                            {
                                serialPort.Read(buffer, existCount, count);
                                existCount += count;
                                startTime = DateTime.Now;
                            }

                            if (existCount > 0 && (DateTime.Now - startTime).TotalMilliseconds > 200)
                            {
                                var tempCode = Encoding.ASCII.GetString(buffer, 0, existCount);
                                tempCode = tempCode.Trim('\r').Replace("\r", "").Replace("\n", "");
                                existCount = 0;
                                var tempCodeCopy = tempCode;
                                QueuedHostedService.TaskQueue.QueueBackgroundWorkItem(async (token) =>
                                {
                                    SignalToPlc(tempCodeCopy, list);
                                });
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        XLogGlobal.Logger?.LogError("扫码触发plc信号 串口异常", exception);
                        Growl.ErrorGlobal(exception.Message);
                    }

                    await Task.Delay(5000);
                }

                await Task.Delay(2000);
                return;
            }
        }

        private async void Initialized()
        {
            await using var db = new SignalDbContext();

            var findComModel = db.ComModels.Select(item => new ComModel()
            {
                ComName = item.ComName,
                ComBaudRate = item.ComBaudRate,
                ComDataBits = item.ComDataBits,
                ComParity = item.ComParity,
                ComStopBits = item.ComStopBits,
                AutoConnect = item.AutoConnect,
            }).FirstOrDefault();

            ComModel = findComModel ?? new ComModel();

            var findSignalDbModels = db.SignalDbModels.Select(item => new SignalDbModel
            {
                Code = item.Code,
                Plc = item.Plc,
                Position = item.Position,
                Value = item.Value
            }).ToList();
            foreach (var signalDbModel in findSignalDbModels)
            {
                SignalDbModels.Add(signalDbModel);
            }
        }


        private void SignalToPlc(string code, ReadOnlyCollection<SignalDbModel> signalDbModels)
        {
            var find = signalDbModels.FirstOrDefault(item => item.Code == code);

            if (find is null)
            {
                return;
            }

            var value = ushort.Parse(find.Value);
            var result = ConfigPlcs.Instance[find.Plc]?.Write(find.Position, value);

            if (result is not null && result.IsSuccess)
            {
                return;
            }

            var msg = $"AutoSignalInteractionSetupViewModel.SignalToPlc:{result?.Message}";
            Growl.ErrorGlobal(msg);
            XLogGlobal.Logger?.LogError(msg);
        }
    }
}