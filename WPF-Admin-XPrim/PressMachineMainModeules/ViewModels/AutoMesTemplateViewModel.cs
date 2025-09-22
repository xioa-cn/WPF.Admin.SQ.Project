using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using PressMachineMainModeules.Models;
using WPF.Admin.Models;
using WPF.Admin.Models.Models;
using WPF.Admin.Service.Logger;
using WPF.Admin.Service.Services.MesSql;
using WPF.Admin.Service.Services.RestClientSevices;
using WPF.Admin.Themes.Helper;
using WPF.Admin.Themes.W_Dialogs;

namespace PressMachineMainModeules.ViewModels
{

    public class MesHeader
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public partial class AutoMesTemplateViewModel : BindableBase
    {

        public AutoMesTemplateViewModel()
        {
            Initialized();
        }

        [ObservableProperty] private bool _openAfterHttp = false;
        [ObservableProperty] private bool _openAfterSql = false;
        [ObservableProperty] private bool _openBefore = false;

        private void Initialized()
        {
            if (AutoMesConfigManager.Instance.AutoMesConfig.OpenAfterChecked)
            {

                if (AutoMesConfigManager.Instance.AutoMesConfig.AfterAutoMesMode == AutoMesAfterMode.None)
                {
                    Growl.WarningGlobal("请先配置MES数据上传模式");
                    return;
                }
                if (AutoMesConfigManager.Instance.AutoMesConfig.AfterAutoMesMode == AutoMesAfterMode.Http)
                {
                    this.OpenAfterHttp = true;
                    this.MesRequestMethodAfter = AutoMesConfigManager.Instance.AutoMesConfig.AfterAutoMesHttp!.MesRequestMethod;
                    this.MesRequestUrlAfter = AutoMesConfigManager.Instance.AutoMesConfig.AfterAutoMesHttp.RequestUrl;
                    this.MesRequestDataAfter = AutoMesConfigManager.Instance.AutoMesConfig.AfterAutoMesHttp.RequestBody;
                    this.HeaderAfter = new ObservableCollection<MesHeader>(
                        AutoMesConfigManager.Instance.AutoMesConfig.AfterAutoMesHttp.Headers.Select(item => new MesHeader { Key = item.Key, Value = item.Value }));
                }
                if (AutoMesConfigManager.Instance.AutoMesConfig.AfterAutoMesMode == AutoMesAfterMode.Sql)
                {
                    this.OpenAfterSql = true;
                    this.MesSqlCommandAfter = AutoMesConfigManager.Instance.AutoMesConfig.AfterAutoMesSql!.SqlCommand;
                    this.MesSqlConnectStringAfter = AutoMesConfigManager.Instance.AutoMesConfig.AfterAutoMesSql.ConnectString;
                    this.MesSqlDbTypeAfter = AutoMesConfigManager.Instance.AutoMesConfig.AfterAutoMesSql.SqlType;
                }
            }
            if (AutoMesConfigManager.Instance.AutoMesConfig.OpenBeforeChecked)
            {
                this.OpenBefore = true;

                this.MesRequestMethodBefore = AutoMesConfigManager.Instance.AutoMesConfig.BeforeAutoMesHttp!.MesRequestMethod;
                this.MesRequestUrlBefore = AutoMesConfigManager.Instance.AutoMesConfig.BeforeAutoMesHttp.RequestUrl;
                this.MesRequestDataBefore = AutoMesConfigManager.Instance.AutoMesConfig.BeforeAutoMesHttp.RequestBody;
                this.HeaderBefore = new ObservableCollection<MesHeader>(
                    AutoMesConfigManager.Instance.AutoMesConfig.BeforeAutoMesHttp.Headers.Select(item => new MesHeader { Key = item.Key, Value = item.Value }));

            }
        }

        [ObservableProperty] private AutoMesSqlDbType _mesSqlDbTypeAfter = AutoMesSqlDbType.None;
        [ObservableProperty] private string _mesSqlCommandAfter = string.Empty;
        [ObservableProperty] private string _mesSqlConnectStringAfter = string.Empty;

        [ObservableProperty] private MesRequestMethod _mesRequestMethodAfter = MesRequestMethod.None;
        [ObservableProperty] private MesRequestMethod _mesRequestMethodBefore = MesRequestMethod.None;

        [ObservableProperty] private string _mesRequestUrlAfter = string.Empty;
        [ObservableProperty] private string _mesRequestUrlBefore = string.Empty;

        [ObservableProperty]
        private string _mesRequestDataAfter =
            string.Empty;
        [ObservableProperty]
        private string _mesRequestDataBefore =
            string.Empty;

        [ObservableProperty] private string _mesResponseDataAfter = string.Empty;
        [ObservableProperty] private string _mesResponseDataBefore = string.Empty;

        [ObservableProperty] private ObservableCollection<MesHeader> _headerAfter = new();
        [ObservableProperty] private ObservableCollection<MesHeader> _headerBefore = new();

        private string mesRequestData(string mode)
        {
            if (string.IsNullOrWhiteSpace(mode)) return string.Empty;
            if (mode.ToLower() == "after")
            {
                return _mesRequestDataAfter;
            }
            else
            {
                return _mesRequestDataBefore;
            }
        }

        private MesRequestMethod mesRequestMethod(string mode)
        {
            if (string.IsNullOrWhiteSpace(mode)) return MesRequestMethod.Get;
            if (mode.ToLower() == "after")
            {
                return _mesRequestMethodAfter;
            }
            else
            {
                return _mesRequestMethodBefore;
            }
        }

        private string mesUrl(string mode)
        {
            if (string.IsNullOrWhiteSpace(mode)) return "localhost";
            if (mode.ToLower() == "after")
            {
                return _mesRequestUrlAfter;
            }
            else
            {
                return _mesRequestUrlBefore;
            }
        }

        private Dictionary<string, string>? mesDictionary(string mode)
        {
            if (string.IsNullOrWhiteSpace(mode)) return null;
            if (mode.ToLower() == "after")
            {
                if (this._headerAfter.Count == 0) return null;
                return _headerAfter.ToDictionary(a => a.Key, b => b.Value);
            }
            else
            {
                if (this._headerBefore.Count == 0) return null;
                return _headerBefore.ToDictionary(a => a.Key, b => b.Value);
            }
        }

        [RelayCommand]
        private async Task ExecuteSql()
        {
            try
            {
                var service = SqlServiceHelper.GetSqlService(_mesSqlDbTypeAfter, _mesSqlConnectStringAfter);

                var result = await service.ExecuteAsync(_mesSqlCommandAfter);

                SnackbarHelper.Show("Sql 执行成功");
            }
            catch (Exception ex)
            {
                Growl.ErrorGlobal(ex.Message);
            }
        }

        [RelayCommand]
        private void ClearResponseData(string mode)
        {
            if (mode.ToLower() == "after")
            {
                this.MesResponseDataAfter = "";
            }
            else if (mode.ToLower() == "before")
            {
                this.MesResponseDataBefore = "";
            }

        }

        [RelayCommand]
        private async Task Request(string mode)
        {
            try
            {
                await using var service = new RestClientServices(mesUrl(mode), mesDictionary(mode));
                var result = await service.SendRequestAsync(
                    mesRequestMethod(mode),
                    mesRequestData(mode));
                SetMesResponseData(mode, result.Content ?? (result.ErrorMessage ?? "Bad Request"));

            }
            catch (Exception e)
            {
                SetMesResponseData(mode, e.Message);
            }
        }

        private void SetMesResponseData(string mode, string message)
        {
            ClearResponseData(mode);
            if (mode.ToLower() == "after")
            {
                this.MesResponseDataAfter += message;
            }
            else if (mode.ToLower() == "before")
            {
                this.MesResponseDataBefore += message;
            }
        }

        [RelayCommand]
        private async Task AddHeader(string mode)
        {
            var result =
                await AdminDialogHelper.ShowKeyValueDialog("请输入请求头参数", HcDialogMessageToken.DialogMesKeyValueToken
                    , buttontype: MessageBoxButton.OKCancel);

            if (result.Item1 != MessageBoxResult.OK) return;

            if (result is { Item2: { Key: not null, Value: not null } })
            {
                //this.Header.Add(result.Item2.Key, result.Item2.Value);
                if (mode.ToLower() == "after")
                {
                    this.HeaderAfter.Add(new MesHeader()
                    {
                        Key = result.Item2.Key,
                        Value = result.Item2.Value,
                    });
                }
                else if (mode.ToLower() == "before")
                {
                    this.HeaderBefore.Add(new MesHeader()
                    {
                        Key = result.Item2.Key,
                        Value = result.Item2.Value,
                    });
                }
            }
            else
            {
                Growl.WarningGlobal("请输入正确的键值对");
            }
        }

        [RelayCommand]
        private void RemoveHeaderAfter(MesHeader header)
        {
            this.HeaderAfter.Remove(header);
        }

        [RelayCommand]
        private void RemoveHeaderBefore(MesHeader header)
        {
            this.HeaderBefore.Remove(header);
        }
    }
}