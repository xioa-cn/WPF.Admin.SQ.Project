using System.ComponentModel.DataAnnotations.Schema;
using WPF.Xlog.Logger.Model;


namespace WPF.Admin.Models.Models {
    [Table("dblogentries")]
    public class DbLogEntry {
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }

        public LogLevel Level { get; set; }

        public string? Message { get; set; }

        public string? Source { get; set; }

        public string? UserName { get; set; }
        
        public string? Exception { get; set; }

        public string? AdditionalInfo { get; set; }
    }
}