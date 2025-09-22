using System.Data.SQLite;
using WPF.Admin.Models.Models;
using WPF.Admin.Service.Logger;

namespace WPF.Admin.Service.Services.MesSql {
    public class SqliteService : SqlServicesBase {
        public SqliteService(string connectString) : base(connectString) {
        }

        public override async Task<SqlServiceResult> ExecuteAsync(string sqlCommand) {
            try
            {
                // 使用SQLite连接对象
                await using SQLiteConnection connection = new SQLiteConnection(ConnectString);
                await connection.OpenAsync(); 
                await using SQLiteCommand command = new SQLiteCommand(sqlCommand, connection);
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