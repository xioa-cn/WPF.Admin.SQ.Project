using Microsoft.EntityFrameworkCore;
using WPF.Admin.Models.Impl;
using WPF.Admin.Models.Models;
using WPF.Admin.Models.Utils;

namespace WPF.Admin.Models.Db {
    public class SysDbContent : DbContext, ISqliteNormalable {
        protected string ConnectionString {
            get => System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Config", "user.sqlite"
            );
        }

        public DbSet<LoginUser> LoginUsers { get; set; }

        public SysDbContent() : base() {
        }

        public SysDbContent(DbContextOptions<SysDbContent> options) : base(options) {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (ApplicationAuthTaskFactory.AuthFlag)
            {
                throw new Exception("授权失败，无法获取DB");
            }

            optionsBuilder.UseSqlite("Data Source = " + ConnectionString );
            //默认禁用实体跟踪
            //optionsBuilder = optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            base.OnConfiguring(optionsBuilder);
        }

        public async Task DbFileExistOrCreate() {
            if (System.IO.File.Exists(ConnectionString))
            {
                return;
            }

            var createResult = this.Database.EnsureCreated();

            if (!createResult)
            {
                throw new DbUpdateException(nameof(SysDbContent));
            }

            this.LoginUsers.Add(new LoginUser {
                LoginAuth = LoginAuth.Admin,
                UserName = "Admin",
                Password = "1433223",
            });
            await this.SaveChangesAsync();
        }
    }
}