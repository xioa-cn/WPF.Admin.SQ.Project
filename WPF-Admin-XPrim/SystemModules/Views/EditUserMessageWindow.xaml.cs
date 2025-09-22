using HandyControl.Controls;
using System.Windows;
using System.Windows.Input;
using WPF.Admin.Models.Db;
using WPF.Admin.Models.Models;
using WPF.Admin.Themes.I18n;

namespace SystemModules.Views
{
    public enum EditType
    {
        Change,
        Create
    }

    public enum EditReuslt
    {
        None,
        EditSucces,
        EditError,
        EditCanael
    }

    /// <summary>
    /// EditUserMessageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditUserMessageWindow : System.Windows.Window
    {
        public EditType EditType { get; set; }
        public LoginUser Login { get; set; }

        public Func<string, string> t;

        public EditUserMessageWindow(LoginUser? login, EditType editType = EditType.Change)
        {
            (t, _) = CSharpI18n.UseI18n();
            this.EditType = editType;
            InitializeComponent();
            if (login != null)
            {
                Login = login;
                this.UserName.Text = login.UserName ?? "";
                this.Pwd.Text = login.Password ?? "";
                switch (login.LoginAuth)
                {
                    case LoginAuth.Admin:
                        this.IsAdmin.IsChecked = true;
                        break;
                    case LoginAuth.Engineer:
                        this.IsEngineer.IsChecked = true;
                        break;
                    case LoginAuth.Employee:
                        this.IsOperator.IsChecked = true;
                        break;
                    default:
                        this.IsOperator.IsChecked = true;
                        break;
                }
            }
            else
            {
                Login = new LoginUser();
                this.IsOperator.IsChecked = true;
            }


            if (editType == EditType.Change)
            {
                this.UserName.IsEnabled = false;
            }
        }

        public EditReuslt Reuslt { get; set; }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            this.Login.UserName = this.UserName.Text;
            this.Login.Password = this.Pwd.Text;

            if (string.IsNullOrEmpty(this.Login.UserName))
            {
                Growl.WarningGlobal(t("User.Edit_UserInfoNull"));
                return;
            }

            if (string.IsNullOrEmpty(this.Login.Password))
            {
                Growl.WarningGlobal(t("User.Edit_PasswordNull"));
                return;
            }


            if (this.IsAdmin.IsChecked == true)
            {
                this.Login.LoginAuth = LoginAuth.Admin;
            }

            if (this.IsEngineer.IsChecked == true)
            {
                this.Login.LoginAuth = LoginAuth.Engineer;
            }

            if (this.IsOperator.IsChecked == true)
            {
                this.Login.LoginAuth = LoginAuth.Employee;
            }

            using SysDbContent db = new SysDbContent();
            var findInfo = db.LoginUsers.FirstOrDefault(x => x.UserName == this.UserName.Text);

            if (EditType == EditType.Change && findInfo == null)
            {
                HandyControl.Controls.MessageBox.Show(t("User.UserInfoErrorNotEdit"), "ERROR");
                return;
            }
            else if (EditType == EditType.Change && findInfo != null)
            {
                findInfo.Password = this.Pwd.Text;

                if (findInfo.UserName is not "Admin")
                {
                    findInfo.LoginAuth = this.Login.LoginAuth;
                }
                else if (findInfo.UserName is "Admin" && findInfo.LoginAuth != this.Login.LoginAuth)
                {
                    Growl.WarningGlobal(t("User.AdminEdit"));
                }


                db.LoginUsers.Update(findInfo);
            }
            else if (EditType == EditType.Create && findInfo != null)
            {
                Growl.WarningGlobal(t("User.AccountExist"));
                return;
            }
            else if (EditType == EditType.Create && findInfo == null)
            {
                this.Login.CreateTime = DateTime.Now;
                db.LoginUsers.Add(this.Login);
            }

            try
            {
                var result = db.SaveChanges();
                if (result > 0)
                {
                    Reuslt = EditReuslt.EditSucces;
                    this.Close();
                }
                else
                {
                    Growl.WarningGlobal(t("Save.Error"));
                }
            }
            catch (Exception ex)
            {
                Growl.WarningGlobal($"{t("Save.ErrorMsg")}: {ex.Message}");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Reuslt = EditReuslt.EditCanael;

            this.Close();
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}