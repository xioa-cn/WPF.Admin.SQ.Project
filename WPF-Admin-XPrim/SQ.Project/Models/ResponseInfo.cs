namespace SQ.Project.Models
{
    public class ResponseInfo
    {
        public bool IsSuccess { get; set; }
        public string Time { get; set; }
        public string ResultCode { get; set; }
        public string Message { get; set; }
    }

    public class ResponseInfo<T> : ResponseInfo
    {
        public T Data { get; set; }
    }
}