using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using PressMachineMainModeules.Helper;
using System.Collections.ObjectModel;
using WPF.Admin.Models;
using WPF.Admin.Models.Db;
using WPF.Admin.Models.Models;
using WPF.Admin.Themes.I18n;
using WPF.Admin.Themes.W_Dialogs;

namespace PressMachineMainModeules.ViewModels
{
    public partial class SignalInteractionViewModel : BindableBase
    {
        public string[] ComNames { get; set; } = System.IO.Ports.SerialPort.GetPortNames();

        [ObservableProperty] private ComModel _comModel;

        public ObservableCollection<SignalDbModel> SignalDbModels { get; set; }

        public SignalInteractionViewModel()
        {
            (t, _) = CSharpI18n.UseI18n();
            SignalDbModels =
                new ObservableCollection<SignalDbModel>();
            Initialized();
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

        [RelayCommand]
        private async Task Save()
        {
            await using var db = new SignalDbContext();
            var findComModel = db.ComModels.FirstOrDefault();
            if (findComModel != null)
            {
                findComModel.AutoConnect = ComModel.AutoConnect;
                findComModel.ComName = ComModel.ComName;
                findComModel.ComBaudRate = ComModel.ComBaudRate;
                findComModel.ComDataBits = ComModel.ComDataBits;
                findComModel.ComParity = ComModel.ComParity;
                findComModel.ComStopBits = ComModel.ComStopBits;
                await db.SaveChangesAsync();
            }
            else
            {
                var data = new ComModelEntity()
                {
                    AutoConnect = ComModel.AutoConnect,
                    ComName = ComModel.ComName,
                    ComBaudRate = ComModel.ComBaudRate,
                    ComDataBits = ComModel.ComDataBits,
                    ComParity = ComModel.ComParity,
                    ComStopBits = ComModel.ComStopBits,
                }
                    ;

                await db.ComModels.AddAsync(data);
                await db.SaveChangesAsync();
            }


            Growl.SuccessGlobal(t!("Msg.PreSystem.SaveSuccess"));
        }

        [RelayCommand]
        public async Task Delete(SignalDbModel signalDbModel)
        {
            var result = await AdminDialogHelper.ShowTextDialog(t("Signal.Msg.DeleteMsg"),WPF.Admin.Models.Models.HcDialogMessageToken.SignalInteractionViewToken,
                System.Windows.MessageBoxButton.OKCancel);

            if(result != System.Windows.MessageBoxResult.OK)
            {
                return;
            }

            await using var db = new SignalDbContext();
            var findModel = db.SignalDbModels.FirstOrDefault(item => item.Code == signalDbModel.Code);

            if (findModel != null)
            {
                db.SignalDbModels.Remove(findModel);
                await db.SaveChangesAsync();
                SignalDbModels.Remove(signalDbModel);
            }
            else
            {
                Growl.ErrorGlobal(t!("Signal.Msg.DeleteError"));
            }
        }


        [RelayCommand]
        private async Task AddCodeRule()
        {
            var result = await PressMachineDialogHelper.ShowSignalDbModelDialog(
                t!("Signal.Msg.AddModel"), WPF.Admin.Models.Models.HcDialogMessageToken.SignalInteractionViewToken,
                System.Windows.MessageBoxButton.OKCancel);

            if (result.Item1 == System.Windows.MessageBoxResult.OK)
            {
                if (string.IsNullOrEmpty(result.Item2.Code))
                {
                    Growl.ErrorGlobal(t!("Signal.Msg.CodeError"));
                    return;
                }

                if (string.IsNullOrEmpty(result.Item2.Plc))
                {
                    Growl.ErrorGlobal(t!("Signal.Msg.PlcError"));
                    return;
                }

                if (string.IsNullOrEmpty(result.Item2.Position))
                {
                    Growl.ErrorGlobal(t!("Signal.Msg.PositionError"));
                    return;
                }

                if (string.IsNullOrEmpty(result.Item2.Value))
                {
                    Growl.ErrorGlobal(t!("Signal.Msg.ValueError"));
                    return;
                }

                using var db = new SignalDbContext();
                var find = db.SignalDbModels.FirstOrDefault(item => item.Code == result.Item2.Code);

                if (find is not null)
                {
                    Growl.ErrorGlobal(t!("Signal.Msg.CodeExist"));
                    return;
                }

                await db.SignalDbModels.AddAsync(new SignalDbModelEntity
                {
                    Code = result.Item2.Code,
                    Plc = result.Item2.Plc,
                    Position = result.Item2.Position,
                    Value = result.Item2.Value,
                });
                await db.SaveChangesAsync();

                this.SignalDbModels.Add(result.Item2);
            }
        }
    }
}