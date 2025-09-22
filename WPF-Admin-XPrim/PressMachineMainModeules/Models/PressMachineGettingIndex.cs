using CommunityToolkit.Mvvm.Messaging;

namespace PressMachineMainModeules.Models;

public enum SetIndexKeyEnum {
    pre1,
    pre2,
    pre3,
    pre4,
}

/// <summary>
/// 设置压机大屏
/// </summary>
public class PressMachineGettingIndex(string key) {
    public string Key { get; set; } = key;

    public static PressMachineGettingIndex Create(SetIndexKeyEnum setIndexKey) =>
        new PressMachineGettingIndex(setIndexKey.ToString());

    public static void ChangeHomeShow(SetIndexKeyEnum setIndexKey) =>
        WeakReferenceMessenger.Default.Send(PressMachineGettingIndex.Create(setIndexKey));
}