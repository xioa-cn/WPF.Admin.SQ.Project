using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WPF.Admin.Models.EFDbContext;

namespace WPF.Admin.Models.Models
{
    [Table("Pre")]
    public class PreSaveModel : BaseEntity
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(255)]
        public string? Code { get; set; }

        [MaxLength(255)]public string? Recipe { get; set; }

        public string? Step { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        [MaxLength(255)]
        public string? Operator { get; set; }

        public DateTime CreateTime { get; set; }

        [MaxLength(10)]
        public string? Reuslt { get; set; }

        [MaxLength(255)]
        public string? Data { get; set; }
        
        public string? PartialCodeList { get; set; }


        /// <summary>
        /// 曲线文件路径
        /// </summary>
        public string? PoltFilePath { get; set; }
    }
}
