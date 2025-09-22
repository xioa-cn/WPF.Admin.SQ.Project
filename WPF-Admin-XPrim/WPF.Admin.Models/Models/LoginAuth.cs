namespace WPF.Admin.Models.Models;

public enum LoginAuth
{
    Test=0,
    Admin = 1,//超级权限
    FUser = 2,//前台用户
    HUser = 3,//后台用户
    Engineer = 4,
    Employee = 5,
    None = 6,
}