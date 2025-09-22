using System.Data.Entity;
using WPF.Admin.Models.Models;

namespace WPF.Admin.Models.Db {
    public class CheckCodeDbContext : DbContext {
        public DbSet<CheckCodeModel> CheckCodeDbModels { get; set; }
    }
}