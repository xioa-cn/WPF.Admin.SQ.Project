using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WPF.Admin.Models;

namespace WPF.Admin.Themes.W_Dialogs {
    public class AdminDialogBase : BindableBase, HandyControl.Tools.Extension.IDialogResultable<MessageBoxResult> {
        private readonly ICommand _okCommand;
        private readonly ICommand _cancelCommand;
        private readonly ICommand _yesCommand;
        private readonly ICommand _noCommand;
        private readonly string _ok;
        private readonly string _cancel;
        private readonly string _yes;
        private readonly string _no;
        public ObservableCollection<DialogButton> Buttons { get; set; } = new ObservableCollection<DialogButton>();
        public MessageBoxResult Result { get; set; }
        public Action CloseAction { get; set; }

        public AdminDialogBase(string dialogToken,
            MessageBoxButton buttontype = MessageBoxButton.OK,
            string ok = "确认", string cancel = "取消", string yes = "是", string no = "否") {
            this._ok = ok;
            this._cancel = cancel;
            this._yes = yes;
            this._no = no;
            _okCommand = DialogButtonCommand.CreateCommand(() =>
            {
                Result = MessageBoxResult.OK;
                HandyControl.Controls.Dialog.Close(dialogToken);
            });
            _cancelCommand = DialogButtonCommand.CreateCommand(() =>
            {
                Result = MessageBoxResult.Cancel;
                HandyControl.Controls.Dialog.Close(dialogToken);
            });
            _yesCommand = DialogButtonCommand.CreateCommand(() =>
            {
                Result = MessageBoxResult.Yes;
                HandyControl.Controls.Dialog.Close(dialogToken);
            });
            _noCommand = DialogButtonCommand.CreateCommand(() =>
            {
                Result = MessageBoxResult.No;
                HandyControl.Controls.Dialog.Close(dialogToken);
            });
            InitializedButtons(buttontype);
        }

        private void InitializedButtons(MessageBoxButton buttontype) {
            switch (buttontype)
            {
                case MessageBoxButton.OK:
                    Buttons.Add(new DialogButton(_ok, _okCommand));
                    break;
                case MessageBoxButton.OKCancel:
                    Buttons.Add(new DialogButton(_ok, _okCommand));
                    Buttons.Add(new DialogButton(_cancel, _cancelCommand));
                    break;
                case MessageBoxButton.YesNo:
                    Buttons.Add(new DialogButton(_yes, _yesCommand));
                    Buttons.Add(new DialogButton(_no, _noCommand));
                    break;
                case MessageBoxButton.YesNoCancel:
                    Buttons.Add(new DialogButton(_yes, _yesCommand));
                    Buttons.Add(new DialogButton(_no, _noCommand));
                    Buttons.Add(new DialogButton(_cancel, _cancelCommand));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(buttontype), buttontype, null);
            }
        }
    }
}