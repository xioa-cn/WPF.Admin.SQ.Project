using WPF.Admin.Models.Models;
using WPF.Admin.Service.Logger;
using WPF.Admin.Service.Services.MesSql;

namespace PressMachineMainModeules.Models {
    public class AutoMesSqlMode {
        public string ConnectString { get; set; }
        public string SqlCommand { get; set; }
        public AutoMesSqlDbType SqlType { get; set; }

        public async Task<SqlServiceResult> ExecuteSaveSql(AutoMesProperties autoMesProperties) {
            var service = SqlServiceHelper.GetSqlService(SqlType, ConnectString);
            var sqlCommand = autoMesProperties.Analysis(this.SqlCommand);
            XLogGlobal.Logger?.LogInfo("执行MesSQLCommand:" + sqlCommand);
            var result = await service.ExecuteAsync(sqlCommand);
            return result;
        }
    }
}