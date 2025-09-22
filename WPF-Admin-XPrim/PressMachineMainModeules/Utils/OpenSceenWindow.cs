using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Application = System.Windows.Application;

namespace PressMachineMainModeules.Utils
{
    public static class OpenSceenWindow
    {
        public static void ShowNowSceen(this Window window)
        {
            foreach (var obj in Application.Current.Windows)
            {
                if (obj is Window win)
                {
                    if (win.IsActive)
                    {
                        if(window != win)
                        {
                            window.Owner = win;
                            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        }
                        
                        window.Show();
                        return;
                    }
                }
            }
        }
    }
}
