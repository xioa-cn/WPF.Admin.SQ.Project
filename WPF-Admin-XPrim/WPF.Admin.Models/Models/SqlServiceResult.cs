namespace WPF.Admin.Models.Models {
    public class SqlServiceResult {
        public bool IsSuccess { get; set; }
        public string ErroeMessage { get; set; }

        public static SqlServiceResult Success(bool isSuccess) {
            return new SqlServiceResult() {
                IsSuccess = isSuccess,
                ErroeMessage = ""
            };
        }

        public static SqlServiceResult Error(string errorMessage) {
            return new SqlServiceResult() {
                IsSuccess = false,
                ErroeMessage = errorMessage
            };
        }
        
    }
}