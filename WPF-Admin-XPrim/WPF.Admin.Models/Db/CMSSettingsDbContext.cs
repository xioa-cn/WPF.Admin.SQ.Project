using Microsoft.EntityFrameworkCore;
using WPF.Admin.Models.Models;

namespace WPF.Admin.Models.Db {
    public class CMSSettingsDbContext : DbContext {
        private static string? NormalDir { get; set; }
        public DbSet<CMSConfigData> CmsConfigDatas { get; set; }

        public CMSSettingsDbContext(DbContextOptions<CMSSettingsDbContext> options,
            string dbDir) : base(options) {
            this.Dir = System.IO.Path.Combine(dbDir,
                "CMSSettings.sqlite3");
            CMSSettingsDbContext.NormalDir = Dir;
        }

        /// <summary>
        /// CMSConfig.Instance.ConfigDir
        /// </summary>
        /// <param name="dbDir"></param>
        public CMSSettingsDbContext(string dbDir) : base() {
            this.Dir = System.IO.Path.Combine(dbDir,
                "CMSSettings.sqlite3");
            CMSSettingsDbContext.NormalDir = Dir;
        }

        public CMSSettingsDbContext() : base() {
            if (string.IsNullOrEmpty(CMSSettingsDbContext.NormalDir))
            {
                throw new Exception("请先初始化CMSConfig.Instance.ConfigDir");
            }

            this.Dir = CMSSettingsDbContext.NormalDir;
        }

        public string Dir { get; init; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite("Data Source=" + Dir);
            base.OnConfiguring(optionsBuilder);
        }
    }
}