using Microsoft.EntityFrameworkCore;
using WPF.Admin.Models.Models;

namespace WPF.Admin.Models.Db {
    public class AlarmDbContext : DbContext {
        
        internal AlarmDbContext(DbContextOptions<AlarmDbContext> options,
            string dbDir) : base(options) {
            this._connectionString = dbDir;
        }
        
        public DbSet<AlarmLog> AlarmLogs { get; set; }

        private readonly string _connectionString;

        internal AlarmDbContext(string dbDir) {
            this._connectionString = dbDir;
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite("Data Source=" + _connectionString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}