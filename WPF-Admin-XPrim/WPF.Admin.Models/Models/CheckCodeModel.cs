using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;


namespace WPF.Admin.Models.Models {
    [Table("CheckCode")]
    public class CheckCodeModel {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? Version { get; set; }

        public string? ParameterName { get; set; }

        public List<CheckCodeContent> CheckCodeContents { get; set; }
    }

    public class CheckCodeContent {
        /// <summary>
        /// AutoMode映射
        /// </summary>
        public string PlotAutoModeName { get; set; }

        public bool OpenStep { get; set; }

        public List<CheckRolaContent> CheckRolaContents { get; set; }
    }

    public partial class CheckRolaContent() : BindableBase {
        public int OpenStep { get; set; }
        [ObservableProperty] private ObservableCollection<CheckRolaValue> checkRolaValues;
    }

    public partial class CheckRolaValue : BindableBase {
        public string TokenKey { get; set; }
        [ObservableProperty] private string rolaString;
        public AutoRolaCodeType AutoRolaCodeType { get; set; }
    }
}