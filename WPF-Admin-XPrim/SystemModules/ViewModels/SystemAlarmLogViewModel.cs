using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using WPF.Admin.Models;
using WPF.Admin.Models.Models;
using WPF.Admin.Models.Utils;
using WPF.Admin.Themes.I18n;

namespace SystemModules.ViewModels {
    public partial class SystemAlarmLogViewModel : BindableBase {

        public SystemAlarmLogViewModel()
        {
            (t, _) = CSharpI18n.UseI18n();
        }
        public ObservableCollection<AlarmLog> AlarmLogs { get; set; } = new ObservableCollection<AlarmLog>();

        [RelayCommand]
        public void Loaded() {
            AlarmLogs.Clear();
            using var db = AlarmDbInstance.CreateNormal();
            var findlogs = db.AlarmLogs
                .OrderByDescending(log => log.CreateTime) // 按时间降序排序
                .Take(5000) // 取前五千条记录
                .ToList(); // 转换为列表
            
            findlogs.ForEach(item => AlarmLogs.Add(item));

            Growl.SuccessGlobal(t!("Growl.SuccessGlobal"));
        }
    }
}