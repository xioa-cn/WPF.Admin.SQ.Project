using XPrism.Core.BindableBase;
using XPrism.Core.Events;

namespace WPF.Admin.Models;

public abstract class BindableBase : ViewModelBase
{
    /// <summary>
    /// I18n
    /// </summary>
    public Func<string, string>? t; 
    
    public BindableBase(IEventAggregator eventAggregator) : base(eventAggregator: eventAggregator)
    {

    }


    public BindableBase()
    {

    }

}