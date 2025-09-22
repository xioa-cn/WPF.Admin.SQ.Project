using WPF.Admin.Service.Services.MesSql;

namespace AutoPreTest {
    public class AutoMesUploadSqlTest {
        [Fact]
        public async void MysqlTest() {
            var data = AutoMesPropertiesTest.TestModel();
            var connect = "Server=127.0.0.1;Port=3306;Database=mydb;Uid=root;Pwd=123456;SslMode=Required;";
            var sql = "INSERT INTO pretable (Code, Data) VALUES ('%{Code}%', '%{Recipe}%-%{ResultString}%');";
            
            var sqlService = new MysqlService(connect);
            var sqlCommand = data.Analysis(sql);
            var result =await sqlService.ExecuteAsync(sqlCommand);
        }
    }
}