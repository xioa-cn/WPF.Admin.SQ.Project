﻿using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using WPF.Admin.Models;

namespace CustomModules.ViewModels;

public partial class QQComboBoxViewModel : BindableBase
{
    [ObservableProperty] private string? _InputText;

    public ObservableCollection<string> Items { get; set; } = new ObservableCollection<string>()
    {

    };

    [RelayCommand]
    private void Delete(string value)
    {
        Items.Remove(value);
    }
    [RelayCommand]
    private void Login()
    {
        if (InputText is null)
        {
            Growl.Success($"Login: 不能为空");
            return;
        }
        var having = Items.ToArray().FirstOrDefault(item => item == InputText);
        if (having is not null)
        {
            Growl.Success($"Login: 已在历史记录");
            return;
        }
        Growl.Success($"Login: 加入历史记录");
        Items.Add(InputText);
    }
}