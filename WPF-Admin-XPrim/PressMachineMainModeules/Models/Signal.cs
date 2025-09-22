using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressMachineMainModeules.Models
{
    public enum Edge
    {

       
        /// <summary>
        /// 上升沿
        /// </summary>
        Rising,

        /// <summary>
        /// 下降沿
        /// </summary>
        Falling,

        //     真
        True,

        //     假
        False
    }

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
}
