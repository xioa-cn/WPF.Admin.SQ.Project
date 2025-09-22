using System.Collections.ObjectModel;
using System.Windows.Input;

namespace WPF.Admin.Themes.I18n
{
    public interface IChangeLangable
    {
        string Lang { get; set; }

        ObservableCollection<LangSource> Langs { get; }

        ICommand ChangeLangCommand { get; }
    }
}