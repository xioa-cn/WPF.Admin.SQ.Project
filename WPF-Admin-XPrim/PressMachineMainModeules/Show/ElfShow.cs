using PressMachineMainModeules.Models;

namespace PressMachineMainModeules.Show;

public class ElfShow {
     public static ElfContent[] ElfContents { get; set; } = {
                new ElfContent {
                    Content = "原点回归",
                    Down = "DB1-BOOL-TRUE",
                    UsingUp = true,
                    Up = "DB1-BOOL-FALSE",
                    BtnType = BtnType.Info,
                },
                new ElfContent {
                    Content = "报警复位",
                    Down = "DB1-BOOL-TRUE",
                    UsingUp = true,
                    Up = "DB1-BOOL-FALSE",
                    BtnType = BtnType.Error,
                },
                new ElfContent {
                    Content = "手动模式",
                    Down = "DB1-BOOL-FALSE",
                    BtnType = BtnType.Success,
                },
                new ElfContent {
                    Content = "自动模式",
                    Down = "DB1-BOOL-FALSE",
                    BtnType = BtnType.Success,
                },
                new ElfContent {
                    Content = "自动启动",
                    Down = "DB1-BOOL-FALSE",
                    BtnType = BtnType.Success,
                },
                new ElfContent {
                    Content = "自动停止",
                    Down = "DB1-BOOL-FALSE",
                    BtnType = BtnType.Success,
                },
                new ElfContent {
                    Content = "产量清零",
                    Down = "DB1-BOOL-FALSE",
                    BtnType = BtnType.Success,
                },
            };
}