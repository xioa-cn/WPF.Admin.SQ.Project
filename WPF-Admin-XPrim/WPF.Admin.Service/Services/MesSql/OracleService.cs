using Oracle.ManagedDataAccess.Client;
using WPF.Admin.Models.Models;
using WPF.Admin.Service.Logger;

namespace WPF.Admin.Service.Services.MesSql {
    public class OracleService : SqlServicesBase {
        public OracleService(string connectString) : base(connectString) {
        }

        public override async Task<SqlServiceResult> ExecuteAsync(string sqlCommand) {
            try
            {
                // 使用Oracle连接对象
                await using OracleConnection connection = new OracleConnection(ConnectString);
                await connection.OpenAsync(); // 异步打开连接

                await using OracleCommand command = new OracleCommand(sqlCommand, connection);
        
                // 执行非查询命令（如INSERT、UPDATE、DELETE）
                int rowsAffected = await command.ExecuteNonQueryAsync();
        
                // 根据受影响行数判断操作是否成功
                return SqlServiceResult.Success(rowsAffected > 0);
            }
            catch (Exception ex)
            {
                XLogGlobal.Logger?.LogError($"执行SQL命令时出错: {sqlCommand}",ex);
                return SqlServiceResult.Error(ex.Message);
            }
        }
    }
}