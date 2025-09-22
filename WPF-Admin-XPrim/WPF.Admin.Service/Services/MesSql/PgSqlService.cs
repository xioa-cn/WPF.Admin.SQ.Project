using Npgsql;
using WPF.Admin.Models.Models;
using WPF.Admin.Service.Logger;

namespace WPF.Admin.Service.Services.MesSql {
    public class PgSqlService : SqlServicesBase {
        public PgSqlService(string connectString) : base(connectString) {
        }


        public override async Task<SqlServiceResult> ExecuteAsync(string sqlCommand) {
            try
            {
                // 使用Npgsql连接对象
                await using NpgsqlConnection connection = new NpgsqlConnection(ConnectString);
                await connection.OpenAsync(); 
                await using NpgsqlCommand command = new NpgsqlCommand(sqlCommand, connection);
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