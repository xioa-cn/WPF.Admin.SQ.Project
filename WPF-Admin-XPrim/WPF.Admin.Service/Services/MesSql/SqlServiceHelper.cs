using WPF.Admin.Models.Models;

namespace WPF.Admin.Service.Services.MesSql {
    public static class SqlServiceHelper {
        public static ISqlServices GetSqlService(WPF.Admin.Models.Models.AutoMesSqlDbType dbType, string connectString) {
            return dbType switch {
                AutoMesSqlDbType.SqlServer => new SqlServerService(connectString),
                AutoMesSqlDbType.Mysql => new MysqlService(connectString),
                AutoMesSqlDbType.Oracle => new OracleService(connectString),
                AutoMesSqlDbType.Sqlite => new SqliteService(connectString),
                AutoMesSqlDbType.PgSql => new PgSqlService(connectString),
                AutoMesSqlDbType.None => new MysqlService(connectString),
                _ => throw new NotImplementedException(),
            };
        }
    }
}