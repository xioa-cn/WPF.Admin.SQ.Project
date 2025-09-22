using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WPF.Admin.Models.Models;

[Table("user")]
public class LoginUser {
    public int Id { get; set; }
    [MaxLength(255)]
    [Required]
    public string? UserName { get; set; }
    [MaxLength(255)]
    [Required]
    public string? Password { get; set; }
    public DateTime CreateTime { get; set; } = DateTime.Now;
    public string? Header { get; set; }
    public LoginAuth LoginAuth { get; set; }

    public UILoginUser ToUILoginUser()
    {
        var loginUser = new UILoginUser
        {
            UserName = this.UserName,
            Password = this.Password,
            CreateTime = this.CreateTime,
            Header = this.Header,
            LoginAuth = this.LoginAuth
        };
        return loginUser;
    }

}