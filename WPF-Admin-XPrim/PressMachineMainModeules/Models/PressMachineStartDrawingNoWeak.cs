using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressMachineMainModeules.Models
{
    public class PressMachineStartDrawingNoWeak
    {
        public string? Token { get; set; }

        public static PressMachineStartDrawingNoWeak GetDrawingToken(string? token)
        {
            return new PressMachineStartDrawingNoWeak
            {
                Token = token
            };
        }
    }
}
