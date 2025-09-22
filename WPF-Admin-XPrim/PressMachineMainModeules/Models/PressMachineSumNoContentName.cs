using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressMachineMainModeules.Models
{
    public partial class PressMachineSumNoContentName : ObservableObject
    {
        [ObservableProperty] private string? press01;
        [ObservableProperty] private string? press02;
        [ObservableProperty] private string? press03;
        [ObservableProperty] private string? press04;
    }
}
