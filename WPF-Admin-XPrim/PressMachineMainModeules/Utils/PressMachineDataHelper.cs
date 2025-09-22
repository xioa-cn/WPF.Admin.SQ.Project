using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.IO;
using WPF.Admin.Models.Models;
using WPF.Admin.Service.Logger;

namespace PressMachineMainModeules.Utils
{
    public enum PressMachineResultEnum
    {
        OK = 0,
        NG = 1,
    }

    [Table("PressMachine")]
    public class PressHistory
    {
        public int Id { get; set; }
        [MaxLength(1000)] public string? Code { get; set; }
        public PressMachineResultEnum Result { get; set; }
        [MaxLength(100)] public string? MaxPos { get; set; }
        [MaxLength(100)] public string? MaxPre { get; set; }
        public DateTime? CreateTime { get; set; } = DateTime.Now;
        [MaxLength(255)] public string? PressMachineNum { get; set; }
        [MaxLength(255)] public string? ParamsName { get; set; }
    }

    public class PressMachineDataContext : DbContext
    {
        public DbSet<PreSaveModel> PreSaveModels { get; set; }

        //public DbSet<PressHistory> PressHistory { get; set; }
        public readonly string DbPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config");

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (ApplicationAuthTaskFactory.AuthFlag)
            {
                throw new Exception("授权失败，无法获取DB");
            }

            string DbFile =
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "PressMachineDB.db");
            optionsBuilder.UseSqlite("Data Source=" + DbFile);
        }
    }

    public static class PressMachineDataHelper
    {
        public static async Task<bool> Save(PreSaveModel psModel)
        {
            if (ApplicationAuthTaskFactory.AuthFlag)
            {
                throw new Exception("授权失败，无法保存数据");
            }

            try
            {
                await using var db = new PressMachineDataContext();
                db.PreSaveModels.Add(psModel);
                var result = await db.SaveChangesAsync();
                return result == 1;
            }
            catch (Exception ex)
            {
                //var strMessage = JsonConvert.SerializeObject(psModel);
                XLogGlobal.Logger?.LogError($"Save失败 {ex.Message}");
                return false;
            }
        }

        public static async Task<PreSaveModel?> CheckCode(string code)
        {
            if (ApplicationAuthTaskFactory.AuthFlag)
            {
                throw new Exception("授权失败，无法保存数据");
            }

            try
            {
                await using var db = new PressMachineDataContext();
                var result = await db.PreSaveModels.FirstOrDefaultAsync(x => x.Code == code);
                return result;
            }
            catch (Exception ex)
            {
                //var strMessage = JsonConvert.SerializeObject(psModel);
                XLogGlobal.Logger?.LogError($"CheckCode失败 {ex.Message}");
                return null;
            }
        }

        public static async Task<bool> InitializedPressMachineDb()
        {
            try
            {
                await using var db = new PressMachineDataContext();
                db.PreSaveModels.RemoveRange(db.PreSaveModels);
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}