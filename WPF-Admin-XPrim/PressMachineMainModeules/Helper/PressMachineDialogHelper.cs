using HandyControl.Controls;
using HandyControl.Tools.Extension;
using PressMachineMainModeules.Models;
using System.Windows;
using WPF.Admin.Models.Models;
using WPF.Admin.Themes.W_Dialogs;

namespace PressMachineMainModeules.Helper
{
    public static class PressMachineDialogHelper
    {
        public async static Task<(MessageBoxResult,PosintModel)> ShowPosintDialog(
           string message, string dialogToken,
            MessageBoxButton buttontype = MessageBoxButton.OK,
            string ok = "确认", string cancel = "取消", string yes = "是", string no = "否")
        {
            (ok, cancel, yes, no) = AdminDialogHelper.DialogBtnI18nText();
            var textDialog = new PosintModelDialog(message, dialogToken, buttontype, ok, cancel, yes, no);
            var dialog = Dialog.Show(textDialog, dialogToken);
            var temp = dialog.Initialize<PosintModelDialogViewModel>(vm => { });
            var result = await temp.GetResultAsync<MessageBoxResult>();
            Dialog.Close(dialogToken);
            return (result,textDialog.GetDialogPosintModelResult);
        }

        public async static Task<(MessageBoxResult, SignalDbModel)> ShowSignalDbModelDialog(
            string message, string dialogToken,
            MessageBoxButton buttontype = MessageBoxButton.OK,
            string ok = "确认", string cancel = "取消", string yes = "是", string no = "否")
        {
            (ok, cancel, yes, no) = AdminDialogHelper.DialogBtnI18nText();
            var textDialog = new SignalDbModelDialog(message, dialogToken, buttontype, ok, cancel, yes, no);
            var dialog = Dialog.Show(textDialog, dialogToken);
            var temp = dialog.Initialize<SignalDbModelDialogViewModel>(vm => { });
            var result = await temp.GetResultAsync<MessageBoxResult>();
            Dialog.Close(dialogToken);
            return (result, textDialog.GetDialogSignalDbModelResult);
        }
    }
}
