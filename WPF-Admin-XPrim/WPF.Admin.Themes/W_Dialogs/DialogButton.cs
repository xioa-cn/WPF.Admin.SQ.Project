using System.Windows.Input;

namespace WPF.Admin.Themes.W_Dialogs {
    public class DialogButton {
        public string Text { get; set; }
        public ICommand Command { get; set; }

        public DialogButton(string text, ICommand command) {
            this.Text = text;
            this.Command = command;
        }
    }
}