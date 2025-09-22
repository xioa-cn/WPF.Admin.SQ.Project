using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace WPF.Admin.Themes.W_Dialogs {
    public static class DialogButtonCommand {
        public static ICommand CreateCommand(Action action) {
            return new RelayCommand(action);
        }
    }

    public class DialogButtonCommand<T> {
        public static ICommand CreateCommand(Action<T> action) {
            return new RelayCommand<T>(action);
        }
    }
}

