using System.IO.Ports;
using Microsoft.EntityFrameworkCore;
using WPF.Admin.Models.Impl;
using WPF.Admin.Models.Models;

namespace WPF.Admin.Models.Db
{
    public class SignalDbContext : DbContext, ISqliteNormalable
    {
        protected string ConnectionString
        {
            get => System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Config", "Signal.sqlite"
            );
        }

        public DbSet<ComModelEntity> ComModels { get; set; }

        public DbSet<SignalDbModelEntity> SignalDbModels { get; set; }

        public SignalDbContext() : base()
        {
        }

        public SignalDbContext(DbContextOptions<SysDbContent> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (ApplicationAuthTaskFactory.AuthFlag)
            {
                throw new Exception("授权失败，无法获取DB");
            }

            optionsBuilder.UseSqlite("Data Source = " + ConnectionString);
            //默认禁用实体跟踪
            //optionsBuilder = optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            base.OnConfiguring(optionsBuilder);
        }

        public async Task DbFileExistOrCreate()
        {
            if (System.IO.File.Exists(ConnectionString))
            {
                return;
            }

            var createResult = this.Database.EnsureCreated();

            if (!createResult)
            {
                throw new DbUpdateException(nameof(SysDbContent));
            }

            this.ComModels.Add(new ComModelEntity
            {
                AutoConnect = false,
                ComName = "COM1",
                ComBaudRate = 0,
                ComDataBits = 0,
                ComParity = Parity.None,
                ComStopBits = StopBits.None
            });

            await this.SaveChangesAsync();

        }
    }
}