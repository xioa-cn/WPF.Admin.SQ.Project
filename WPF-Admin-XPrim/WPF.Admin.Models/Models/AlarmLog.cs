using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WPF.Admin.Models.Models {
    public enum AlarmType {
        [Description("系统警告")] System,
        [Description("设备警告")] Device,
    }

    [Table("AlarmLog")]
    public class AlarmLog {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public string? Message { get; set; }
        
        public AlarmType Type { get; set; }
    }
}