using WPF.Admin.Models.Models;

namespace WPF.Admin.Service.Services.MesSql {
    public interface ISqlServices {
        public Task<SqlServiceResult> ExecuteAsync(string sqlCommand);
    }

    public abstract class SqlServicesBase : ISqlServices {
        public string ConnectString { get; set; }

        protected SqlServicesBase(string connectString) {
            ConnectString = connectString;
        }

        public abstract Task<SqlServiceResult> ExecuteAsync(string sqlCommand);
    }
}