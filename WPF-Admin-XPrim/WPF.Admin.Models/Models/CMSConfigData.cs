using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WPF.Admin.Models.EFDbContext;

namespace WPF.Admin.Models.Models {
    [Table("CMSConfig")]
    public class CMSConfigData : CMSBaseEntity {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required] 
        [MaxLength(500)]
        public string ConfigName { get; set; }
        public string ConfigJson { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
}