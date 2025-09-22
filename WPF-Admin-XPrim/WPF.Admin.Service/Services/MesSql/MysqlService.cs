using MySqlConnector;
using WPF.Admin.Models.Models;
using WPF.Admin.Service.Logger;

namespace WPF.Admin.Service.Services.MesSql {
    public class MysqlService : SqlServicesBase {
        public MysqlService(string connectString) : base(connectString) {
        }

        public override async Task<SqlServiceResult> ExecuteAsync(string sqlCommand)
        {
            try
            {
                await using MySqlConnection connection = new MySqlConnection(ConnectString);
                await connection.OpenAsync(); // 异步打开连接
                await using MySqlCommand command = new MySqlCommand(sqlCommand, connection);
                // 使用 ExecuteNonQueryAsync 执行插入、更新或删除操作
                int rowsAffected = await command.ExecuteNonQueryAsync();
        
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