namespace WPF.Admin.Models.Models;

public class Signal
{       
    public bool OldState { get; set; }      
    public bool NewState { get; set; }      
    public Edge State { get; private set; }     
    public void Update(bool state)
    {
        OldState = NewState;
        NewState = state;
        if (OldState && NewState)
        {
            State = Edge.True;
        }

        if (!OldState && !NewState)
        {
            State = Edge.False;
        }

        if (OldState && !NewState)
        {
            State = Edge.Falling;
        }

        if (!OldState && NewState)
        {
            State = Edge.Rising;
        }
    }
}