using Microsoft.EntityFrameworkCore;
using WPF.Admin.Models.Models;

namespace WPF.Admin.Models.Db {
    public class LogDbContext : DbContext {
        public LogDbContext(DbContextOptions<LogDbContext> options) : base(options) {
        }
        public DbSet<DbLogEntry> DbLogEntries { get; set; }
        private readonly string _connectionString;
        public LogDbContext(string connectionString) {
            this._connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite("Data Source=" + _connectionString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}