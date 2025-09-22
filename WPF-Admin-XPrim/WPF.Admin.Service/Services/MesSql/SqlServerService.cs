using Microsoft.Data.SqlClient;
using WPF.Admin.Models.Models;
using WPF.Admin.Service.Logger;

namespace WPF.Admin.Service.Services.MesSql {
    public class SqlServerService : SqlServicesBase {
        public SqlServerService(string connectString) : base(connectString) {
        }

        public override async Task<SqlServiceResult> ExecuteAsync(string sqlCommand) {
            try
            {
                // 使用SQL Server连接对象
                await using SqlConnection connection = new SqlConnection(ConnectString);
                await connection.OpenAsync(); 
                await using SqlCommand command = new SqlCommand(sqlCommand, connection);
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