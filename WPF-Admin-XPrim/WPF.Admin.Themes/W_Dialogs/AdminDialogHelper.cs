using HandyControl.Controls;
using HandyControl.Tools.Extension;
using System.Windows;
using WPF.Admin.Models.Models;
using WPF.Admin.Themes.I18n;

namespace WPF.Admin.Themes.W_Dialogs
{
    public static class AdminDialogHelper
    {
        public static (string ok, string cancel, string yes, string no) DialogBtnI18nText()
        {
            var (t, _) = CSharpI18n.UseI18n();

            var ok = t("btn.Confirm");
            var cancel = t("btn.Cancel");
            var yes = t("btn.Yes");
            var no = t("btn.No");

            return (ok, cancel, yes, no);
        }


        public static async Task<MessageBoxResult> ShowTextDialog(
            string message, string dialogToken,
            MessageBoxButton buttontype = MessageBoxButton.OK,
            string ok = "确认", string cancel = "取消", string yes = "是", string no = "否")
        {
            (ok, cancel, yes, no) = DialogBtnI18nText();
            var textDialog = new TextDialog(message, dialogToken, buttontype, ok, cancel, yes, no);
            var dialog = Dialog.Show(textDialog, dialogToken);
            var temp = dialog.Initialize<TextDialogViewModel>(vm => { });
            var result = await temp.GetResultAsync<MessageBoxResult>();
            Dialog.Close(dialogToken);
            return result;
        }

        public static async Task<(MessageBoxResult, KeyValueDialogResult)> ShowKeyValueDialog(
            string title, string dialogToken,
            MessageBoxButton buttontype = MessageBoxButton.OK,
            string ok = "确认", string cancel = "取消", string yes = "是", string no = "否")
        {
            (ok, cancel, yes, no) = DialogBtnI18nText();
            var textDialog = new KeyValueDialog(title, dialogToken, buttontype, ok, cancel, yes, no);
            var dialog = Dialog.Show(textDialog, dialogToken);
            var temp = dialog.Initialize<KeyValueDialogViewModel>(vm => { });
            var result = await temp.GetResultAsync<MessageBoxResult>();
            Dialog.Close(dialogToken);
            return (result, textDialog.GetDialogKeyValueResult);
        }

        public static async Task<(MessageBoxResult, string)> ShowInputTextDialog(
            string title, string dialogToken,
            MessageBoxButton buttontype = MessageBoxButton.OK,
            string ok = "确认", string cancel = "取消", string yes = "是", string no = "否")
        {
            (ok, cancel, yes, no) = DialogBtnI18nText();
            var textBoxDialog = new TextBoxDialog(title, dialogToken, buttontype, ok, cancel, yes, no);
            var dialog = Dialog.Show(textBoxDialog, dialogToken);
            var temp = dialog.Initialize<TextBoxDialogViewModel>(vm => { });
            var result = await temp.GetResultAsync<MessageBoxResult>();
            Dialog.Close(dialogToken);
            return (result, textBoxDialog.GetDialogInputResult);
        }
    }
}