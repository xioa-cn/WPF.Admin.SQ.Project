using System.Windows;
using PressMachineMainModeules.Config;
using PressMachineMainModeules.Models;
using WPF.Admin.Models.Models;
using WPF.Admin.Themes.Controls;
using WPF.Admin.Themes.W_Dialogs;
using UserControl = System.Windows.Controls.UserControl;

namespace PressMachineMainModeules.Components
{
    public partial class AutoBarcodeCharacteristicsContent : UserControl
    {
        public static readonly DependencyProperty CheckRolaValueSetpProperty =
            ElementBase.Property<AutoBarcodeCharacteristicsContent, CheckRolaValueSetp>(
                nameof(CheckRolaValueSetpProperty));

        public CheckRolaValueSetp CheckRolaValueSetp
        {
            get { return (CheckRolaValueSetp)GetValue(CheckRolaValueSetpProperty); }
            set { SetValue(CheckRolaValueSetpProperty, value); }
        }

        public AutoBarcodeCharacteristicsContent()
        {
            InitializeComponent();
        }

        private void DataGrid_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private async void AddRolaClick(object sender, RoutedEventArgs e)
        {
            var result = await AdminDialogHelper.ShowInputTextDialog(
                Common.t("BarcodeCharacteristics.WriteRule"), HcDialogMessageToken.DialogPartialCodeToken,
                MessageBoxButton.OKCancel);


            if (result.Item1 == MessageBoxResult.OK)
            {
                if (this.CheckRolaValueSetp.CheckRolaValues is null)
                    this.CheckRolaValueSetp.CheckRolaValues =
                        new System.Collections.ObjectModel.ObservableCollection<CheckRolaValue>();
                this.CheckRolaValueSetp.CheckRolaValues.Add(new CheckRolaValue
                {
                    TokenKey = this.CheckRolaValueSetp.TokenKey,
                    AutoRolaCodeType = AutoRolaCodeType.Partial,
                    RolaString = result.Item2.Replace(" ", ""),
                });
            }
        }

        private async void DeleteSelectValueClick(object sender, RoutedEventArgs e)
        {
            if (sender is not System.Windows.Controls.Button btn) return;

            if (btn.Tag is not CheckRolaValue value) return;
            var result = await AdminDialogHelper.ShowTextDialog(
                $"{Common.t("BarcodeCharacteristics.IfDelete")}{value.RolaString}{Common.t("BarcodeCharacteristics.Rule")}",
                HcDialogMessageToken.DialogPartialCodeToken, MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                if (this.CheckRolaValueSetp.CheckRolaValues is not null)
                {
                    this.CheckRolaValueSetp.CheckRolaValues.Remove(value);
                }
            }
        }
    }
}