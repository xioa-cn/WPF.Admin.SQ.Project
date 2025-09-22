using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using System.Windows;
using SystemModules.Views;
using WPF.Admin.Models;
using WPF.Admin.Models.Db;
using WPF.Admin.Models.Models;

namespace SystemModules.ViewModels
{
    public partial class SystemUserViewModel : BindableBase
    {
        
        [RelayCommand]
        private void Add()
        {
            EditUserMessageWindow edit = new EditUserMessageWindow(null,EditType.Create);
            edit.ShowDialog();
            if (edit.Reuslt == EditReuslt.EditSucces)
            {
                RefreshView();
            }

        }

        [RelayCommand]
        private void Delete(LoginUser loginUser)
        {
            var result = HandyControl.Controls.MessageBox.Show($"{t!("User.TipDelUser")}{loginUser.UserName}",
                t!("Msg.Title_Hint"), MessageBoxButton.YesNo);

            if(loginUser.UserName is "Admin")
            {
                Growl.WarningGlobal(t!("User.AdminDel"));
                return;
            }


            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            using var db = new SysDbContent();
            var find = db.LoginUsers.FirstOrDefault(e => e.Id == loginUser.Id && e.UserName == loginUser.UserName);

            if (find is null)
            {
                return;
            }

            db.Remove(find);
            db.SaveChanges();
            Growl.SuccessGlobal(t!("User.DelSuccess"));
            RefreshView();
        }

        [RelayCommand]
        private void Edit(LoginUser loginUser)
        {
            EditUserMessageWindow edit = new EditUserMessageWindow(loginUser);
            edit.ShowDialog();
            if (edit.Reuslt == EditReuslt.EditSucces)
            {
                RefreshView();
            }
        }

        private void RefreshView()
        {
            _allItems = GenerateSampleData();
            UpdatePagingInfo();
            LoadCurrentPageData();
        }
    }
}
